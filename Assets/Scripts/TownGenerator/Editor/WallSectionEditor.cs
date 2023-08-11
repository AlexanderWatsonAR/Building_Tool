using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine.ProBuilder.Shapes;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;

[CustomEditor(typeof(WallSection))]
public class WallSectionEditor : Editor
{
    private bool m_IsArchActiveFoldout = true;
    private bool m_IsDoorActiveFoldout = true;
    private bool m_IsWindowActiveFoldout = true;

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
        SerializedProperty doorScale = serializedObject.FindProperty("m_DoorScale");
        SerializedProperty doorHingePoint = serializedObject.FindProperty("m_DoorHingePoint");
        SerializedProperty doorHingeOffset = serializedObject.FindProperty("m_DoorHingeOffset");
        SerializedProperty doorHingeEulerAngles = serializedObject.FindProperty("m_DoorHingeEulerAngles");
        SerializedProperty doorMaterial = serializedObject.FindProperty("m_DoorMaterial");
        // End Door

        // Window
        SerializedProperty winColumns = serializedObject.FindProperty("m_WindowColumns");
        SerializedProperty winRows = serializedObject.FindProperty("m_WindowRows");
        SerializedProperty winHeight = serializedObject.FindProperty("m_WindowHeight");
        SerializedProperty winWidth = serializedObject.FindProperty("m_WindowWidth");
        SerializedProperty winSides = serializedObject.FindProperty("m_WindowSides");
        SerializedProperty winAngles = serializedObject.FindProperty("m_WindowAngle");
        SerializedProperty winActive = serializedObject.FindProperty("m_IsWindowActive");
        SerializedProperty winFColumns = serializedObject.FindProperty("m_WindowFrameColumns");
        SerializedProperty winFRows = serializedObject.FindProperty("m_WindowFrameRows");
        SerializedProperty winFScale = serializedObject.FindProperty("m_WindowFrameScale");

        SerializedProperty winPaneMat = serializedObject.FindProperty("m_WindowPaneMaterial");
        SerializedProperty winFrameMat = serializedObject.FindProperty("m_WindowFrameMaterial");
        // End Window

        // Extension
        SerializedProperty exHeight = serializedObject.FindProperty("m_ExtendHeight");
        SerializedProperty exWidth = serializedObject.FindProperty("m_ExtendWidth");
        SerializedProperty exDistance = serializedObject.FindProperty("m_ExtendDistance");
        // End Extension

        EditorGUILayout.PropertyField(element);

        switch (section.WallElement)
        {
            case WallElement.Wall:
                break;
            case WallElement.Doorway:
                EditorGUILayout.LabelField("Number of Doors");
                EditorGUILayout.IntSlider(doorColumns, 1, 10, "Columns");
                EditorGUILayout.IntSlider(doorRows, 1, 10, "Rows");
                EditorGUILayout.LabelField("Size");
                EditorGUILayout.PropertyField(doorOffset);
                EditorGUILayout.PropertyField(doorHeight);
                EditorGUILayout.PropertyField(doorWidth);

                //doorArched.boolValue = EditorGUILayout.Toggle("Is Arched", doorArched.boolValue);
                m_IsArchActiveFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_IsArchActiveFoldout, "Arch");

                if(m_IsArchActiveFoldout)
                {
                    //EditorGUI.BeginDisabledGroup(!doorArched.boolValue);
                    doorArchHeight.floatValue = EditorGUILayout.Slider("Height", doorArchHeight.floatValue, 0, 1);
                    doorArchSides.intValue = EditorGUILayout.IntSlider("Sides", doorArchSides.intValue, 3, 32);
                    //EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();

                doorActive.boolValue = EditorGUILayout.Toggle("Is Active", doorActive.boolValue);

                m_IsDoorActiveFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_IsDoorActiveFoldout, "Door");

                if (m_IsDoorActiveFoldout)
                {
                    EditorGUI.BeginDisabledGroup(!doorActive.boolValue);
                    doorScale.vector3Value = EditorGUILayout.Vector3Field("Scale", doorScale.vector3Value);
                    EditorGUILayout.ObjectField(doorMaterial, new GUIContent("Material"));

                    EditorGUILayout.LabelField("Hinge");
                    doorHingePoint.SetEnumValue((TransformPoint)EditorGUILayout.EnumPopup("Position", doorHingePoint.GetEnumValue<TransformPoint>()));
                    doorHingeOffset.vector3Value = EditorGUILayout.Vector3Field("Offset", doorHingeOffset.vector3Value);
                    doorHingeEulerAngles.vector3Value = EditorGUILayout.Vector3Field("Rotation", doorHingeEulerAngles.vector3Value);
                    EditorGUI.EndDisabledGroup();
                }

                EditorGUILayout.EndFoldoutHeaderGroup();

                break;
            case WallElement.Window:
                EditorGUILayout.LabelField("Number of Windows");
                EditorGUILayout.IntSlider(winColumns, 1, 10, "Columns");
                EditorGUILayout.IntSlider(winRows, 1, 10, "Rows");
                EditorGUILayout.IntSlider(winSides, 3, 32, "Sides");
                EditorGUILayout.LabelField("Size");
                EditorGUILayout.Slider(winHeight, 0, 0.999f, "Height");
                EditorGUILayout.Slider(winWidth, 0, 0.999f, "Width");
                EditorGUILayout.Slider(winAngles, -180, 180, "Angle");

                winActive.boolValue = EditorGUILayout.Toggle("Is Active", winActive.boolValue);

                if (winActive.boolValue)
                {
                    m_IsWindowActiveFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_IsWindowActiveFoldout, "Window");

                    if (m_IsWindowActiveFoldout)
                    {
                        winFColumns.intValue = EditorGUILayout.IntSlider("Columns", winFColumns.intValue, 1, 10);
                        winFRows.intValue = EditorGUILayout.IntSlider("Rows", winFRows.intValue, 1, 10);
                        EditorGUILayout.LabelField("Size");
                        float y = EditorGUILayout.Slider("Height", winFScale.vector3Value.y, 0, 0.999f);
                        float x = EditorGUILayout.Slider("Width", winFScale.vector3Value.x, 0, 0.999f);
                        winFScale.vector3Value = new Vector3(x, y, winFScale.vector3Value.z);
                        EditorGUILayout.LabelField("Material");
                        EditorGUILayout.ObjectField(winPaneMat, new GUIContent("Pane"));
                        EditorGUILayout.ObjectField(winFrameMat, new GUIContent("Frame"));
                    }
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
