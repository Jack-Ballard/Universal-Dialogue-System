using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttributeSprite : MonoBehaviour
{
    [Serializable]
    private struct SpriteEntry
    {
        public string key;
        public Sprite sprite;
    }

    [Serializable]
    private struct SpriteTarget
    {
        public string targetName;
        public SpriteRenderer spriteRenderer;
        public Transform transform;
    }

    [SerializeField]
    private SpriteTarget[] spriteTargets;

    [SerializeField]
    private SpriteEntry[] sprites;

    private readonly Dictionary<string, Sprite> spriteLookup = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, SpriteTarget> targetLookup = new Dictionary<string, SpriteTarget>(StringComparer.OrdinalIgnoreCase);

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

        targetLookup.Clear();
        for (int i = 0; i < spriteTargets.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(spriteTargets[i].targetName))
            {
                targetLookup[spriteTargets[i].targetName] = spriteTargets[i];
            }
        }
    }

    private void OnEnable()
    {
        Globals.AddAttribute("SpriteTexture", SetSpriteTexture);
        Globals.AddAttribute("MoveSprite", MoveSpriteAbsolute);
        Globals.AddAttribute("MoveSpriteBy", MoveSpriteRelative);
        
        Globals.AddFunction((Func<string, string>)GetSpriteTextureName);
        Globals.AddFunction((Action<string, string>)SetSpriteTexture);
    }

    private void OnDisable()
    {
        Globals.RemoveAttribute("SpriteTexture", SetSpriteTexture);
        Globals.RemoveAttribute("MoveSprite", MoveSpriteAbsolute);
        Globals.RemoveAttribute("MoveSpriteBy", MoveSpriteRelative);
    }

    private void SetSpriteTexture(NarrativeAttributeContext context)
    {
        if (!TryGetTarget(context, out SpriteTarget target)) return;

        if (target.spriteRenderer == null)
        {
            Debug.LogWarning($"AttributeSprite: spriteRenderer for '{target.targetName}' is not assigned.");
            return;
        }

        string spriteKey = (context.Attribute.Value ?? string.Empty).Trim();
        if (!spriteLookup.TryGetValue(spriteKey, out Sprite sprite))
        {
            Debug.LogWarning($"AttributeSprite: unknown sprite key '{spriteKey}'.");
            return;
        }

        target.spriteRenderer.sprite = sprite;
        target.spriteRenderer.size = new Vector2(1,1);
    }

    private void SetSpriteTexture(string spriteName, string spriteTexture)
    {
        
        if(!targetLookup.TryGetValue(spriteName, out SpriteTarget target))
        {
            Debug.LogWarning($"AttributeSprite: no such sprite as '{spriteName}'.");
            return;
        }
        if (!spriteLookup.TryGetValue(spriteTexture, out Sprite sprite))
        {
            Debug.LogWarning($"AttributeSprite: unknown sprite key '{spriteTexture}'.");
            return;
        }
        target.spriteRenderer.sprite = sprite;
        target.spriteRenderer.size = new Vector2(1, 1);
    }

    private string GetSpriteTextureName(string spriteName)
    {
        if (!targetLookup.TryGetValue(spriteName, out SpriteTarget target))
        {
            Debug.LogWarning($"AttributeSprite: no such sprite as '{spriteName}'.");
            return null;
        }
        if (target.spriteRenderer == null || target.spriteRenderer.sprite == null)
        {
            Debug.LogWarning($"AttributeSprite: spriteRenderer for '{spriteName}' is not assigned or has no sprite.");
            return null;
        }
        string currentSpriteName = null;
        foreach (var kvp in spriteLookup)
        {
            if (kvp.Value == target.spriteRenderer.sprite)
            {
                currentSpriteName = kvp.Key;
                break;
            }
        }
        return currentSpriteName;
    }


    private void MoveSpriteAbsolute(NarrativeAttributeContext context)
    {
        if (!TryGetTarget(context, out SpriteTarget target)) return;

        if (target.transform == null)
        {
            Debug.LogWarning($"AttributeSprite: transform for '{target.targetName}' is not assigned.");
            return;
        }

        if (!TryParseMovement(context.Attribute.Value, out Vector3 position, out float time))
        {
            Debug.LogWarning($"AttributeSprite: invalid MoveSprite value '{context.Attribute.Value}'. Expected 'x,y,z' or 'x,y,z,time'.");
            return;
        }

        if (time > 0f)
        {
            StartCoroutine(DoMoveOverTime(target.transform, target.transform.position, position, time));
        }
        else
        {
            target.transform.position = position;
        }
    }

    private void MoveSpriteRelative(NarrativeAttributeContext context)
    {
        if (!TryGetTarget(context, out SpriteTarget target)) return;

        if (target.transform == null)
        {
            Debug.LogWarning($"AttributeSprite: transform for '{target.targetName}' is not assigned.");
            return;
        }

        if (!TryParseMovement(context.Attribute.Value, out Vector3 delta, out float time))
        {
            Debug.LogWarning($"AttributeSprite: invalid MoveSpriteBy value '{context.Attribute.Value}'. Expected 'x,y,z' or 'x,y,z,time'.");
            return;
        }

        Vector3 targetPosition = target.transform.position + delta;

        if (time > 0f)
        {
            StartCoroutine(DoMoveOverTime(target.transform, target.transform.position, targetPosition, time));
        }
        else
        {
            target.transform.position = targetPosition;
        }
    }

    private IEnumerator DoMoveOverTime(Transform target, Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (target == null) yield break; // Safety check in case object is destroyed

            target.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (target != null)
        {
            target.position = endPos;
        }
    }

    private bool TryGetTarget(NarrativeAttributeContext context, out SpriteTarget target)
    {
        target = default;
        if (!TryGetAttributeValue(context.TextBox, "Name", out string targetName))
        {
            return false; // No speaker name assigned to this text box
        }

        return targetLookup.TryGetValue(targetName, out target);
    }

    private static bool TryGetAttributeValue(TextBox textBox, string attributeName, out string value)
    {
        value = null;
        if (textBox.Attributes == null) return false;

        foreach (var attr in textBox.Attributes)
        {
            if (string.Equals(attr.Name, attributeName, StringComparison.OrdinalIgnoreCase))
            {
                value = attr.Value;
                return true;
            }
        }
        return false;
    }

    private static bool TryParseMovement(string input, out Vector3 result, out float time)
    {
        result = Vector3.zero;
        time = 0f;
        if (string.IsNullOrWhiteSpace(input)) return false;

        string[] parts = input.Split(',');
        if (parts.Length < 3 || parts.Length > 4) return false;

        if (float.TryParse(parts[0].Trim(), out float x) &&
            float.TryParse(parts[1].Trim(), out float y) &&
            float.TryParse(parts[2].Trim(), out float z))
        {
            result = new Vector3(x, y, z);

            // Try parse the optional duration if available
            if (parts.Length == 4 && float.TryParse(parts[3].Trim(), out float parsedTime))
            {
                time = parsedTime;
            }
            return true;
        }
        return false;
    }
}
