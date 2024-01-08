using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DoorDataSerializedProperties
{
    [SerializeField] private SerializedObject m_SerializedObject;
    [SerializeField] private SerializedProperty m_Data;

    public SerializedObject SerializedObject => m_SerializedObject;
    public SerializedProperty ActiveElements => m_Data.FindPropertyRelative("m_ActiveElements");
    public SerializedProperty Depth => m_Data.FindPropertyRelative("m_Depth");
    public SerializedProperty Scale => m_Data.FindPropertyRelative("m_Scale"); 
    public SerializedProperty HingeOffset => m_Data.FindPropertyRelative("m_HingeOffset");
    public SerializedProperty HingeEulerAngle => m_Data.FindPropertyRelative("m_HingeEulerAngles");
    public SerializedProperty HingePoint => m_Data.FindPropertyRelative("m_HingePoint");
    public SerializedProperty HandlePoint => m_Data.FindPropertyRelative("m_HandlePoint");
    public SerializedProperty HandlePosition => m_Data.FindPropertyRelative("m_HandlePosition");
    public SerializedProperty HandleScale => m_Data.FindPropertyRelative("m_HandleScale");

    public DoorDataSerializedProperties(SerializedProperty doorData)
    {
        m_Data = doorData;
        m_SerializedObject = doorData.serializedObject;
    }

}
