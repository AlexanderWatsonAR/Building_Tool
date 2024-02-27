using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CornerDataSerializedProperties
{
    [SerializeField] private SerializedProperty m_CornerData;
    [SerializeField] private SerializedObject m_SerializedObject;

    #region Constants
    const string k_Type = "m_Type";
    const string k_Sides = "m_Sides";
    #endregion

    #region Accessors
    public SerializedObject SerializedObject => m_SerializedObject;
    public SerializedProperty Type => m_CornerData.FindPropertyRelative(k_Type);
    public SerializedProperty Sides => m_CornerData.FindPropertyRelative(k_Sides);
    #endregion

    public CornerDataSerializedProperties(SerializedProperty cornerData)
    {
        m_CornerData = cornerData;
        m_SerializedObject = cornerData.serializedObject;
    }
}
