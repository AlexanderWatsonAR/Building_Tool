using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class OpeningDataSerializedProperties
{
    [SerializeField] protected SerializedObject m_SerializedObject;
    [SerializeField] protected SerializedProperty m_OpeningData;

    public SerializedObject SerializedObject => m_SerializedObject;
    public SerializedProperty Columns => m_OpeningData.FindPropertyRelative("m_Columns");
    public SerializedProperty Rows => m_OpeningData.FindPropertyRelative("m_Rows");
    public SerializedProperty Height => m_OpeningData.FindPropertyRelative("m_Height");
    public SerializedProperty Width => m_OpeningData.FindPropertyRelative("m_Width");

    public OpeningDataSerializedProperties(SerializedProperty data)
    {
        m_OpeningData = data;
        m_SerializedObject = data.serializedObject;
    }
}
