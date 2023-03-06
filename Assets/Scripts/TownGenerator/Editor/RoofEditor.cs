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

        int index = serializedObject.FindProperty("m_FrameType").intValue;

        int[] frames = roof.AvailableRoofFrames();
        string[] allOptions = serializedObject.FindProperty("m_FrameType").enumDisplayNames; // "Open Gable", "Mansard", "Flat", "Dormer", "M Shaped", "Pyramid", "Pyramid Hip"
        string[] options = new string[frames.Length];

        for(int i = 0; i < frames.Length; i++)
        {
            options[i] = allOptions[frames[i]];
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Frame Type");

        int frameIndex = EditorGUILayout.Popup(index, options);
        //serializedObject.FindProperty("m_FrameType").SetEnumValue((RoofType)frames[frameIndex]); // grabs the current index from the frames array.
        EditorGUILayout.EndHorizontal();

        //EditorGUILayout.PropertyField(serializedObject.FindProperty("m_FrameType"));
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
