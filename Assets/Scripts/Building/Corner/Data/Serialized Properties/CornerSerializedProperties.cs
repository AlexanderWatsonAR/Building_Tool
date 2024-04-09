using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CornerDataSerializedProperties : SerializedPropertyGroup
{
    #region Constants
    const string k_Type = "m_Type";
    const string k_Sides = "m_Sides";
    #endregion

    #region Accessors
    public SerializedProperty Type => m_Data.FindPropertyRelative(k_Type);
    public SerializedProperty Sides => m_Data.FindPropertyRelative(k_Sides);
    #endregion

    public CornerDataSerializedProperties(SerializedProperty cornerData) : base(cornerData)
    {
        m_Data = cornerData;
    }
}
