using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DoorSerializedProperties
{
    [SerializeField] private SerializedObject m_SerializedObject;
    [SerializeField] private SerializedProperty m_DataProp;
    [SerializeField] private DoorData m_Data;

    //public DoorData Data => m_Data;
    public SerializedProperty Scale { get { return m_DataProp.FindPropertyRelative("m_Scale"); } }
    public SerializedProperty HingeOffset { get { return m_DataProp.FindPropertyRelative("m_HingeOffset"); } }
    public SerializedProperty HingeEulerAngle { get { return m_DataProp.FindPropertyRelative("m_HingeEulerAngles"); } }
    public SerializedProperty HingePoint { get { return m_DataProp.FindPropertyRelative("m_HingePoint"); } }

    public DoorSerializedProperties(Door door)
    {
        m_SerializedObject = new SerializedObject(door);
        m_Data = door.DoorData;

        m_DataProp = m_SerializedObject.FindProperty("m_Data");
    }

    public DoorSerializedProperties(SerializedObject serializedObject, SerializedProperty doorData)
    {
        m_SerializedObject = serializedObject;
        m_DataProp = doorData;
    }

}
