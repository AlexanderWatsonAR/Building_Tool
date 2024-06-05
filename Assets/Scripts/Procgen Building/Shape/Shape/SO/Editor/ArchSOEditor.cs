using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(ArchSO))]
public class ArchSOEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        serializedObject.Update();

        SerializedProperty arch = serializedObject.FindProperty("m_Shape");

        SerializedProperty height = arch.FindPropertyRelative("m_Height");
        SerializedProperty sides = arch.FindPropertyRelative("m_Sides");

        PropertyField heightField = new PropertyField(height);
        PropertyField sidesField = new PropertyField(sides);

        root.Add(heightField);
        root.Add(sidesField);


        return root;
    }

}
