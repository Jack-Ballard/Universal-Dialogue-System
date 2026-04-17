using MoonSharp.Interpreter;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.MemoryProfiler;
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

    int currentTextBoxID = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Importer.ImportFile(dialogueFileName);
        //OnPresentOptions();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTextBoxID == -1)
        {
            currentTextBoxID = 0;
            OnPresentOptions();
            Globals.Export();
        }
    }

    public void OnOptionSelected(int ID)
    {
        bool validOption = false;
        List<Connection> connections = Globals.connections.Where(connection => connection.FromTextBoxID == currentTextBoxID && connection.FromComponentID == ID).ToList();
        for (int j = 0; j < connections.Count; j++)
        {
            //Add check for default outgoing connection
            if (connections[j].FromConnectionID == 0)
            {
                currentTextBoxID = connections[j].ToTextBoxID;
                validOption = true;
                break;
            }
            List<Condition> nodeConditions = new List<Condition>(Globals.textBoxes[currentTextBoxID].Components[ID-1].OutgoingConnections[connections[j].FromConnectionID-1].Conditions);
            for (int i = 0; i < nodeConditions.Count; i++)
            {
                // If you need to evaluate a condition, you may need to use nodeConditions[i].Value directly
                if (LuaLogic.EvaluateLuaCondition(nodeConditions[i].Value))
                {
                    currentTextBoxID = connections[j].ToTextBoxID;
                    validOption = true;
                    break;
                }
            }
            if (validOption)
            {
                break;
            }
        }

        if(!validOption)
        {
            //fallback if no conditions conditions are true
            currentTextBoxID = connections.First(connection => connection.FromConnectionID == connections.Count-1).ToTextBoxID;
        }

        foreach (Transform child in parentTransform)
        {
            Destroy(child.gameObject);
        }
        OnPresentOptions();
    }

    public void OnPresentOptions()
    {
        TextBox textBox = Globals.textBoxes.First(textBox => textBox.ID == currentTextBoxID);
        string text = LuaLogic.FormatByLua(textBox.TextContent);
        textMeshProUGUI.text = text;

        foreach(Attribute attribute in textBox.Attributes)
        {
            if (Globals.TryGetAttribute(attribute.Name, out Action<NarrativeAttributeContext> attributeHandler))
            {
                attributeHandler.Invoke(new NarrativeAttributeContext(textBox, new Components(null), attribute, this));
            }
        }

        foreach (Components connectionComponent in textBox.Components)
        {
            if (connectionComponent.ID == 0)
            {
                OptionButtonScript optionButtonScript = Instantiate(objectButtonPrefab, parentTransform).GetComponent<OptionButtonScript>();
                optionButtonScript.Initalise("Continue", connectionComponent.ID, OnOptionSelected);
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

            foreach (Attribute attribute in connectionComponent.Attributes)
            {
                if (Globals.TryGetAttribute(attribute.Name, out Action<NarrativeAttributeContext> attributeHandler))
                {
                    attributeHandler.Invoke(new NarrativeAttributeContext(textBox, connectionComponent, attribute, this));
                }
            }
        }
    }
}
