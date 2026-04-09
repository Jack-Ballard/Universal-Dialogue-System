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
    private TextMeshProUGUI textMeshProUGUI;

    [SerializeField]
    private GameObject objectButtonPrefab;

    [SerializeField]
    private Transform parentTransform;

    int currentTextBoxID = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Importer.ImportFile();
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

            if(Globals.textBoxes[currentTextBoxID].Components[ID].OutgoingConnections[connections[j].FromConnectionID].ID == 0)
            {
                currentTextBoxID = connections[j].ToTextBoxID;
                validOption = true;
                break;
            }
            List<Condition> nodeConditions = new List<Condition>(Globals.textBoxes[currentTextBoxID].Components[ID].OutgoingConnections[connections[j].FromConnectionID].Conditions);
            for (int i = 0; i < nodeConditions.Count; i++)
            {
                if (Globals.attributes.TryGetValue(nodeConditions[i].Value, out Delegate conditionFunction))
                {
                    if (LuaLogic.EvaluateLuaCondition(conditionFunction.ToString()))
                    {
                        currentTextBoxID = connections[j].ToTextBoxID;
                        validOption = true;
                        break;
                    }
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

        foreach (Components connectionComponent in textBox.Components)
        {
            
            if(connectionComponent.ID==0)
            {
                //Console.Write(attribute.Value);
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
                
                if (attribute.Name == "ChoiceName")
                {
                    //Console.Write(attribute.Value);
                    OptionButtonScript optionButtonScript = Instantiate(objectButtonPrefab, parentTransform).GetComponent<OptionButtonScript>();
                    optionButtonScript.Initalise(attribute.Value, connectionComponent.ID, OnOptionSelected);
                    break;
                }
                if (Globals.attributes.TryGetValue(attribute.Name, out var attributeFunction))
                {
                    attributeFunction = attributeFunction as Action<System.Object>;
                    attributeFunction?.DynamicInvoke(new Tuple<Components, Attribute, NarativeBridge>(connectionComponent, attribute, this));
                    break; // If only one attribute per connection should be handled
                }
            }
            Console.WriteLine();
        }
    }
}
