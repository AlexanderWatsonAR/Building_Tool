using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ArchwayDataSerializedProperties : DoorwayDataSerializedProperties
{
    public SerializedProperty ArchHeight => m_OpeningData.FindPropertyRelative("m_ArchHeight");
    public SerializedProperty ArchSides => m_OpeningData.FindPropertyRelative("m_ArchSides");

    public ArchwayDataSerializedProperties(SerializedProperty data) : base(data)
    {

    }
}
