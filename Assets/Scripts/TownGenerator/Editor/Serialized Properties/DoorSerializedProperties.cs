using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DoorSerializedProperties
{
    [SerializeField] private SerializedObject m_SerializedObject;
    [SerializeField] private SerializedProperty m_Data;

    public SerializedProperty ActiveElements { get { return m_Data.FindPropertyRelative("m_ActiveElements"); } }
    public SerializedProperty Scale { get { return m_Data.FindPropertyRelative("m_Scale"); } }
    public SerializedProperty HingeOffset { get { return m_Data.FindPropertyRelative("m_HingeOffset"); } }
    public SerializedProperty HingeEulerAngle { get { return m_Data.FindPropertyRelative("m_HingeEulerAngles"); } }
    public SerializedProperty HingePoint { get { return m_Data.FindPropertyRelative("m_HingePoint"); } }

    public DoorSerializedProperties(Door door)
    {
        m_SerializedObject = new SerializedObject(door);

        m_Data = m_SerializedObject.FindProperty("m_Data");
    }

    public DoorSerializedProperties(SerializedObject serializedObject, SerializedProperty doorData)
    {
        m_SerializedObject = serializedObject;
        m_Data = doorData;
    }

}
