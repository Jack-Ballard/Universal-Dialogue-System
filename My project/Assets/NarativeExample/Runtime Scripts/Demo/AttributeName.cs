using TMPro;
using UnityEngine;

public class AttributeName : MonoBehaviour
{
    private void OnEnable()
    {
        Globals.AddAttribute("Name", ApplyName);
    }

    private void OnDisable()
    {
        Globals.RemoveAttribute("Name", ApplyName);
    }

    private void ApplyName(NarrativeAttributeContext context)
    {
        string speakerName = context.Attribute.Value;
        if (string.IsNullOrWhiteSpace(speakerName))
        {
            return;
        }

        //context.Bridge.textMeshProUGUI.text = speakerName + ": " + context.Bridge.textMeshProUGUI.text;
        context.Bridge.currentFullText = speakerName + ": " + context.Bridge.currentFullText;
        return;
    }
}
