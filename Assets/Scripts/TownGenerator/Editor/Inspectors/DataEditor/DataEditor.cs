using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class DataEditor : Editor
{
    protected SerializedProperty m_Data;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        serializedObject.Update();

        m_Data = serializedObject.FindProperty("m_Data");

        PropertyField dataField = new PropertyField(m_Data);
        dataField.BindProperty(m_Data);

        root.Add(dataField);

        return root;
    }
}
