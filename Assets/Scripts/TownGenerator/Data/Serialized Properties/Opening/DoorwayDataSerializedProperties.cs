using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DoorwayDataSerializedProperties : OpeningDataSerializedProperties
{
    #region Constants
    const string k_Doors = "m_Doors";
    const string k_Frames = "m_Frames";
    const string k_ActiveElements = "m_ActiveElements";
    const string k_PositionOffset = "m_PositionOffset";
    #endregion

    #region Accessors
    public SerializedProperty Doors => m_Data.FindPropertyRelative(k_Doors);
    public SerializedProperty ActiveElements => m_Data.FindPropertyRelative(k_ActiveElements);
    public SerializedProperty Frames => m_Data.FindPropertyRelative(k_Frames);
    public SerializedProperty PositionOffset => m_Data.FindPropertyRelative(k_PositionOffset);
    #endregion

    public DoorwayDataSerializedProperties(SerializedProperty doorwayData) : base(doorwayData)
    {

    }

}
