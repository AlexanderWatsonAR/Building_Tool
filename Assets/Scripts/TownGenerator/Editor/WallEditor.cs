using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;

[CustomEditor(typeof(Wall))]
public class WallEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.ObjectField(serializedObject.FindProperty("m_WallOutlinePrefab"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_WallLength"));

        if (serializedObject.ApplyModifiedProperties())
        {
            Wall wall = (Wall)serializedObject.targetObject;
            wall.Initialize(wall).Build();
        }
    }
}
