using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoofTileDataSerializedProperties
{
    [SerializeField] private SerializedProperty m_RoofTileData;

    #region Accessors
    public SerializedProperty Thickness => m_RoofTileData.FindPropertyRelative("m_Thickness");
    public SerializedProperty Extend => m_RoofTileData.FindPropertyRelative("m_Extend");
    public SerializedProperty Height => m_RoofTileData.FindPropertyRelative("m_Height");
    public SerializedProperty Columns => m_RoofTileData.FindPropertyRelative("m_Columns");
    public SerializedProperty Rows => m_RoofTileData.FindPropertyRelative("m_Rows");
    #endregion

    public RoofTileDataSerializedProperties(SerializedProperty roofTileData)
    {
        m_RoofTileData = roofTileData;
    }
}
