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
        
        EditorGUILayout.PropertyField(extend);
        EditorGUILayout.PropertyField(height);

        if(extend.floatValue < 0)
            extend.floatValue = 0;

        if(height.floatValue < 0)
            height.floatValue = 0;

        float extendTile = tile.ExtendDistance;
        float heightTile = tile.Height;

        if (serializedObject.ApplyModifiedProperties())
        {
            if(extend.floatValue != extendTile ||
               height.floatValue != heightTile)
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
