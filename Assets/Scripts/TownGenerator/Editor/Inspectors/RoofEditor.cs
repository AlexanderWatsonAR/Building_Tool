using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(Roof))]
public class RoofEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement container = new VisualElement();

        serializedObject.Update();

        SerializedProperty data = serializedObject.FindProperty("m_Data");

        PropertyField roofDataField = new PropertyField(data);
        roofDataField.BindProperty(data);

        container.Add(roofDataField);

        return container;
    }
}
