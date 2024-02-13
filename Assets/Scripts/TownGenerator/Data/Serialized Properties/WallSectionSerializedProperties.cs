using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public class WallSectionDataSerializedProperties
{
    [SerializeField] SerializedObject m_SerializedObject;
    [SerializeField] SerializedProperty m_WallSectionData;
    [SerializeField] WindowDataSerializedProperties m_WindowSerializedProperties;
    [SerializeField] DoorDataSerializedProperties m_DoorSerializedProperties;
    [SerializeField] DoorDataSerializedProperties m_ArchDoorSerializedProperties;

    #region Accessors
    public WindowDataSerializedProperties Window => m_WindowSerializedProperties;
    public DoorDataSerializedProperties Door => m_DoorSerializedProperties;
    public DoorDataSerializedProperties ArchDoor => m_ArchDoorSerializedProperties;
    public SerializedObject SerializedObject => m_SerializedObject;

    public SerializedProperty WallSectionData => m_WallSectionData;
    public SerializedProperty WallElement => WallSectionData.FindPropertyRelative("m_WallElement");

    #region Window
    public SerializedProperty Windows => WallSectionData.FindPropertyRelative("m_Windows");
    public SerializedProperty WindowColumns => WallSectionData.FindPropertyRelative("m_WindowColumns");
    public SerializedProperty WindowRows => WallSectionData.FindPropertyRelative("m_WindowRows");
    public SerializedProperty WindowHeight => WallSectionData.FindPropertyRelative("m_WindowHeight");
    public SerializedProperty WindowWidth => WallSectionData.FindPropertyRelative("m_WindowWidth");
    public SerializedProperty WindowSides => WallSectionData.FindPropertyRelative("m_WindowSides");
    public SerializedProperty WindowAngle => WallSectionData.FindPropertyRelative("m_WindowAngle");
    public SerializedProperty WindowData => WallSectionData.FindPropertyRelative("m_WindowData");
    public SerializedProperty WindowActiveElements => Window.ActiveElements;
    public SerializedProperty WindowInnerFrameColumns => Window.InnerFrameColumns;
    public SerializedProperty WindowInnerFrameRows => Window.InnerFrameRows;
    public SerializedProperty WindowInnerFrameScale => Window.InnerFrameScale;
    public SerializedProperty WindowInnerFrameDepth => Window.InnerFrameDepth;
    public SerializedProperty WindowOuterFrameScale => Window.OuterFrameScale;
    public SerializedProperty WindowOuterFrameDepth => Window.OuterFrameDepth;
    public SerializedProperty WindowPaneDepth => Window.PaneDepth;
    public SerializedProperty WindowShuttersDepth => Window.ShuttersDepth;
    public SerializedProperty WindowShuttersAngle => Window.ShuttersAngle;
    #endregion

    #region Door
    public SerializedProperty Doors => WallSectionData.FindPropertyRelative("m_Doors");
    public SerializedProperty DoorwayElement => WallSectionData.FindPropertyRelative("m_ActiveDoorwayElements");
    public SerializedProperty DoorColumns => WallSectionData.FindPropertyRelative("m_DoorColumns");
    public SerializedProperty DoorRows => WallSectionData.FindPropertyRelative("m_DoorRows"); 
    public SerializedProperty DoorHeight => WallSectionData.FindPropertyRelative("m_DoorPedimentHeight");
    public SerializedProperty DoorWidth => WallSectionData.FindPropertyRelative("m_DoorSideWidth");
    public SerializedProperty DoorOffset => WallSectionData.FindPropertyRelative("m_DoorSideOffset");
    public SerializedProperty DoorData => WallSectionData.FindPropertyRelative("m_DoorData");
    public SerializedProperty DoorFrameDepth => WallSectionData.FindPropertyRelative("m_DoorFrameDepth");
    public SerializedProperty DoorFrameScale => WallSectionData.FindPropertyRelative("m_DoorFrameInsideScale");
    public SerializedProperty DoorScale => Door.Scale;
    public SerializedProperty DoorDepth => Door.Depth;
    public SerializedProperty DoorHingePoint => Door.HingePoint;
    public SerializedProperty DoorHingeOffset => Door.HingeOffset;
    public SerializedProperty DoorHingeEulerAngle => Door.HingeEulerAngle;
    public SerializedProperty DoorHandleScale => Door.HandleScale;
    public SerializedProperty DoorHandlePoint => Door.HandlePoint;
    public SerializedProperty DoorHandlePosition => Door.HandlePosition;


    #endregion

    #region Arch
    public SerializedProperty ArchDoors => WallSectionData.FindPropertyRelative("m_ArchDoors");
    public SerializedProperty ArchHeight => WallSectionData.FindPropertyRelative("m_ArchHeight");
    public SerializedProperty ArchSides => WallSectionData.FindPropertyRelative("m_ArchSides");
    public SerializedProperty ArchwayElement => WallSectionData.FindPropertyRelative("m_ActiveArchDoorElements");
    public SerializedProperty ArchColumns => WallSectionData.FindPropertyRelative("m_ArchColumns");
    public SerializedProperty ArchRows => WallSectionData.FindPropertyRelative("m_ArchRows");
    public SerializedProperty ArchDoorHeight => WallSectionData.FindPropertyRelative("m_ArchPedimentHeight");
    public SerializedProperty ArchWidth => WallSectionData.FindPropertyRelative("m_ArchSideWidth");
    public SerializedProperty ArchOffset => WallSectionData.FindPropertyRelative("m_ArchSideOffset");
    public SerializedProperty ArchDoorData => WallSectionData.FindPropertyRelative("m_ArchDoorData");
    public SerializedProperty ArchDoorFrameDepth => WallSectionData.FindPropertyRelative("m_ArchDoorFrameDepth");
    public SerializedProperty ArchDoorFrameScale => WallSectionData.FindPropertyRelative("m_ArchDoorFrameInsideScale");
    public SerializedProperty ArchDoorScale => ArchDoor.Scale;
    public SerializedProperty ArchDoorDepth => ArchDoor.Depth;
    public SerializedProperty ArchDoorHingePoint => ArchDoor.HingePoint;
    public SerializedProperty ArchDoorHingeOffset => ArchDoor.HingeOffset;
    public SerializedProperty ArchDoorHingeEulerAngle => ArchDoor.HingeEulerAngle;
    public SerializedProperty ArchDoorHandleScale => ArchDoor.HandleScale;
    public SerializedProperty ArchDoorHandlePoint => ArchDoor.HandlePoint;
    public SerializedProperty ArchDoorHandlePosition => ArchDoor.HandlePosition;

    #endregion

    #region Extension
    public SerializedProperty ExtensionHeight => WallSectionData.FindPropertyRelative("m_ExtensionHeight");
    public SerializedProperty ExtensionWidth => WallSectionData.FindPropertyRelative("m_ExtensionWidth");
    public SerializedProperty ExtensionDistance => WallSectionData.FindPropertyRelative("m_ExtensionDistance");
    public SerializedProperty ExtensionStorey => WallSectionData.FindPropertyRelative("m_ExtensionStoreyData");
    public SerializedProperty ExtensionRoof => WallSectionData.FindPropertyRelative("m_ExtensionRoofData");
    #endregion
    #endregion

    public WallSectionDataSerializedProperties(SerializedProperty data)
    {
        m_WallSectionData = data;
        m_SerializedObject = data.serializedObject;
        m_WindowSerializedProperties = new WindowDataSerializedProperties(data.FindPropertyRelative("m_WindowData"));
        m_DoorSerializedProperties = new DoorDataSerializedProperties(data.FindPropertyRelative("m_DoorData"));
        m_ArchDoorSerializedProperties = new DoorDataSerializedProperties(data.FindPropertyRelative("m_ArchDoorData"));
    }

}
