using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(Storey))]
public class StoreyEditor : DataEditor
{
    public override VisualElement CreateInspectorGUI()
    {
        return base.CreateInspectorGUI();
    }

}
