using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(District))]
public class DistrictEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        District districtScript = (District)target;

        if (GUILayout.Button("Build"))
        {
            districtScript.Build();
        }

        if (GUILayout.Button("Reset"))
        {
            districtScript.RevertToPolyshape();
        }
    }
}
