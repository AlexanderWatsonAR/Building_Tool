using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine.ProBuilder.Shapes;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(WallSection))]
public class WallSectionEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement container = new VisualElement();

        serializedObject.Update();

        SerializedProperty data = serializedObject.FindProperty("m_Data");

        PropertyField dataField = new PropertyField(data);
        dataField.BindProperty(data);

        container.Add(dataField);

        return container;
    }
}
