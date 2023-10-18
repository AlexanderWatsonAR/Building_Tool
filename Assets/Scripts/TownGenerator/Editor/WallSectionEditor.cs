using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine.ProBuilder.Shapes;

[CustomEditor(typeof(WallSection))]
public class WallSectionEditor : Editor
{
    private bool m_IsArchFoldoutActive = true;
    private bool m_IsDoorFoldoutActive = true;
    private bool m_IsDoorFrameFoldoutActive = true;
    private bool m_IsWindowFoldoutActive = true;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        WallSection section = (WallSection)target;

        SerializedProperty element = serializedObject.FindProperty("m_WallElement");

        // Door
        SerializedProperty doorColumns = serializedObject.FindProperty("m_DoorColumns");
        SerializedProperty doorRows = serializedObject.FindProperty("m_DoorRows");
        SerializedProperty doorHeight = serializedObject.FindProperty("m_PedimentHeight");
        SerializedProperty doorWidth = serializedObject.FindProperty("m_SideWidth");
        SerializedProperty doorOffset = serializedObject.FindProperty("m_SideOffset");
        // SerializedProperty doorArched = serializedObject.FindProperty("m_IsDoorArched");
        SerializedProperty doorArchHeight = serializedObject.FindProperty("m_ArchHeight");
        SerializedProperty doorArchSides = serializedObject.FindProperty("m_ArchSides");

        SerializedProperty doorActive = serializedObject.FindProperty("m_IsDoorActive");
        SerializedProperty doorData = serializedObject.FindProperty("m_DoorData");
        SerializedProperty doorScale = doorData.FindPropertyRelative("m_Scale");
        SerializedProperty doorDepth = doorData.FindPropertyRelative("m_Depth");
        SerializedProperty doorHingePoint = doorData.FindPropertyRelative("m_HingePoint");
        SerializedProperty doorHingeOffset = doorData.FindPropertyRelative("m_HingeOffset");
        SerializedProperty doorHingeEulerAngles = doorData.FindPropertyRelative("m_HingeEulerAngles");
        SerializedProperty doorMaterial = doorData.FindPropertyRelative("m_Material");
        // Frame
        SerializedProperty doorFDepth = serializedObject.FindProperty("m_DoorFrameDepth");
        SerializedProperty doorFScale = serializedObject.FindProperty("m_DoorFrameInsideScale");
        SerializedProperty doorFMaterial = serializedObject.FindProperty("m_DoorFrameMaterial");

        // Handle
        SerializedProperty doorHScale = doorData.FindPropertyRelative("m_HandleScale");
        SerializedProperty doorHPoint = doorData.FindPropertyRelative("m_HandlePoint");
        // End Door

        // Window hole
        SerializedProperty winColumns = serializedObject.FindProperty("m_WindowColumns");
        SerializedProperty winRows = serializedObject.FindProperty("m_WindowRows");
        SerializedProperty winHeight = serializedObject.FindProperty("m_WindowHeight");
        SerializedProperty winWidth = serializedObject.FindProperty("m_WindowWidth");
        SerializedProperty winSides = serializedObject.FindProperty("m_WindowSides");
        SerializedProperty winAngles = serializedObject.FindProperty("m_WindowAngle");
        SerializedProperty winActive = serializedObject.FindProperty("m_IsWindowActive");
        // End Window hole

        // Window
        SerializedProperty winData = serializedObject.FindProperty("m_WindowData");

        WindowData windowData = section.WindowData;

        SerializedProperty activeElements = winData.FindPropertyRelative("m_ActiveElements");

        SerializedProperty winFColumns = winData.FindPropertyRelative("m_Columns");
        SerializedProperty winFRows = winData.FindPropertyRelative("m_Rows");
        SerializedProperty winFOuterScale = winData.FindPropertyRelative("m_OuterFrameScale");
        SerializedProperty winFInnerScale = winData.FindPropertyRelative("m_InnerFrameScale");
        SerializedProperty winFOuterFrameDepth = winData.FindPropertyRelative("m_OuterFrameDepth");
        SerializedProperty winFInnerFrameDepth = winData.FindPropertyRelative("m_InnerFrameDepth");
        SerializedProperty winFPaneDepth = winData.FindPropertyRelative("m_PaneDepth");
        SerializedProperty winFShuttersDepth = winData.FindPropertyRelative("m_ShuttersDepth");
        SerializedProperty winFShuttersAngle = winData.FindPropertyRelative("m_ShuttersAngle");
        SerializedProperty winOuterFrameMat = winData.FindPropertyRelative("m_OuterFrameMaterial");
        SerializedProperty winInnerFrameMat = winData.FindPropertyRelative("m_InnerFrameMaterial");
        SerializedProperty winPaneMat = winData.FindPropertyRelative("m_PaneMaterial");
        SerializedProperty winShuttersMat = winData.FindPropertyRelative("m_ShuttersMaterial");

        // End Window

        // Extension
        SerializedProperty exHeight = serializedObject.FindProperty("m_ExtendHeight");
        SerializedProperty exWidth = serializedObject.FindProperty("m_ExtendWidth");
        SerializedProperty exDistance = serializedObject.FindProperty("m_ExtendDistance");
        // End Extension

        EditorGUILayout.PropertyField(element, new GUIContent("Section"));

        switch (section.WallElement)
        {
            case WallElement.Wall:
                break;
            case WallElement.Doorway:
                {
                    EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.IntSlider(doorColumns, 1, 5, "Columns");
                    EditorGUILayout.IntSlider(doorRows, 1, 5, "Rows");
                    EditorGUI.indentLevel--;
                    EditorGUILayout.LabelField("Size");
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(doorOffset);
                    EditorGUILayout.PropertyField(doorHeight);
                    EditorGUILayout.PropertyField(doorWidth);
                    EditorGUI.indentLevel--;

                    //doorArched.boolValue = EditorGUILayout.Toggle("Is Arched", doorArched.boolValue);
                    m_IsArchFoldoutActive = EditorGUILayout.BeginFoldoutHeaderGroup(m_IsArchFoldoutActive, "Arch");

                    if (m_IsArchFoldoutActive)
                    {
                        //EditorGUI.BeginDisabledGroup(!doorArched.boolValue);
                        doorArchHeight.floatValue = EditorGUILayout.Slider("Height", doorArchHeight.floatValue, 0, 1);
                        doorArchSides.intValue = EditorGUILayout.IntSlider("Sides", doorArchSides.intValue, 3, 16);
                        //EditorGUI.EndDisabledGroup();
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();

                    doorActive.boolValue = EditorGUILayout.Toggle("Is Active", doorActive.boolValue);

                    m_IsDoorFoldoutActive = EditorGUILayout.BeginFoldoutHeaderGroup(m_IsDoorFoldoutActive, "Door");

                    if (m_IsDoorFoldoutActive)
                    {
                        EditorGUI.BeginDisabledGroup(!doorActive.boolValue);

                        EditorGUILayout.PropertyField(doorScale);
                        EditorGUILayout.PropertyField(doorDepth);
                        //EditorGUILayout.ObjectField(doorMaterial, new GUIContent("Material"));

                        EditorGUILayout.LabelField("Hinge", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        doorHingePoint.SetEnumValue((TransformPoint)EditorGUILayout.EnumPopup("Position", doorHingePoint.GetEnumValue<TransformPoint>()));
                        doorHingeOffset.vector3Value = EditorGUILayout.Vector3Field("Offset", doorHingeOffset.vector3Value);
                        doorHingeEulerAngles.vector3Value = EditorGUILayout.Vector3Field("Rotation", doorHingeEulerAngles.vector3Value);
                        EditorGUI.indentLevel--;
                        EditorGUI.EndDisabledGroup();
                    }

                    EditorGUILayout.EndFoldoutHeaderGroup();

                    m_IsDoorFrameFoldoutActive = EditorGUILayout.BeginFoldoutHeaderGroup(m_IsDoorFoldoutActive, "Frame");

                    if (m_IsDoorFrameFoldoutActive)
                    {
                        EditorGUI.BeginDisabledGroup(!doorActive.boolValue);

                        EditorGUILayout.Slider(doorFDepth, 0, 0.999f, "Depth");
                        EditorGUILayout.Slider(doorFScale, 0, 0.999f, "Scale");
                        //EditorGUILayout.ObjectField(doorFMaterial, new GUIContent("Material"));

                        EditorGUI.EndDisabledGroup();
                    }

                    EditorGUILayout.LabelField("Handle", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(doorHScale, new GUIContent("Scale"));
                    EditorGUILayout.PropertyField(doorHPoint, new GUIContent("Position"));


                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
                break;
            case WallElement.Window:
                {
                    EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.IntSlider(winColumns, 1, 5, "Columns");
                    EditorGUILayout.IntSlider(winRows, 1, 5, "Rows");
                    EditorGUI.indentLevel--;
                    EditorGUILayout.LabelField("Shape", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.IntSlider(winSides, 3, 16, "Sides");
                    EditorGUILayout.Slider(winHeight, 0, 0.999f, "Height");
                    EditorGUILayout.Slider(winWidth, 0, 0.999f, "Width");
                    EditorGUILayout.Slider(winAngles, -180, 180, "Angle");
                    EditorGUI.indentLevel--;

                    m_IsWindowFoldoutActive = EditorGUILayout.BeginFoldoutHeaderGroup(m_IsWindowFoldoutActive, "Window");

                    if (m_IsWindowFoldoutActive)
                    {
                        EditorGUILayout.PropertyField(activeElements);

                        EditorGUI.BeginDisabledGroup(!windowData.IsOuterFrameActive);
                        EditorGUILayout.LabelField("Outer Frame", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(winFOuterScale, new GUIContent("Scale"));
                        EditorGUILayout.PropertyField(winFOuterFrameDepth, new GUIContent("Depth"));
                        //EditorGUILayout.PropertyField(winOuterFrameMat, new GUIContent("Material"));
                        EditorGUI.indentLevel--;
                        EditorGUI.EndDisabledGroup();

                        EditorGUI.BeginDisabledGroup(!windowData.IsInnerFrameActive);
                        EditorGUILayout.LabelField("Inner Frame", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(winFColumns, new GUIContent("Columns"));
                        EditorGUILayout.PropertyField(winFRows, new GUIContent("Rows"));
                        EditorGUILayout.PropertyField(winFInnerScale, new GUIContent("Scale"));
                        EditorGUILayout.PropertyField(winFInnerFrameDepth, new GUIContent("Depth"));
                        //EditorGUILayout.PropertyField(winInnerFrameMat, new GUIContent("Material"));
                        EditorGUI.indentLevel--;
                        EditorGUI.EndDisabledGroup();

                        EditorGUI.BeginDisabledGroup(!windowData.IsPaneActive);
                        EditorGUILayout.LabelField("Pane", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(winFPaneDepth);
                        //EditorGUILayout.PropertyField(winPaneMat, new GUIContent("Material"));
                        EditorGUI.indentLevel--;
                        EditorGUI.EndDisabledGroup();

                        EditorGUI.BeginDisabledGroup(!windowData.AreShuttersActive);
                        EditorGUILayout.LabelField("Shutters", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(winFShuttersDepth, new GUIContent("Depth"));
                        EditorGUILayout.PropertyField(winFShuttersAngle, new GUIContent("Angle"));
                        //EditorGUILayout.PropertyField(winShuttersMat, new GUIContent("Material"));
                        EditorGUI.indentLevel--;
                        EditorGUI.EndDisabledGroup();
                    }

                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
                break;
            case WallElement.Extension:
                EditorGUILayout.PropertyField(exWidth);
                EditorGUILayout.PropertyField(exHeight);
                EditorGUILayout.PropertyField(exDistance);
                break;
        }


        if (serializedObject.ApplyModifiedProperties())
        {
            section.Build();
        }
    }
}
