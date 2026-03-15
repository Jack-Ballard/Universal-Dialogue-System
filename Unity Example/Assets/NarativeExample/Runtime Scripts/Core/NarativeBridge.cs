using System.Linq;
using UnityEngine;
using TMPro;
using System;

[AddComponentMenu("Narrative Bridge/Narative Bridge Component")]
public class NarativeBridge : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshProUGUI;

    [SerializeField]
    private GameObject objectButtonPrefab;

    [SerializeField]
    private Transform parentTransform;

    int currentTextBoxID = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Importer.ImportFile();
        OnPresentOptions();
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void OnOptionSelected(int ID)
    {
        currentTextBoxID = Globals.connections.First(connection => connection.Item1 == currentTextBoxID && connection.Item2 == Globals.textBoxes[currentTextBoxID].Connections[(ID)].ID).Item3;
        foreach (Transform child in parentTransform)
        {
            Destroy(child.gameObject);
        }
        OnPresentOptions();
    }

    public void OnPresentOptions()
    {
        string text = LuaLogic.FormatByLua(Globals.textBoxes.First(textBox => textBox.ID == currentTextBoxID).TextContent);
        textMeshProUGUI.text = text;

        foreach (Connection connection in Globals.textBoxes[currentTextBoxID].Connections)
        {
            foreach (Attribute attribute in connection.Attributes)
            {
                if (attribute.Name == "ChoiceName")
                {
                    //Console.Write(attribute.Value);
                    OptionButtonScript optionButtonScript = Instantiate(objectButtonPrefab, parentTransform).GetComponent<OptionButtonScript>();
                    optionButtonScript.Initalise(attribute.Value, connection.ID, OnOptionSelected);
                    break;
                }
            }
            Console.WriteLine();
        }
    }
}
