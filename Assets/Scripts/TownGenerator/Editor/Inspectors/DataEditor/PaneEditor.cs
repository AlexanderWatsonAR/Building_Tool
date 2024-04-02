using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Pane))]
public class PaneEditor : DataEditor
{
    public override VisualElement CreateInspectorGUI()
    {
        return base.CreateInspectorGUI();
    }
}
