using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GridFrameDataSerializedProperties : FrameDataSerializedProperties
{
    #region Constants
    const string k_Columns = "m_Columns";
    const string k_Rows = "m_Rows";
    #endregion

    #region Accessors
    public SerializedProperty Columns => m_Data.FindPropertyRelative(k_Columns);
    public SerializedProperty Rows => m_Data.FindPropertyRelative(k_Rows);
    #endregion


    public GridFrameDataSerializedProperties(SerializedProperty data) : base(data)
    {

    }
}
