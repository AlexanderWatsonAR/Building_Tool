using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(NPolygonScriptableObject))]
public class NPolygonSOEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement() { name = nameof(NPolygonScriptableObject) + " Root" };

        serializedObject.Update();
        SerializedProperty nPolygon = serializedObject.FindProperty("m_Shape");
        SerializedProperty sides = nPolygon.FindPropertyRelative("m_Sides");
        PropertyField sidesField = new PropertyField(sides);

        root.Add(sidesField);

        return root;
    }
}
