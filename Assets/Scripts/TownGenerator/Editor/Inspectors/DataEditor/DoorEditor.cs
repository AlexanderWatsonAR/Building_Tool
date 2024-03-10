using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(Door))]
public class DoorEditor : DataEditor
{
    Vector3 m_HingePosition;
    Quaternion m_HingeRotation = Quaternion.identity;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = base.CreateInspectorGUI();
        
        Vector3 forward = m_Data.FindPropertyRelative("m_Normal").vector3Value;

        m_HingeRotation = Quaternion.LookRotation(forward, Vector3.up);

        return root;
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
        m_HingePosition = m_Data.FindPropertyRelative("m_HingePosition").vector3Value;
        SerializedProperty hingeOffset = m_Data.FindPropertyRelative("m_HingeOffset");
        Vector3 position = Handles.DoPositionHandle(door.transform.TransformPoint(m_HingePosition + hingeOffset.vector3Value), m_HingeRotation);
        hingeOffset.vector3Value = door.transform.InverseTransformPoint(position - m_HingePosition);
        serializedObject.ApplyModifiedProperties();
    }

}
