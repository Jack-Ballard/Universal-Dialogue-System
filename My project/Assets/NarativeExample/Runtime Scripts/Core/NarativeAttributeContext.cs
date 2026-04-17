using MoonSharp.Interpreter;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.InputSystem;
public sealed class NarrativeAttributeContext
{
    public TextBox TextBox { get; }
    public Components Component { get; }
    public Attribute Attribute { get; }
    public NarativeBridge Bridge { get; }

    public NarrativeAttributeContext(TextBox textBox, Components component, Attribute attribute, NarativeBridge bridge)
    {
        TextBox = textBox;
        Component = component;
        Attribute = attribute;
        Bridge = bridge;
    }
}