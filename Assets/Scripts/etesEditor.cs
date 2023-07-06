using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(etes))]
public class EtesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        etes test = (etes)target;

        if (GUILayout.Button("Test"))
        {
            test.DoTest();
        }
    }

}
