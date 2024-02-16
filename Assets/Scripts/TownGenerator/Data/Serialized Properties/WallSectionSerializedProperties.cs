using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public class WallSectionDataSerializedProperties
{
    #region Member Variables
    [SerializeField] SerializedObject m_SerializedObject;
    [SerializeField] SerializedProperty m_WallSectionData;

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
    public SerializedObject SerializedObject => m_SerializedObject;
    public SerializedProperty WallSectionData => m_WallSectionData;
    public SerializedProperty WallElement => WallSectionData.FindPropertyRelative(k_WallElement);
    public DoorDataSerializedProperties Door => m_DoorSerializedProperties;
    public DoorDataSerializedProperties ArchDoor => m_ArchDoorSerializedProperties;
    public WindowDataSerializedProperties Window => m_WindowSerializedProperties;
    public WindowOpeningDataSerializedProperties WindowOpening => m_WindowOpeningDataSerializedProperties;
    public DoorwayDataSerializedProperties Doorway => m_DoorwayDataSerializedProperties;
    public ArchwayDataSerializedProperties Archway => m_ArchwayDataSerializedProperties;
    public ExtensionDataSerializedProperties Extension => m_ExtensionDataSerializedProperties;
    #endregion

    public WallSectionDataSerializedProperties(SerializedProperty data)
    {
        m_WallSectionData = data;
        m_SerializedObject = data.serializedObject;

        m_WindowSerializedProperties = new WindowDataSerializedProperties(data.FindPropertyRelative(k_WindowData));
        m_DoorSerializedProperties = new DoorDataSerializedProperties(data.FindPropertyRelative(k_DoorData));
        m_ArchDoorSerializedProperties = new DoorDataSerializedProperties(data.FindPropertyRelative(k_ArchDoorData));

        m_WindowOpeningDataSerializedProperties = new WindowOpeningDataSerializedProperties(data.FindPropertyRelative(k_WindowOpeningData));
        m_DoorwayDataSerializedProperties = new DoorwayDataSerializedProperties(data.FindPropertyRelative(k_DoorwayData));
        m_ArchwayDataSerializedProperties = new ArchwayDataSerializedProperties(data.FindPropertyRelative(k_ArchwayData));
        m_ExtensionDataSerializedProperties = new ExtensionDataSerializedProperties(data.FindPropertyRelative(k_ExtensionData));
    }

}
