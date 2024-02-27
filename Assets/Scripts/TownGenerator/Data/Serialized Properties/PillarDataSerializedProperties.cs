using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PillarDataSerializedProperties : SerializedPropertyGroup
{
    #region Constants
    const string k_Height = "m_Height";
    const string k_Width = "m_Width";
    const string k_Depth = "m_Depth";
    const string k_Sides = "m_Sides";
    const string k_IsSmooth = "m_IsSmooth";
    #endregion

    #region Accessors
    public SerializedProperty Height => m_Data.FindPropertyRelative(k_Height);
    public SerializedProperty Width => m_Data.FindPropertyRelative(k_Width);
    public SerializedProperty Depth => m_Data.FindPropertyRelative(k_Depth);
    public SerializedProperty Sides => m_Data.FindPropertyRelative(k_Sides);
    public SerializedProperty IsSmooth => m_Data.FindPropertyRelative(k_IsSmooth);
    #endregion

    public PillarDataSerializedProperties(SerializedProperty pillarData) : base(pillarData)
    {
    }
}
