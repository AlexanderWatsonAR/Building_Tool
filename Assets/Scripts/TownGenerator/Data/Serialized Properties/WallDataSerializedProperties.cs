using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WallDataSerializedProperties
{
    private SerializedProperty m_WallData;
    private SerializedObject m_SerializedObject;

    #region Accessors
    public SerializedObject SerializedObject => m_SerializedObject;
    public SerializedProperty Height => m_WallData.FindPropertyRelative("m_Height");
    public SerializedProperty Depth => m_WallData.FindPropertyRelative("m_Depth");
    public SerializedProperty Columns => m_WallData.FindPropertyRelative("m_Columns");
    public SerializedProperty Rows => m_WallData.FindPropertyRelative("m_Rows");
    #endregion

    public WallDataSerializedProperties(SerializedProperty wallData)
    {
        m_WallData = wallData;
        m_SerializedObject = wallData.serializedObject;
    }

}
