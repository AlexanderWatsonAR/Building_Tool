using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OnlyInvalid.ProcGenBuilding.Building;

public class ExportEditorWindow : EditorWindow
{
    private bool m_MergeToSingleGameObject = false;

    public static void ShowWindow()
    {
        GetWindow(typeof(ExportEditorWindow), false, "Asset Exporter");
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;

        GUILayout.Label("Export Settings");
        GUILayout.Toggle(m_MergeToSingleGameObject, "Export as Group");

        if (GUILayout.Button("Export Asset"))
        {
            if(Selection.activeGameObject.TryGetComponent(out Building building))
            {
                string fileName = EditorUtility.SaveFilePanel("Export Location", "", "Building", "prefab");

                PrefabUtility.SaveAsPrefabAsset(building.gameObject, fileName);

                //AssetDatabase.CreateAsset(building.gameObject, fileName);
            }
        }
    }

}
