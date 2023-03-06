using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.Rendering;
using Unity.VisualScripting;

[CustomEditor(typeof(Roof))]
public class RoofEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();

        Roof roof = (Roof)target;
        serializedObject.Update();

        EditorGUILayout.ObjectField(serializedObject.FindProperty("m_CentreBeamPrefab"));
        EditorGUILayout.ObjectField(serializedObject.FindProperty("m_SupportBeamPrefab"));
        EditorGUILayout.ObjectField(serializedObject.FindProperty("m_RoofTileMaterial"));

        SerializedProperty frameType = serializedObject.FindProperty("m_FrameType");

        int index = 0;
        int value = (int) frameType.GetEnumValue<RoofType>();
        string frameName = frameType.GetEnumName<RoofType>();

        int[] frames = roof.AvailableRoofFrames();
        string[] allOptions = frameType.enumNames;
        string[] allOptionsDisplay = frameType.enumDisplayNames; // "Open Gable", "Mansard", "Flat", "Dormer", "M Shaped", "Pyramid", "Pyramid Hip"
        string[] options = new string[frames.Length];
        string[] optionsDisplay = new string[frames.Length];

        for (int i = 0; i < frames.Length; i++)
        {
            optionsDisplay[i] = allOptionsDisplay[frames[i]];
            options[i] = allOptions[frames[i]];

            if (frames[i] == value)
            {
                index = i;
            }
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Frame Type");

        if (index == -1)
        {
            index = 0;
        }

        int frameIndex = EditorGUILayout.Popup(index, optionsDisplay);

        frameType.SetEnumValue((RoofType)frames[frameIndex]);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Height"));

        float roofHeight = serializedObject.FindProperty("m_Height").floatValue;
        float height = roof.Height;

        RoofType roofType = serializedObject.FindProperty("m_FrameType").GetEnumValue<RoofType>();
        RoofType type = roof.FrameType;

        if (serializedObject.ApplyModifiedProperties())
        {
            if (height != roofHeight ||
                type != roofType)
            {
                if (roof.TryGetComponent(out Building building))
                {
                    building.Construct();
                }
                else
                {
                    roof.ConstructFrame();
                }
            }
        }
    }


}
