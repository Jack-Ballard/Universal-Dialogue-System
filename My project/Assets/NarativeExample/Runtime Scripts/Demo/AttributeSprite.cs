using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class AttributeSprite : MonoBehaviour
{
    [Serializable]
    private struct SpriteEntry
    {
        public string key;
        public Sprite sprite;
    }

    [SerializeField]
    private string spriteName;

    [SerializeField]
    private SpriteRenderer targetSpriteRenderer;

    [SerializeField]
    private Transform targetTransform;

    [SerializeField]
    private SpriteEntry[] sprites;

    private readonly Dictionary<string, Sprite> spriteLookup = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);

    private void Awake()
    {
        spriteLookup.Clear();

        for (int i = 0; i < sprites.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(sprites[i].key) && sprites[i].sprite != null)
            {
                spriteLookup[sprites[i].key] = sprites[i].sprite;
            }
        }
    }

    private void OnEnable()
    {
        Globals.AddAttribute("SpriteTexture", SetSpriteTexture);
        Globals.AddAttribute("MoveSprite", MoveSpriteAbsolute);
        Globals.AddAttribute("MoveSpriteBy", MoveSpriteRelative);
    }

    private void OnDisable()
    {
        Globals.RemoveAttribute("SpriteTexture", SetSpriteTexture);
        Globals.RemoveAttribute("MoveSprite", MoveSpriteAbsolute);
        Globals.RemoveAttribute("MoveSpriteBy", MoveSpriteRelative);
    }

    private void SetSpriteTexture(NarrativeAttributeContext context)
    {
        if (!IsForThisSprite(context))
        {
            return;
        }

        if (targetSpriteRenderer == null)
        {
            Debug.LogWarning("AttributeSprite: targetSpriteRenderer is not assigned.");
            return;
        }

        string spriteKey = (context.Attribute.Value ?? string.Empty).Trim();

        if (!spriteLookup.TryGetValue(spriteKey, out Sprite sprite))
        {
            Debug.LogWarning("AttributeSprite: unknown sprite key '" + spriteKey + "'.");
            return;
        }

        targetSpriteRenderer.sprite = sprite;
        targetSpriteRenderer.size = new Vector2(1,1);
    }

    private void MoveSpriteAbsolute(NarrativeAttributeContext context)
    {
        if (!IsForThisSprite(context))
        {
            return;
        }

        if (targetTransform == null)
        {
            Debug.LogWarning("AttributeSprite: targetTransform is not assigned.");
            return;
        }

        if (!TryParseVector3(context.Attribute.Value, out Vector3 position))
        {
            Debug.LogWarning("AttributeSprite: invalid MoveSprite value '" + context.Attribute.Value + "'. Expected 'x,y,z'.");
            return;
        }

        targetTransform.position = position;
    }

    private void MoveSpriteRelative(NarrativeAttributeContext context)
    {
        if (!IsForThisSprite(context))
        {
            return;
        }

        if (targetTransform == null)
        {
            Debug.LogWarning("AttributeSprite: targetTransform is not assigned.");
            return;
        }

        if (!TryParseVector3(context.Attribute.Value, out Vector3 delta))
        {
            Debug.LogWarning("AttributeSprite: invalid MoveSpriteBy value '" + context.Attribute.Value + "'. Expected 'x,y,z'.");
            return;
        }

        targetTransform.position += delta;
    }

    private bool IsForThisSprite(NarrativeAttributeContext context)
    {
        if (!TryGetAttributeValue(context.TextBox, "Name", out string targetName))
        {
            return false;
        }

        return string.Equals(targetName, spriteName, StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryGetAttributeValue(TextBox textBox, string attributeName, out string value)
    {
        value = null;

        for (int i = 0; i < textBox.Attributes.Count; i++)
        {
            Attribute attribute = textBox.Attributes[i];
            if (string.Equals(attribute.Name, attributeName, StringComparison.OrdinalIgnoreCase))
            {
                value = attribute.Value;
                return true;
            }
        }

        return false;
    }

    private static bool TryParseVector3(string value, out Vector3 result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        string[] parts = value.Split(',');
        if (parts.Length != 3)
        {
            return false;
        }

        if (!float.TryParse(parts[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float x))
        {
            return false;
        }

        if (!float.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float y))
        {
            return false;
        }

        if (!float.TryParse(parts[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
        {
            return false;
        }

        result = new Vector3(x, y, z);
        return true;
    }
}
