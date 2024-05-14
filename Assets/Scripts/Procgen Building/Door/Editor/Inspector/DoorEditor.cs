using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Door
{
    [CustomEditor(typeof(Door))]
    public class DoorEditor : BuildableEditor
    {
        DoorDataSerializedProperties m_Props;
        Vector3 m_HingePosition;
        Quaternion m_HingeRotation = Quaternion.identity;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = base.CreateInspectorGUI();

            m_Props = new DoorDataSerializedProperties(m_Data);

            Vector3 forward = m_Props.Normal.vector3Value;
            Vector3 up = m_Props.Up.vector3Value;

            m_HingeRotation = Quaternion.LookRotation(forward, up);

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
            m_HingePosition = m_Props.Hinge.AbsolutePosition.vector3Value;
            SerializedProperty hingeOffset = m_Props.Hinge.PositionOffset;
            Vector3 position = Handles.DoPositionHandle(door.transform.TransformPoint(m_HingePosition + hingeOffset.vector3Value), m_HingeRotation);
            hingeOffset.vector3Value = door.transform.InverseTransformPoint(position - m_HingePosition);
            serializedObject.ApplyModifiedProperties();
        }

    }
}
