using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(Door))]
public class DoorEditor : Editor
{
    private Vector3 m_HingePosition;
    private Quaternion m_HingeRotation = Quaternion.identity;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement content = new VisualElement();

        serializedObject.Update();

        SerializedProperty data = serializedObject.FindProperty("m_Data");

        PropertyField dataField = new PropertyField(data);
        dataField.BindProperty(data);
        
        Vector3 forward = data.FindPropertyRelative("m_Normal").vector3Value;

        m_HingeRotation = Quaternion.LookRotation(forward, Vector3.up);

        content.Add(dataField);

        return content;
    }

    private void OnSceneGUI()
    {
        Draw();
    }

    /// <summary>
    /// Draws a position gizmo for setting the hinge position.
    /// </summary>
    private void Draw()
    {
        serializedObject.Update();
        Door door = target as Door;
        SerializedProperty data = serializedObject.FindProperty("m_Data");
        m_HingePosition = data.FindPropertyRelative("m_HingePosition").vector3Value;
        SerializedProperty hingeOffset = data.FindPropertyRelative("m_HingeOffset");
        Vector3 position = Handles.DoPositionHandle(door.transform.TransformPoint(m_HingePosition + hingeOffset.vector3Value), m_HingeRotation);
        hingeOffset.vector3Value = door.transform.InverseTransformPoint(position - m_HingePosition);
        serializedObject.ApplyModifiedProperties();
    }

}
