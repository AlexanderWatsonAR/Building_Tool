using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine.ProBuilder.Shapes;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(WallSection))]
public class WallSectionEditor : Editor
{
    //private bool m_IsArchFoldoutActive = true;
    //private bool m_IsDoorFoldoutActive = true;
    //private bool m_IsDoorFrameFoldoutActive = true;
    //private bool m_IsWindowFoldoutActive = true;

    //WallSection m_Section;
    //WallSectionSerializedProperties m_SerializedProperties;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement container = new VisualElement();

        serializedObject.Update();

        SerializedProperty data = serializedObject.FindProperty("m_Data");

        PropertyField dataField = new PropertyField(data);
        dataField.BindProperty(data);

        container.Add(dataField);

        return container;
    }

    //public override void OnInspectorGUI()
    //{
    //    serializedObject.Update();
    //    m_Section = (WallSection)target;

    //    m_SerializedProperties = new WallSectionSerializedProperties(m_Section);

    //    WindowData windowData = m_Section.Data.WindowData;

    //    EditorGUILayout.PropertyField(m_SerializedProperties.WallElement, new GUIContent("Section"));

    //    switch (m_Section.Data.WallElement)
    //    {
    //        case WallElement.Wall:
    //            break;
    //        case WallElement.Doorway:
    //            DisplayDoorway();
    //            break;
    //        case WallElement.Window:
    //            {
    //                EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
    //                EditorGUI.indentLevel++;
    //                EditorGUILayout.PropertyField(m_SerializedProperties.WindowColumns, new GUIContent("Columns"));
    //                EditorGUILayout.PropertyField(m_SerializedProperties.WindowRows, new GUIContent("Rows"));
    //                EditorGUI.indentLevel--;
    //                EditorGUILayout.LabelField("Shape", EditorStyles.boldLabel);
    //                EditorGUI.indentLevel++;
    //                EditorGUILayout.PropertyField(m_SerializedProperties.WindowSides, new GUIContent("Sides"));
    //                EditorGUILayout.PropertyField(m_SerializedProperties.WindowHeight, new GUIContent("Height"));
    //                EditorGUILayout.PropertyField(m_SerializedProperties.WindowWidth, new GUIContent("Width"));
    //                EditorGUILayout.PropertyField(m_SerializedProperties.WindowAngle, new GUIContent("Angle"));
    //                EditorGUI.indentLevel--;

    //                m_IsWindowFoldoutActive = EditorGUILayout.BeginFoldoutHeaderGroup(m_IsWindowFoldoutActive, "Window");

    //                if (m_IsWindowFoldoutActive)
    //                {
    //                    EditorGUILayout.PropertyField(m_SerializedProperties.WindowActiveElements);

    //                    EditorGUI.BeginDisabledGroup(!windowData.IsOuterFrameActive);
    //                    EditorGUILayout.LabelField("Outer Frame", EditorStyles.boldLabel);
    //                    EditorGUI.indentLevel++;
    //                    EditorGUILayout.PropertyField(m_SerializedProperties.WindowOuterFrameScale, new GUIContent("Scale"));
    //                    EditorGUILayout.PropertyField(m_SerializedProperties.WindowOuterFrameDepth, new GUIContent("Depth"));
    //                    EditorGUI.indentLevel--;
    //                    EditorGUI.EndDisabledGroup();

    //                    EditorGUI.BeginDisabledGroup(!windowData.IsInnerFrameActive);
    //                    EditorGUILayout.LabelField("Inner Frame", EditorStyles.boldLabel);
    //                    EditorGUI.indentLevel++;
    //                    EditorGUILayout.PropertyField(m_SerializedProperties.WindowInnerFrameColumns, new GUIContent("Columns"));
    //                    EditorGUILayout.PropertyField(m_SerializedProperties.WindowInnerFrameRows, new GUIContent("Rows"));
    //                    EditorGUILayout.PropertyField(m_SerializedProperties.WindowInnerFrameScale, new GUIContent("Scale"));
    //                    EditorGUILayout.PropertyField(m_SerializedProperties.WindowInnerFrameDepth, new GUIContent("Depth"));
    //                    EditorGUI.indentLevel--;
    //                    EditorGUI.EndDisabledGroup();

    //                    EditorGUI.BeginDisabledGroup(!windowData.IsPaneActive);
    //                    EditorGUILayout.LabelField("Pane", EditorStyles.boldLabel);
    //                    EditorGUI.indentLevel++;
    //                    EditorGUILayout.PropertyField(m_SerializedProperties.WindowPaneDepth);
    //                    EditorGUI.indentLevel--;
    //                    EditorGUI.EndDisabledGroup();

    //                    EditorGUI.BeginDisabledGroup(!windowData.AreShuttersActive);
    //                    EditorGUILayout.LabelField("Shutters", EditorStyles.boldLabel);
    //                    EditorGUI.indentLevel++;
    //                    EditorGUILayout.PropertyField(m_SerializedProperties.WindowShuttersDepth, new GUIContent("Depth"));
    //                    EditorGUILayout.PropertyField(m_SerializedProperties.WindowShuttersAngle, new GUIContent("Angle"));
    //                    EditorGUI.indentLevel--;
    //                    EditorGUI.EndDisabledGroup();
    //                }

    //                EditorGUILayout.EndFoldoutHeaderGroup();
    //            }
    //            break;
    //        case WallElement.Extension:
    //            EditorGUILayout.PropertyField(m_SerializedProperties.ExtensionWidth);
    //            EditorGUILayout.PropertyField(m_SerializedProperties.ExtensionHeight);
    //            EditorGUILayout.PropertyField(m_SerializedProperties.ExtensionDistance);
    //            break;
    //        case WallElement.Archway:
    //            DisplayDoorway();
    //            break;
    //    }

    //    if(m_SerializedProperties.SerializedObject.ApplyModifiedProperties())
    //    {
    //        m_Section.Build();
    //        m_Section.OnDataChange_Invoke();
    //    }
    //}

    //private void DisplayDoorway()
    //{
    //    EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
    //    EditorGUI.indentLevel++;
    //    EditorGUILayout.PropertyField(m_SerializedProperties.DoorColumns, new GUIContent("Columns"));
    //    EditorGUILayout.PropertyField(m_SerializedProperties.DoorRows, new GUIContent("Rows"));
    //    EditorGUI.indentLevel--;
    //    EditorGUILayout.LabelField("Size");
    //    EditorGUI.indentLevel++;

    //    EditorGUILayout.PropertyField(m_SerializedProperties.DoorOffset);
    //    EditorGUILayout.PropertyField(m_SerializedProperties.DoorHeight);
    //    EditorGUILayout.PropertyField(m_SerializedProperties.DoorWidth);

    //    if (m_Section.Data.WallElement == WallElement.Archway)
    //    {
    //        EditorGUILayout.PropertyField(m_SerializedProperties.DoorArchSides, new GUIContent("Sides"));
    //        EditorGUILayout.PropertyField(m_SerializedProperties.DoorArchHeight, new GUIContent("Height"));
    //    }

    //    EditorGUI.indentLevel--;

    //    EditorGUILayout.PropertyField(m_SerializedProperties.DoorElement, new GUIContent("Active Elements"));

    //    m_IsDoorFoldoutActive = EditorGUILayout.BeginFoldoutHeaderGroup(m_IsDoorFoldoutActive, "Door");

    //    if (m_IsDoorFoldoutActive)
    //    {
    //        DoorElement doorElement = m_SerializedProperties.DoorElement.GetEnumValue<DoorElement>();

    //        bool isNothing = doorElement.IsElementActive(DoorElement.Nothing);
    //        bool isDoor = doorElement.IsElementActive(DoorElement.Door);
    //        bool isFrame = doorElement.IsElementActive(DoorElement.Frame);
    //        bool isHandle = doorElement.IsElementActive(DoorElement.Handle);

    //        EditorGUI.BeginDisabledGroup(isNothing);
    //        EditorGUI.BeginDisabledGroup(!isDoor);

    //        EditorGUI.indentLevel++;
    //        EditorGUILayout.LabelField("Door", EditorStyles.boldLabel);
    //        EditorGUI.indentLevel++;
    //        EditorGUILayout.PropertyField(m_SerializedProperties.DoorScale);
    //        EditorGUILayout.PropertyField(m_SerializedProperties.DoorDepth);

    //        EditorGUILayout.LabelField("Hinge", EditorStyles.boldLabel);
    //        EditorGUI.indentLevel++;
    //        EditorGUILayout.PropertyField(m_SerializedProperties.DoorHingePoint, new GUIContent("Position"));
    //        EditorGUILayout.PropertyField(m_SerializedProperties.DoorHingeOffset, new GUIContent("Offset"));
    //        EditorGUILayout.PropertyField(m_SerializedProperties.DoorHingeEulerAngle, new GUIContent("Euler Angle"));
    //        EditorGUI.indentLevel--;

    //        EditorGUI.BeginDisabledGroup(!isHandle);

    //        EditorGUILayout.LabelField("Handle", EditorStyles.boldLabel);
    //        EditorGUI.indentLevel++;
    //        EditorGUILayout.PropertyField(m_SerializedProperties.DoorHandleScale, new GUIContent("Scale"));
    //        EditorGUILayout.PropertyField(m_SerializedProperties.DoorHandlePoint, new GUIContent("Position"));
    //        EditorGUI.indentLevel--;
    //        EditorGUI.indentLevel--;

    //        EditorGUI.EndDisabledGroup();
    //        EditorGUI.EndDisabledGroup();

    //        EditorGUI.BeginDisabledGroup(!isFrame);

    //        EditorGUILayout.LabelField("Frame", EditorStyles.boldLabel);
    //        EditorGUI.indentLevel++;
    //        EditorGUILayout.PropertyField(m_SerializedProperties.DoorFrameDepth, new GUIContent("Depth"));
    //        EditorGUILayout.PropertyField(m_SerializedProperties.DoorFrameScale, new GUIContent("Scale"));
    //        EditorGUI.indentLevel--;

    //        EditorGUI.EndDisabledGroup();
    //        EditorGUI.EndDisabledGroup();
    //    }
    //}

}
