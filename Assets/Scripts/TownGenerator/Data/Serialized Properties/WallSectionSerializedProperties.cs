using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WallSectionDataSerializedProperties : SerializedPropertyGroup
{
    #region Member Variables
    readonly WindowOpeningDataSerializedProperties m_WindowOpeningDataSerializedProperties;
    readonly ArchwayDataSerializedProperties m_ArchwayDataSerializedProperties;
    readonly DoorwayDataSerializedProperties m_DoorwayDataSerializedProperties;
    readonly ExtensionDataSerializedProperties m_ExtensionDataSerializedProperties;

    readonly WindowDataSerializedProperties m_WindowSerializedProperties;
    readonly DoorDataSerializedProperties m_DoorSerializedProperties;
    readonly DoorDataSerializedProperties m_ArchDoorSerializedProperties;
    readonly FrameDataSerializedProperties m_FrameDataSerializedProperties;
    #endregion

    #region Constants
    const string k_WallElement = "m_WallElement";
    const string k_WindowData = "m_Window";
    const string k_DoorData = "m_Door";
    const string k_ArchDoorData = "m_ArchDoor";
    const string k_WindowOpeningData = "m_WindowOpening";
    const string k_DoorwayData = "m_Doorway";
    const string k_ArchwayData = "m_Archway";
    const string k_ExtensionData = "m_Extension";
    const string k_DoorFrameData = "m_DoorFrame";
    #endregion

    #region Accessors
    public SerializedProperty WallElement => m_Data.FindPropertyRelative(k_WallElement);
    public DoorDataSerializedProperties Door => m_DoorSerializedProperties;
    public DoorDataSerializedProperties ArchDoor => m_ArchDoorSerializedProperties;
    public WindowDataSerializedProperties Window => m_WindowSerializedProperties;
    public WindowOpeningDataSerializedProperties WindowOpening => m_WindowOpeningDataSerializedProperties;
    public DoorwayDataSerializedProperties Doorway => m_DoorwayDataSerializedProperties;
    public ArchwayDataSerializedProperties Archway => m_ArchwayDataSerializedProperties;
    public ExtensionDataSerializedProperties Extension => m_ExtensionDataSerializedProperties;
    public FrameDataSerializedProperties Frame => m_FrameDataSerializedProperties;
    #endregion

    public WallSectionDataSerializedProperties(SerializedProperty wallSectionData) : base(wallSectionData)
    {
        m_WindowSerializedProperties = new WindowDataSerializedProperties(m_Data.FindPropertyRelative(k_WindowData));
        m_DoorSerializedProperties = new DoorDataSerializedProperties(m_Data.FindPropertyRelative(k_DoorData));
        m_ArchDoorSerializedProperties = new DoorDataSerializedProperties(m_Data.FindPropertyRelative(k_ArchDoorData));

        m_WindowOpeningDataSerializedProperties = new WindowOpeningDataSerializedProperties(m_Data.FindPropertyRelative(k_WindowOpeningData));
        m_DoorwayDataSerializedProperties = new DoorwayDataSerializedProperties(m_Data.FindPropertyRelative(k_DoorwayData));
        m_ArchwayDataSerializedProperties = new ArchwayDataSerializedProperties(m_Data.FindPropertyRelative(k_ArchwayData));
        m_ExtensionDataSerializedProperties = new ExtensionDataSerializedProperties(m_Data.FindPropertyRelative(k_ExtensionData));
        m_FrameDataSerializedProperties = new FrameDataSerializedProperties(m_Data.FindPropertyRelative(k_DoorFrameData));
    }

}
