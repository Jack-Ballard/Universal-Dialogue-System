using MoonSharp.Interpreter;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("Narrative Bridge/Narative Bridge Component")]
public class NarativeBridge : MonoBehaviour
{

    [SerializeField]
    public TextMeshProUGUI textMeshProUGUI;

    [SerializeField]
    private GameObject objectButtonPrefab;

    [SerializeField]
    private Transform parentTransform;

    [SerializeField]
    private string dialogueFileName;

    [SerializeField]
    private float textRevealDelay = 0.05f;

    [Header("Audio Settings")]
    [SerializeField]
    private AudioClip optionSelectAudio;

    [SerializeField]
    private AudioClip typingAudio;

    [SerializeField]
    private AudioSource audioSource;

    int currentTextBoxID = -1;
    private UnityEngine.Coroutine typingCoroutine;

    // Additional Tracking Variables for Input Handling
    private bool isTyping = false;
    private bool waitingForContinue = false;

    [HideInInspector]
    public string currentFullText = "";
    private TextBox activeTextBox; 

    void Start()
    {
        if (audioSource == null) 
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        Importer.ImportFile(dialogueFileName);
    }

    void Update()
    {
        if (currentTextBoxID == -1)
        {
            currentTextBoxID = 0;
            OnOptionSelected(0);
            Globals.Export();
        }

        // Use the New Input System to check for Space bar or Left Mouse Click
        bool inputPressed = (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
                            (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);

        if (currentTextBoxID != -1 && inputPressed)
        {
            if (isTyping)
            {
                // Fast-forward text if currently typing
                isTyping = false;
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }
                textMeshProUGUI.text = currentFullText;
                ShowOptions(activeTextBox); // Reveal the choices visually right away
            }
            else if (waitingForContinue)
            {
                // Trigger the default continue path automatically
                waitingForContinue = false;
                OnOptionSelected(0);
            }
        }
    }

    public void OnOptionSelected(int ID)
    {
        // ------------- Play Audio -------------
        if (optionSelectAudio != null)
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(optionSelectAudio);
            }
            else
            {
                // Fallback to playing audio at the camera/transform position if no audioSource is assigned
                AudioSource.PlayClipAtPoint(optionSelectAudio, Camera.main != null ? Camera.main.transform.position : transform.position);
            }
        }
        // --------------------------------------

        isTyping = false;
        waitingForContinue = false;
        
        bool validOption = false;
        List<Connection> connections = Globals.connections.Where(connection => connection.FromTextBoxID == currentTextBoxID && connection.FromComponentID == ID).ToList();
        for (int j = 0; j < connections.Count; j++)
        {
            if (connections[j].FromConnectionID == 0)
            {
                currentTextBoxID = connections[j].ToTextBoxID;
                validOption = true;
                break;
            }
            List<Condition> nodeConditions = new List<Condition>(Globals.textBoxes[currentTextBoxID].Components[ID - 1].OutgoingConnections[connections[j].FromConnectionID - 1].Conditions);
            for (int i = 0; i < nodeConditions.Count; i++)
            {
                if (LuaLogic.EvaluateLuaCondition(nodeConditions[i].Value))
                {
                    currentTextBoxID = connections[j].ToTextBoxID;
                    validOption = true;
                    break;
                }
            }
            if (validOption) break;
        }

        if (!validOption)
        {
            currentTextBoxID = connections.First(connection => connection.FromConnectionID == connections.Count - 1).ToTextBoxID;
        }

        foreach (Transform child in parentTransform)
        {
            Destroy(child.gameObject);
        }

        OnPresentOptions();
    }

    public void OnPresentOptions()
    {
        activeTextBox = Globals.textBoxes.First(textBox => textBox.ID == currentTextBoxID);
        currentFullText = LuaLogic.FormatByLua(activeTextBox.TextContent);
        textMeshProUGUI.text = "";
        
        isTyping = true;
        waitingForContinue = false;

        foreach (Attribute attribute in activeTextBox.Attributes)
        {
            if (Globals.TryGetAttribute(attribute.Name, out Action<NarrativeAttributeContext> attributeHandler))
            {
                attributeHandler.Invoke(new NarrativeAttributeContext(activeTextBox, new Components(null), attribute, this));
            }
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(RevealTextAndOptions(currentFullText, activeTextBox));
    }

    private IEnumerator RevealTextAndOptions(string text, TextBox textBox)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (typingAudio != null)
            {
                if (audioSource != null)
                {
                    audioSource.PlayOneShot(typingAudio);
                }
                else
                {
                    // Fallback to playing audio at the camera/transform position if no audioSource is assigned
                    AudioSource.PlayClipAtPoint(optionSelectAudio, Camera.main != null ? Camera.main.transform.position : transform.position);
                }
            }
            textMeshProUGUI.text += text[i];
            yield return new WaitForSeconds(textRevealDelay);
        }

        isTyping = false;
        ShowOptions(textBox);
    }

    // Extracted Display Logic into its own method to support Fast-Forwarding 
    private void ShowOptions(TextBox textBox)
    {
        bool hasOptions = false;

        foreach (Components connectionComponent in textBox.Components)
        {
            if (connectionComponent.ID == 0)
            {
                // Flag that we are waiting for a user click/spacebar instead of drawing a button
                waitingForContinue = true;
                continue; 
            }

            bool conditionMet = true;
            foreach (Condition condition in connectionComponent.Conditions)
            {
                if (!LuaLogic.EvaluateLuaCondition(condition.Value))
                {
                    conditionMet = false;
                    break;
                }
            }
            if (!conditionMet) continue;

            hasOptions = true;
            foreach (Attribute attribute in connectionComponent.Attributes)
            {
                if (Globals.TryGetAttribute(attribute.Name, out Action<NarrativeAttributeContext> attributeHandler))
                {
                    attributeHandler.Invoke(new NarrativeAttributeContext(textBox, connectionComponent, attribute, this));
                }
            }
        }

        // Failsafe: if there are no literal dialog choices spawned yet, await generic continue
        if (!hasOptions)
        {
            waitingForContinue = true;
        }
    }
}
