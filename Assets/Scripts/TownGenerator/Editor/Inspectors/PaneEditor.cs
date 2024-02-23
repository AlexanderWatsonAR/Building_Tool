using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Pane))]
public class PaneEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        serializedObject.Update();

        VisualElement root = new VisualElement();

        SerializedProperty data = serializedObject.FindProperty("m_Data");

        PropertyField dataField = new PropertyField(data);
        dataField.BindProperty(data);

        root.Add(dataField);

        return root;
    }
}
