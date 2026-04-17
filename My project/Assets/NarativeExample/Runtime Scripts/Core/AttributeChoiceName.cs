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

    private void OnEnable()
    {
        Globals.AddAttribute("ChoiceName", ChoiceName);
    }

    private void OnDisable()
    {
        Globals.RemoveAttribute("ChoiceName", ChoiceName);
    }

    private void ChoiceName(NarrativeAttributeContext context)
    {
        OptionButtonScript optionButtonScript = Instantiate(objectButtonPrefab, parentTransform).GetComponent<OptionButtonScript>();
        optionButtonScript.Initalise(context.Attribute.Value, context.Component.ID, context.Bridge.OnOptionSelected);
    }
}
