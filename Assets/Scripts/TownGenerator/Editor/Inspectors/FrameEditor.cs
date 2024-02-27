using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Frame))]
public class FrameEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        serializedObject.Update();

        VisualElement container = new VisualElement();

        SerializedProperty data = serializedObject.FindProperty("m_Data");

        PropertyField dataField = new PropertyField(data);
        dataField.BindProperty(data);

        container.Add(dataField);

        return container;
    }
}
