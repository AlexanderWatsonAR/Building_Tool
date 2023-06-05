using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine.UIElements;

[CustomEditor(typeof(Door))]
public class DoorEditor : Editor
{
    private Vector3 m_HingePosition;
    private Quaternion m_HingeRotation = Quaternion.identity;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Door door = (Door)target;

        m_HingePosition = serializedObject.FindProperty("m_HingePosition").vector3Value;
        Vector3 forward = serializedObject.FindProperty("m_Forward").vector3Value;
        m_HingeRotation = Quaternion.LookRotation(forward, Vector3.up);

        SerializedProperty doorScale = serializedObject.FindProperty("m_Scale");
        SerializedProperty hingeOffset = serializedObject.FindProperty("m_HingeOffset");
        SerializedProperty hingeEulerAngles = serializedObject.FindProperty("m_HingeEulerAngles");
        SerializedProperty hingePoint = serializedObject.FindProperty("m_HingePoint");

        doorScale.vector3Value = EditorGUILayout.Vector3Field("Scale", doorScale.vector3Value);
        EditorGUILayout.LabelField("Hinge");
        door.HingePoint = (TransformPoint) EditorGUILayout.EnumPopup("Position", hingePoint.GetEnumValue<TransformPoint>());
        hingeOffset.vector3Value = EditorGUILayout.Vector3Field("Offset", hingeOffset.vector3Value);
        hingeEulerAngles.vector3Value = EditorGUILayout.Vector3Field("Rotation", hingeEulerAngles.vector3Value);

        if (serializedObject.ApplyModifiedProperties())
        {
            door.Build();
        }
    }

    private void OnSceneGUI()
    {
        Draw();
    }

    private void Draw()
    {
        serializedObject.Update();
        SerializedProperty hingeOffset = serializedObject.FindProperty("m_HingeOffset");
        Vector3 position = Handles.DoPositionHandle(m_HingePosition + hingeOffset.vector3Value, m_HingeRotation);
        hingeOffset.vector3Value = position - m_HingePosition;
        serializedObject.ApplyModifiedProperties();
    }

}
