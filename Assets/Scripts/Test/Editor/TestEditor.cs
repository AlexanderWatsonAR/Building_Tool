using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.EditorTools;
using OnlyInvalid.ProcGenBuilding.Common;

public class Drawable
{

}

[CustomEditor(typeof(Drawable))]
public class TestEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        return base.CreateInspectorGUI();
    }

    void DisplayMessages(DrawState state)
    {

    }
}
