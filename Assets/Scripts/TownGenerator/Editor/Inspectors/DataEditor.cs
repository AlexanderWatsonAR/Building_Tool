using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class DataEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        serializedObject.Update();

        SerializedProperty data = serializedObject.FindProperty("m_Data");

        PropertyField dataField = new PropertyField(data);
        dataField.BindProperty(data);

        root.Add(dataField);

        return root;
    }
}
