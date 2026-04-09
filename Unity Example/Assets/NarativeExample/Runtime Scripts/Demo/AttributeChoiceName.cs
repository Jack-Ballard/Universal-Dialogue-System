using System;
using TMPro;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class AttributeChoiceName : MonoBehaviour
{

    [SerializeField]
    private GameObject objectButtonPrefab;

    [SerializeField]
    private Transform parentTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddAttributeToAPI()
    {
        Globals.AddAttribute(new Action<System.Object>(ChoiceName));
    }

    public void ChoiceName(System.Object data)
    {
        data = (Tuple<Components, Attribute, Action<int>>)data;
        Components connection = ((Tuple<Components, Attribute, Action<int>>)data).Item1;
        Attribute attribute = ((Tuple<Components, Attribute, Action<int>>)data).Item2;
        NarativeBridge narativeBridge = ((Tuple<Components, Attribute, NarativeBridge>)data).Item3;

        //Console.Write(attribute.Value);
        OptionButtonScript optionButtonScript = Instantiate(objectButtonPrefab, parentTransform).GetComponent<OptionButtonScript>();
        optionButtonScript.Initalise(attribute.Value, connection.ID, narativeBridge.OnOptionSelected);
    }
}
