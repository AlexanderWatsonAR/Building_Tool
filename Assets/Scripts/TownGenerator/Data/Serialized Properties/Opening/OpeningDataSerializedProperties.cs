using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class OpeningDataSerializedProperties : SerializedPropertyGroup
{
    #region Constants
    const string k_Columns = "m_Columns";
    const string k_Rows = "m_Rows";
    const string k_Height = "m_Height";
    const string k_Width = "m_Width";
    #endregion

    public SerializedProperty Columns => m_Data.FindPropertyRelative(k_Columns);
    public SerializedProperty Rows => m_Data.FindPropertyRelative(k_Rows);
    public SerializedProperty Height => m_Data.FindPropertyRelative(k_Height);
    public SerializedProperty Width => m_Data.FindPropertyRelative(k_Width);

    public OpeningDataSerializedProperties(SerializedProperty openingData) : base(openingData)
    {
    }
}
