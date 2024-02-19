using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WallSectionDataSerializedProperties : SerializedPropertyGroup
{
    #region Member Variables
    [SerializeField] WindowOpeningDataSerializedProperties m_WindowOpeningDataSerializedProperties;
    [SerializeField] ArchwayDataSerializedProperties m_ArchwayDataSerializedProperties;
    [SerializeField] DoorwayDataSerializedProperties m_DoorwayDataSerializedProperties;
    [SerializeField] ExtensionDataSerializedProperties m_ExtensionDataSerializedProperties;

    [SerializeField] WindowDataSerializedProperties m_WindowSerializedProperties;
    [SerializeField] DoorDataSerializedProperties m_DoorSerializedProperties;
    [SerializeField] DoorDataSerializedProperties m_ArchDoorSerializedProperties;
    #endregion

    #region Constants
    const string k_WallElement = "m_WallElement";
    const string k_WindowData = "m_WindowData";
    const string k_DoorData = "m_DoorData";
    const string k_ArchDoorData = "m_ArchDoorData";
    const string k_WindowOpeningData = "m_WindowOpeningData";
    const string k_DoorwayData = "m_DoorwayData";
    const string k_ArchwayData = "m_ArchwayData";
    const string k_ExtensionData = "m_ExtensionData";
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
    }

}
