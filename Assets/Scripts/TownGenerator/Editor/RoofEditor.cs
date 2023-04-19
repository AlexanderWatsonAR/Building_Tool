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

        if(frameType.enumValueIndex == (int)RoofType.Dormer || frameType.enumValueIndex == (int)RoofType.Mansard)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_MansardHeight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_MansardScale"));
        }

        if(frameType.enumValueIndex != (int)RoofType.Mansard)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Height"));
        }

        EditorGUILayout.LabelField("Tile");

        SerializedProperty tileHeight = serializedObject.FindProperty("m_TileHeight");
        SerializedProperty tileExtend = serializedObject.FindProperty("m_TileExtend");

        float tHeight = roof.TileHeight;
        float tExtend = roof.TileExtend;

        EditorGUILayout.PropertyField(tileExtend);
        EditorGUILayout.PropertyField(tileHeight);

        float mansardRoofScale = serializedObject.FindProperty("m_MansardScale").floatValue;
        float mansardScale = roof.MansardScale;

        float mansardRoofHeight = serializedObject.FindProperty("m_MansardHeight").floatValue;
        float mansardheight = roof.MansardHeight;

        float roofHeight = serializedObject.FindProperty("m_Height").floatValue;
        float height = roof.Height;

        RoofType roofType = serializedObject.FindProperty("m_FrameType").GetEnumValue<RoofType>();
        RoofType type = roof.FrameType;

        if (serializedObject.ApplyModifiedProperties())
        {
            if (height != roofHeight ||
                mansardheight != mansardRoofHeight ||
                type != roofType ||
                tHeight != tileHeight.floatValue ||
                tExtend != tileExtend.floatValue ||
                mansardScale != mansardRoofScale)
            {
                if (roof.TryGetComponent(out Building building))
                {
                    building.Build();
                }
                else
                {
                    roof.ConstructFrame();
                    roof.OnAnyRoofChange_Invoke();
                }
            }
        }
    }


}
