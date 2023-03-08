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
        
        EditorGUILayout.PropertyField(extend);

        if(extend.floatValue < 0)
            extend.floatValue = 0;

        float extendTile = tile.ExtendDistance;

        if (serializedObject.ApplyModifiedProperties())
        {
            if(extend.floatValue != extendTile)
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
