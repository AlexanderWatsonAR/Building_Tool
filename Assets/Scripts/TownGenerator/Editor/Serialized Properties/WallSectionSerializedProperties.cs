using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WallSectionSerializedProperties
{
    [SerializeField] SerializedObject m_SerializedObject;
    [SerializeField] WindowSerializedProperties m_WindowSerializedProperties;

    public WindowSerializedProperties WindowSerializedProperties => m_WindowSerializedProperties;
    public SerializedObject SerializedObject => m_SerializedObject;
    public SerializedProperty WallElement { get { return m_SerializedObject.FindProperty("m_WallElement"); } }
    public SerializedProperty DoorColumns { get { return m_SerializedObject.FindProperty("m_DoorColumns"); } }
    public SerializedProperty DoorRows { get { return m_SerializedObject.FindProperty("m_DoorRows"); } }
    public SerializedProperty DoorHeight { get { return m_SerializedObject.FindProperty("m_PedimentHeight"); } }
    public SerializedProperty DoorWidth { get { return m_SerializedObject.FindProperty("m_SideWidth"); } }
    public SerializedProperty DoorOffset { get { return m_SerializedObject.FindProperty("m_SideOffset"); } }
    public SerializedProperty DoorArchHeight { get { return m_SerializedObject.FindProperty("m_DoorArchHeight"); } }
    public SerializedProperty DoorArchSides { get { return m_SerializedObject.FindProperty("m_DoorArchSides"); } }
    public SerializedProperty DoorActive { get { return m_SerializedObject.FindProperty("m_IsDoorActive"); } }
    public SerializedProperty DoorData { get { return m_SerializedObject.FindProperty("m_DoorData"); } }
    public SerializedProperty DoorScale { get { return DoorData.FindPropertyRelative("m_Scale"); } }
    public SerializedProperty DoorDepth { get { return DoorData.FindPropertyRelative("m_Depth"); } }
    public SerializedProperty DoorHingePoint { get { return DoorData.FindPropertyRelative("m_HingePoint"); } }
    public SerializedProperty DoorHingeOffset { get { return DoorData.FindPropertyRelative("m_HingeOffset"); } }
    public SerializedProperty DoorHingeEulerAngle { get { return DoorData.FindPropertyRelative("m_HingeEulerAngles"); } }
    public SerializedProperty DoorFrameDepth { get { return m_SerializedObject.FindProperty("m_DoorFrameDepth"); } }
    public SerializedProperty DoorFrameScale { get { return m_SerializedObject.FindProperty("m_DoorFrameInsideScale"); } }
    public SerializedProperty DoorHandleScale { get { return DoorData.FindPropertyRelative("m_HandleScale"); } }
    public SerializedProperty DoorHandlePoint { get { return DoorData.FindPropertyRelative("m_HandlePoint"); } }
    public SerializedProperty WindowColumns { get { return m_SerializedObject.FindProperty("m_WindowColumns"); } }
    public SerializedProperty WindowRows { get { return m_SerializedObject.FindProperty("m_WindowRows"); } }
    public SerializedProperty WindowHeight { get { return m_SerializedObject.FindProperty("m_WindowHeight"); } }
    public SerializedProperty WindowWidth { get { return m_SerializedObject.FindProperty("m_WindowWidth"); } }
    public SerializedProperty WindowSides { get { return m_SerializedObject.FindProperty("m_WindowSides"); } }
    public SerializedProperty WindowAngle { get { return m_SerializedObject.FindProperty("m_WindowAngle"); } }
    public SerializedProperty WindowData { get { return m_SerializedObject.FindProperty("m_WindowData"); } }
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
    public SerializedProperty ExtensionHeight { get { return m_SerializedObject.FindProperty("m_ExtendHeight"); } }
    public SerializedProperty ExtensionWidth { get { return m_SerializedObject.FindProperty("m_ExtendWidth"); } }
    public SerializedProperty ExtensionDistance { get { return m_SerializedObject.FindProperty("m_ExtendDistance"); } }

    public WallSectionSerializedProperties(WallSection wallSection)
    {
        m_SerializedObject = new SerializedObject(wallSection);

        m_WindowSerializedProperties = new WindowSerializedProperties(m_SerializedObject, WindowData);

    }

}
