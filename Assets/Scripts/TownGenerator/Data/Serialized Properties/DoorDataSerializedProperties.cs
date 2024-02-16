using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DoorDataSerializedProperties
{
    [SerializeField] SerializedObject m_SerializedObject;
    [SerializeField] SerializedProperty m_Data;

    #region Constants
    const string k_ActiveElements = "m_ActiveElements";
    const string k_Depth = "m_Depth";
    const string k_Scale = "m_Scale";
    const string k_HingeOffset = "m_HingeOffset";
    const string k_HingeEulerAngles = "m_HingeEulerAngles";
    const string k_HingePoint = "m_HingePoint";
    const string k_HandlePoint = "m_HandlePoint";
    const string k_HandlePosition = "m_HandlePosition";
    const string k_HandleScale = "m_HandleScale";
    #endregion

    #region Accessors
    public SerializedObject SerializedObject => m_SerializedObject;
    public SerializedProperty Data => m_Data;
    public SerializedProperty ActiveElements => m_Data.FindPropertyRelative(k_ActiveElements);
    public SerializedProperty Depth => m_Data.FindPropertyRelative(k_Depth);
    public SerializedProperty Scale => m_Data.FindPropertyRelative(k_Scale); 
    public SerializedProperty HingeOffset => m_Data.FindPropertyRelative(k_HingeOffset);
    public SerializedProperty HingeEulerAngle => m_Data.FindPropertyRelative(k_HingeEulerAngles);
    public SerializedProperty HingePoint => m_Data.FindPropertyRelative(k_HingePoint);
    public SerializedProperty HandlePoint => m_Data.FindPropertyRelative(k_HandlePoint);
    public SerializedProperty HandlePosition => m_Data.FindPropertyRelative(k_HandlePosition);
    public SerializedProperty HandleScale => m_Data.FindPropertyRelative(k_HandleScale);
    #endregion

    public DoorDataSerializedProperties(SerializedProperty doorData)
    {
        m_Data = doorData;
        m_SerializedObject = doorData.serializedObject;
    }

}
