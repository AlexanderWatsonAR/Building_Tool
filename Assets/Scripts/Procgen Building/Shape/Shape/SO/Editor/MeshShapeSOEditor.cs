using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


[CustomEditor(typeof(MeshShapeScriptableObject))]
public class MeshShapeSOEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        SerializedProperty meshShape = serializedObject.FindProperty("m_Shape");
        SerializedProperty mesh = meshShape.FindPropertyRelative("m_Mesh");
        SerializedProperty index = meshShape.FindPropertyRelative("m_Index");

        PropertyField meshField = new PropertyField(mesh);
        PropertyField indexField = new PropertyField(index);

        root.Add(meshField);
        root.Add(indexField);

        return root;
    }

}
