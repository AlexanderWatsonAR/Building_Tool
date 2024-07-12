using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(ArchScriptableObject))]
public class ArchSOEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        serializedObject.Update();

        SerializedProperty arch = serializedObject.FindProperty("m_Shape");

        SerializedProperty topHeight = arch.FindPropertyRelative("m_TopHeight");
        SerializedProperty bottomHeight = arch.FindPropertyRelative("m_BottomHeight");
        SerializedProperty sides = arch.FindPropertyRelative("m_Sides");

        PropertyField topHeightField = new PropertyField(topHeight);
        PropertyField bottomHeightField = new PropertyField(bottomHeight);
        PropertyField sidesField = new PropertyField(sides);

        root.Add(topHeightField);
        root.Add(bottomHeightField);
        root.Add(sidesField);


        return root;
    }

}
