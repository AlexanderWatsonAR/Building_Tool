using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoofTileDataSerializedProperties : SerializedPropertyGroup
{
    #region Constants
    const string k_Thickness = "m_Thickness";
    const string k_Extend = "m_Extend";
    const string k_Height = "m_Height";
    const string k_Columns = "m_Columns";
    const string k_Rows = "m_Rows";
    #endregion


    #region Accessors
    public SerializedProperty Thickness => m_Data.FindPropertyRelative(k_Thickness);
    public SerializedProperty Extend => m_Data.FindPropertyRelative(k_Extend);
    public SerializedProperty Height => m_Data.FindPropertyRelative(k_Height);
    public SerializedProperty Columns => m_Data.FindPropertyRelative(k_Columns);
    public SerializedProperty Rows => m_Data.FindPropertyRelative(k_Rows);
    #endregion

    public RoofTileDataSerializedProperties(SerializedProperty roofTileData) : base(roofTileData)
    {
    }
}
