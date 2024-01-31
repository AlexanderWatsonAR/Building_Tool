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
        
        Vector3 forward = data.FindPropertyRelative("m_Forward").vector3Value;

        m_HingeRotation = Quaternion.LookRotation(forward, Vector3.up);

        content.Add(dataField);

        return content;
    }

    //public override void OnInspectorGUI()
    //{
    //    serializedObject.Update();
    //    Door door = (Door)target;

    //    SerializedProperty data = serializedObject.FindProperty("m_Data");

    //    SerializedProperty activeElements = data.FindPropertyRelative("m_ActiveElements");

    //    int flag = (int)activeElements.GetEnumValue<DoorwayElement>();

    //    if(flag == (int) DoorwayElement.Frame)
    //    {
    //        flag = (int)DoorwayElement.Nothing;
    //    }    

    //    activeElements.SetEnumValue<DoorwayElement>( (DoorwayElement) EditorGUILayout.MaskField("Active Elements", flag, new string[] { "Door", "Handle"}));

    //    m_HingePosition = data.FindPropertyRelative("m_HingePosition").vector3Value;
    //    Vector3 forward = data.FindPropertyRelative("m_Forward").vector3Value;
    //    

    //    SerializedProperty doorScale = data.FindPropertyRelative("m_Scale");
    //    SerializedProperty hingeOffset = data.FindPropertyRelative("m_HingeOffset");
    //    SerializedProperty hingeEulerAngles = data.FindPropertyRelative("m_HingeEulerAngles");
    //    SerializedProperty hingePoint = data.FindPropertyRelative("m_HingePoint");

    //    EditorGUILayout.PropertyField(doorScale);
    //    //doorScale.vector3Value = EditorGUILayout.Vector3Field("Scale", doorScale.vector3Value);
    //    EditorGUILayout.LabelField("Hinge");
    //    door.DoorData.HingePoint = (TransformPoint) EditorGUILayout.EnumPopup("Position", hingePoint.GetEnumValue<TransformPoint>());
    //    hingeOffset.vector3Value = EditorGUILayout.Vector3Field("Offset", hingeOffset.vector3Value);
    //    hingeEulerAngles.vector3Value = EditorGUILayout.Vector3Field("Rotation", hingeEulerAngles.vector3Value);

    //    if (serializedObject.ApplyModifiedProperties())
    //    {
    //        door.Build();
    //    }
    //}

    private void OnSceneGUI()
    {
        Draw();
    }

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