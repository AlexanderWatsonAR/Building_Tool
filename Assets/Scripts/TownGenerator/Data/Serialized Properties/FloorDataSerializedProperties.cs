using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FloorDataSerializedProperties
{
    [SerializeField] private SerializedProperty m_FloorData;
    [SerializeField] private SerializedObject m_SerializedObject;

    #region Accessors
    public SerializedObject SerializedObject => m_SerializedObject;
    public SerializedProperty Height => m_FloorData.FindPropertyRelative("m_Height");
    #endregion

    public FloorDataSerializedProperties(SerializedProperty floorData)
    {
        m_FloorData = floorData;
        m_SerializedObject = floorData.serializedObject;
    }
}
