using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Extrudable))]
public class ExtrudableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

}
