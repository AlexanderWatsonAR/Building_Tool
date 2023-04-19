using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tile)), CanEditMultipleObjects]
public class TileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Tile tile = (Tile)target;
        Tile[] tiles = new Tile[targets.Length];

        if(tiles.Length > 1)
        {
            for(int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = (Tile)targets[i];
            }
        }

        SerializedProperty extend = serializedObject.FindProperty("m_Extend");
        SerializedProperty height = serializedObject.FindProperty("m_Height");
        SerializedProperty scale = serializedObject.FindProperty("m_Scale");

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(extend);
        EditorGUILayout.PropertyField(height);
        EditorGUILayout.PropertyField(scale);

        if (extend.floatValue < 0)
            extend.floatValue = 0;

        if(height.floatValue < 0)
            height.floatValue = 0;

        float extendTile = tile.ExtendDistance;
        float heightTile = tile.Height;
        float scaleTile = tile.Scale;

        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Tile Property data changed");
        }

        if (serializedObject.ApplyModifiedProperties())
        {
            if(extend.floatValue != extendTile ||
               height.floatValue != heightTile ||
               scale.floatValue != scaleTile)
            {
                if(tiles.Length > 1)
                {
                    foreach(Tile t in tiles)
                    {
                        t.StartConstruction();
                    }
                }
                else
                {
                    tile.StartConstruction();
                }
                
            }
        }
    }

}
