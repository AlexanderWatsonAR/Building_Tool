using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DoorwayDataSerializedProperties : OpeningDataSerializedProperties
{
    public SerializedProperty Doors => m_OpeningData.FindPropertyRelative("m_Doors");
    public SerializedProperty ActiveElements => m_OpeningData.FindPropertyRelative("m_ActiveElements");
    public SerializedProperty FrameDepth => m_OpeningData.FindPropertyRelative("m_FrameDepth");
    public SerializedProperty FrameScale => m_OpeningData.FindPropertyRelative("m_FrameScale");
    public SerializedProperty PositionOffset => m_OpeningData.FindPropertyRelative("m_PositionOffset");

    public DoorwayDataSerializedProperties(SerializedProperty data) : base(data)
    {

    }

}
