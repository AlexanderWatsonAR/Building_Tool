using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Pillar))]
public class PillarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Pillar pillar = (Pillar)target;
        serializedObject.Update();

        SerializedProperty data = serializedObject.FindProperty("m_Data");

        bool enter = true;
        while (data.NextVisible(enter))
        {
            EditorGUILayout.PropertyField(data);
        }

        if(serializedObject.ApplyModifiedProperties())
        {
            pillar.Build();
        }
    }
}
