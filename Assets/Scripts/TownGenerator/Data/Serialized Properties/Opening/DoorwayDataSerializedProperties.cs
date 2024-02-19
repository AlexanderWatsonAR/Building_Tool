using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DoorwayDataSerializedProperties : OpeningDataSerializedProperties
{
    #region Constants
    const string k_Doors = "m_Doors";
    const string k_ActiveElements = "m_ActiveElements";
    const string k_FrameDepth = "m_FrameDepth";
    const string k_FrameScale = "m_FrameScale";
    const string k_PositionOffset = "m_PositionOffset";
    #endregion

    #region Accessors
    public SerializedProperty Doors => m_Data.FindPropertyRelative(k_Doors);
    public SerializedProperty ActiveElements => m_Data.FindPropertyRelative(k_ActiveElements);
    public SerializedProperty FrameDepth => m_Data.FindPropertyRelative(k_FrameDepth);
    public SerializedProperty FrameScale => m_Data.FindPropertyRelative(k_FrameScale);
    public SerializedProperty PositionOffset => m_Data.FindPropertyRelative(k_PositionOffset);
    #endregion

    public DoorwayDataSerializedProperties(SerializedProperty doorwayData) : base(doorwayData)
    {

    }

}
