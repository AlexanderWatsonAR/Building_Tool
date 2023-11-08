using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine.ProBuilder.Shapes;
using UnityEditor.Overlays;
using UnityEngine.UIElements;

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

        WallSectionSerializedProperties serialProps = new WallSectionSerializedProperties(section);

        WindowData windowData = section.Data.WindowData;

        EditorGUILayout.PropertyField(serialProps.WallElement, new GUIContent("Section"));

        switch (section.WallElement)
        {
            case WallElement.Wall:
                break;
            case WallElement.Doorway:
                {
                    EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serialProps.DoorColumns, new GUIContent("Columns"));
                    EditorGUILayout.PropertyField(serialProps.DoorRows, new GUIContent("Rows"));
                    EditorGUI.indentLevel--;
                    EditorGUILayout.LabelField("Size");
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serialProps.DoorOffset);
                    EditorGUILayout.PropertyField(serialProps.DoorHeight);
                    EditorGUILayout.PropertyField(serialProps.DoorWidth);
                    EditorGUI.indentLevel--;

                    EditorGUILayout.PropertyField(serialProps.DoorActive, new GUIContent("Is Active"));

                    m_IsDoorFoldoutActive = EditorGUILayout.BeginFoldoutHeaderGroup(m_IsDoorFoldoutActive, "Door");

                    if (m_IsDoorFoldoutActive)
                    {
                        EditorGUI.BeginDisabledGroup(!serialProps.DoorActive.boolValue);

                        EditorGUILayout.PropertyField(serialProps.DoorScale);
                        EditorGUILayout.PropertyField(serialProps.DoorDepth);
                        EditorGUILayout.LabelField("Hinge", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serialProps.DoorHingePoint, new GUIContent("Position"));
                        EditorGUILayout.PropertyField(serialProps.DoorHingeOffset, new GUIContent("Offset"));
                        EditorGUILayout.PropertyField(serialProps.DoorHingeEulerAngle, new GUIContent("Euler Angle"));
                        EditorGUI.indentLevel--;

                        EditorGUI.EndDisabledGroup();
                    }

                    EditorGUILayout.EndFoldoutHeaderGroup();

                    m_IsDoorFrameFoldoutActive = EditorGUILayout.BeginFoldoutHeaderGroup(m_IsDoorFoldoutActive, "Frame");

                    if (m_IsDoorFrameFoldoutActive)
                    {
                        EditorGUI.BeginDisabledGroup(!serialProps.DoorActive.boolValue);

                        EditorGUILayout.PropertyField(serialProps.DoorFrameDepth, new GUIContent("Depth"));
                        EditorGUILayout.PropertyField(serialProps.DoorFrameScale, new GUIContent("Scale"));

                        EditorGUI.EndDisabledGroup();
                    }

                    EditorGUILayout.LabelField("Handle", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(serialProps.DoorHandleScale, new GUIContent("Scale"));
                    EditorGUILayout.PropertyField(serialProps.DoorHandlePoint, new GUIContent("Position"));


                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
                break;
            case WallElement.Window:
                {
                    EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serialProps.WindowColumns, new GUIContent("Columns"));
                    EditorGUILayout.PropertyField(serialProps.WindowRows, new GUIContent("Rows"));
                    EditorGUI.indentLevel--;
                    EditorGUILayout.LabelField("Shape", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serialProps.WindowSides, new GUIContent("Sides"));
                    EditorGUILayout.PropertyField(serialProps.WindowHeight, new GUIContent("Height"));
                    EditorGUILayout.PropertyField(serialProps.WindowWidth, new GUIContent("Width"));
                    EditorGUILayout.PropertyField(serialProps.WindowAngle, new GUIContent("Angle"));
                    EditorGUI.indentLevel--;

                    m_IsWindowFoldoutActive = EditorGUILayout.BeginFoldoutHeaderGroup(m_IsWindowFoldoutActive, "Window");

                    if (m_IsWindowFoldoutActive)
                    {
                        EditorGUILayout.PropertyField(serialProps.WindowActiveElements);

                        EditorGUI.BeginDisabledGroup(!windowData.IsOuterFrameActive);
                        EditorGUILayout.LabelField("Outer Frame", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serialProps.WindowOuterFrameScale, new GUIContent("Scale"));
                        EditorGUILayout.PropertyField(serialProps.WindowOuterFrameDepth, new GUIContent("Depth"));
                        EditorGUI.indentLevel--;
                        EditorGUI.EndDisabledGroup();

                        EditorGUI.BeginDisabledGroup(!windowData.IsInnerFrameActive);
                        EditorGUILayout.LabelField("Inner Frame", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serialProps.WindowInnerFrameColumns, new GUIContent("Columns"));
                        EditorGUILayout.PropertyField(serialProps.WindowInnerFrameRows, new GUIContent("Rows"));
                        EditorGUILayout.PropertyField(serialProps.WindowInnerFrameScale, new GUIContent("Scale"));
                        EditorGUILayout.PropertyField(serialProps.WindowInnerFrameDepth, new GUIContent("Depth"));
                        EditorGUI.indentLevel--;
                        EditorGUI.EndDisabledGroup();

                        EditorGUI.BeginDisabledGroup(!windowData.IsPaneActive);
                        EditorGUILayout.LabelField("Pane", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serialProps.WindowPaneDepth);
                        EditorGUI.indentLevel--;
                        EditorGUI.EndDisabledGroup();

                        EditorGUI.BeginDisabledGroup(!windowData.AreShuttersActive);
                        EditorGUILayout.LabelField("Shutters", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serialProps.WindowShuttersDepth, new GUIContent("Depth"));
                        EditorGUILayout.PropertyField(serialProps.WindowShuttersAngle, new GUIContent("Angle"));
                        EditorGUI.indentLevel--;
                        EditorGUI.EndDisabledGroup();
                    }

                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
                break;
            case WallElement.Extension:
                EditorGUILayout.PropertyField(serialProps.ExtensionWidth);
                EditorGUILayout.PropertyField(serialProps.ExtensionHeight);
                EditorGUILayout.PropertyField(serialProps.ExtensionDistance);
                break;
        }

        if(serialProps.SerializedObject.ApplyModifiedProperties())
        {
            section.Build();
        }
    }
}
