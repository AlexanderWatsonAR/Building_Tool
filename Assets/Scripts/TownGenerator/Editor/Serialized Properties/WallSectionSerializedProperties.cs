using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WallSectionSerializedProperties
{
    [SerializeField] SerializedObject m_SerializedObject;
    [SerializeField] private SerializedProperty m_WallSectionData;
    [SerializeField] WindowSerializedProperties m_WindowSerializedProperties;
    [SerializeField] DoorSerializedProperties m_DoorSerializedProperties;

    public WindowSerializedProperties WindowSerializedProperties => m_WindowSerializedProperties;
    public DoorSerializedProperties DoorSerializedProperties => m_DoorSerializedProperties;
    public SerializedObject SerializedObject => m_SerializedObject;

    #region Window
    public SerializedProperty Windows { get { return WallSectionData.FindPropertyRelative("m_Windows"); } }
    public SerializedProperty WindowColumns { get { return WallSectionData.FindPropertyRelative("m_WindowColumns"); } }
    public SerializedProperty WindowRows { get { return WallSectionData.FindPropertyRelative("m_WindowRows"); } }
    public SerializedProperty WindowHeight { get { return WallSectionData.FindPropertyRelative("m_WindowHeight"); } }
    public SerializedProperty WindowWidth { get { return WallSectionData.FindPropertyRelative("m_WindowWidth"); } }
    public SerializedProperty WindowSides { get { return WallSectionData.FindPropertyRelative("m_WindowSides"); } }
    public SerializedProperty WindowAngle { get { return WallSectionData.FindPropertyRelative("m_WindowAngle"); } }
    public SerializedProperty WindowData { get { return WallSectionData.FindPropertyRelative("m_WindowData"); } }
    public SerializedProperty WindowActiveElements { get { return WindowData.FindPropertyRelative("m_ActiveElements"); } }
    public SerializedProperty WindowInnerFrameColumns { get { return WindowData.FindPropertyRelative("m_InnerFrameColumns"); } }
    public SerializedProperty WindowInnerFrameRows { get { return WindowData.FindPropertyRelative("m_InnerFrameRows"); } }
    public SerializedProperty WindowInnerFrameScale { get { return WindowData.FindPropertyRelative("m_InnerFrameScale"); } }
    public SerializedProperty WindowInnerFrameDepth { get { return WindowData.FindPropertyRelative("m_InnerFrameDepth"); } }
    public SerializedProperty WindowOuterFrameScale { get { return WindowData.FindPropertyRelative("m_OuterFrameScale"); } }
    public SerializedProperty WindowOuterFrameDepth { get { return WindowData.FindPropertyRelative("m_OuterFrameDepth"); } }
    public SerializedProperty WindowPaneDepth { get { return WindowData.FindPropertyRelative("m_PaneDepth"); } }
    public SerializedProperty WindowShuttersDepth { get { return WindowData.FindPropertyRelative("m_ShuttersDepth"); } }
    public SerializedProperty WindowShuttersAngle { get { return WindowData.FindPropertyRelative("m_ShuttersAngle"); } }
    #endregion

    #region Door
    public SerializedProperty WallSectionData { get { return m_WallSectionData; } }
    public SerializedProperty WallElement { get { return WallSectionData.FindPropertyRelative("m_WallElement"); } }
    public SerializedProperty DoorwayElement { get { return WallSectionData.FindPropertyRelative("m_ActiveDoorwayElements"); } }
    public SerializedProperty DoorColumns { get { return WallSectionData.FindPropertyRelative("m_DoorColumns"); } }
    public SerializedProperty DoorRows { get { return WallSectionData.FindPropertyRelative("m_DoorRows"); } }
    public SerializedProperty DoorHeight { get { return WallSectionData.FindPropertyRelative("m_DoorPedimentHeight"); } }
    public SerializedProperty DoorWidth { get { return WallSectionData.FindPropertyRelative("m_DoorSideWidth"); } }
    public SerializedProperty DoorOffset { get { return WallSectionData.FindPropertyRelative("m_DoorSideOffset"); } }
    public SerializedProperty DoorData { get { return WallSectionData.FindPropertyRelative("m_DoorData"); } }
    public SerializedProperty DoorFrameDepth { get { return WallSectionData.FindPropertyRelative("m_DoorFrameDepth"); } }
    public SerializedProperty DoorFrameScale { get { return WallSectionData.FindPropertyRelative("m_DoorFrameInsideScale"); } }
    public SerializedProperty DoorScale { get { return DoorData.FindPropertyRelative("m_Scale"); } }
    public SerializedProperty DoorDepth { get { return DoorData.FindPropertyRelative("m_Depth"); } }
    public SerializedProperty DoorHingePoint { get { return DoorData.FindPropertyRelative("m_HingePoint"); } }
    public SerializedProperty DoorHingeOffset { get { return DoorData.FindPropertyRelative("m_HingeOffset"); } }
    public SerializedProperty DoorHingeEulerAngle { get { return DoorData.FindPropertyRelative("m_HingeEulerAngles"); } }
    public SerializedProperty DoorHandleScale { get { return DoorData.FindPropertyRelative("m_HandleScale"); } }
    public SerializedProperty DoorHandlePoint { get { return DoorData.FindPropertyRelative("m_HandlePoint"); } }
    public SerializedProperty Doors { get { return DoorData.FindPropertyRelative("m_Doors"); } }

    #endregion

    #region Arch
    public SerializedProperty ArchHeight { get { return WallSectionData.FindPropertyRelative("m_ArchHeight"); } }
    public SerializedProperty ArchSides { get { return WallSectionData.FindPropertyRelative("m_ArchSides"); } }
    public SerializedProperty ArchDoorElement { get { return WallSectionData.FindPropertyRelative("m_ActiveArchDoorElements"); } }
    public SerializedProperty ArchColumns { get { return WallSectionData.FindPropertyRelative("m_ArchColumns"); } }
    public SerializedProperty ArchRows { get { return WallSectionData.FindPropertyRelative("m_ArchRows"); } }
    public SerializedProperty ArchDoorHeight { get { return WallSectionData.FindPropertyRelative("m_ArchPedimentHeight"); } }
    public SerializedProperty ArchWidth { get { return WallSectionData.FindPropertyRelative("m_ArchSideWidth"); } }
    public SerializedProperty ArchOffset { get { return WallSectionData.FindPropertyRelative("m_ArchSideOffset"); } }
    public SerializedProperty ArchDoorData { get { return WallSectionData.FindPropertyRelative("m_ArchDoorData"); } }
    public SerializedProperty ArchDoorFrameDepth { get { return WallSectionData.FindPropertyRelative("m_ArchDoorFrameDepth"); } }
    public SerializedProperty ArchDoorFrameScale { get { return WallSectionData.FindPropertyRelative("m_ArchDoorFrameInsideScale"); } }
    public SerializedProperty ArchDoorScale { get { return ArchDoorData.FindPropertyRelative("m_Scale"); } }
    public SerializedProperty ArchDoorDepth { get { return ArchDoorData.FindPropertyRelative("m_Depth"); } }
    public SerializedProperty ArchDoorHingePoint { get { return ArchDoorData.FindPropertyRelative("m_HingePoint"); } }
    public SerializedProperty ArchDoorHingeOffset { get { return ArchDoorData.FindPropertyRelative("m_HingeOffset"); } }
    public SerializedProperty ArchDoorHingeEulerAngle { get { return ArchDoorData.FindPropertyRelative("m_HingeEulerAngles"); } }
    public SerializedProperty ArchDoorHandleScale { get { return ArchDoorData.FindPropertyRelative("m_HandleScale"); } }
    public SerializedProperty ArchDoorHandlePoint { get { return ArchDoorData.FindPropertyRelative("m_HandlePoint"); } }
    public SerializedProperty ArchDoors { get { return DoorData.FindPropertyRelative("m_ArchDoors"); } }

    #endregion

    #region Extension
    public SerializedProperty ExtensionHeight { get { return WallSectionData.FindPropertyRelative("m_ExtendHeight"); } }
    public SerializedProperty ExtensionWidth { get { return WallSectionData.FindPropertyRelative("m_ExtendWidth"); } }
    public SerializedProperty ExtensionDistance { get { return WallSectionData.FindPropertyRelative("m_ExtendDistance"); } }
    #endregion

    public WallSectionSerializedProperties(WallSection wallSection)
    {
        m_SerializedObject = new SerializedObject(wallSection);

        m_WallSectionData = m_SerializedObject.FindProperty("m_Data");

        m_WindowSerializedProperties = new WindowSerializedProperties(m_SerializedObject, WindowData);
        m_DoorSerializedProperties = new DoorSerializedProperties(m_SerializedObject, DoorData);

    }

    public WallSectionSerializedProperties(SerializedProperty data)
    {
        m_WallSectionData = data;
        m_SerializedObject = data.serializedObject;
        m_WindowSerializedProperties = new WindowSerializedProperties(m_SerializedObject, WindowData);
        m_DoorSerializedProperties = new DoorSerializedProperties(m_SerializedObject, DoorData);
    }

}
