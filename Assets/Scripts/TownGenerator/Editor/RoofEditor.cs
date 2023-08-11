using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.Rendering;
using Unity.VisualScripting;
using TreeEditor;
using Cinemachine.Editor;

[CustomEditor(typeof(Roof))]
public class RoofEditor : Editor
{
    private bool m_ShowPyramid = true;
    private bool m_ShowMansard = true;
    private bool m_ShowGable = true;
    private bool m_ShowMShaped = true;
    private bool m_ShowTile = true;

    public override void OnInspectorGUI()
    {
        Roof roof = (Roof)target;
        serializedObject.Update();

        SerializedProperty roofData = serializedObject.FindProperty("m_Data");

        SerializedProperty roofActive = roofData.FindPropertyRelative("m_IsActive");
        SerializedProperty frameType = roofData.FindPropertyRelative("m_RoofType");
        SerializedProperty mansardHeight = roofData.FindPropertyRelative("m_MansardHeight");
        SerializedProperty mansardScale = roofData.FindPropertyRelative("m_MansardScale");
        SerializedProperty pyramidHeight = roofData.FindPropertyRelative("m_PyramidHeight");
        SerializedProperty gableHeight = roofData.FindPropertyRelative("m_GableHeight");
        SerializedProperty gableScale = roofData.FindPropertyRelative("m_GableScale");
        SerializedProperty isFlipped = roofData.FindPropertyRelative("m_IsFlipped");
        SerializedProperty isOpen = roofData.FindPropertyRelative("m_IsOpen");

        SerializedProperty roofTileData = roofData.FindPropertyRelative("m_RoofTileData");

        roofActive.boolValue = EditorGUILayout.BeginToggleGroup("Is Active", roofActive.boolValue);

        int index = 0;
        int value = (int)frameType.GetEnumValue<RoofType>();

        int[] frames = roof.AvailableRoofFrames();

        if (frames.Length > 0)
        {
            string[] allOptions = frameType.enumNames;
            string[] allOptionsDisplay = frameType.enumDisplayNames; // "Gable", "Mansard", "Flat", "Dormer", "M Shaped", "Pyramid", "Pyramid Hip"
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

            switch (frameType.enumValueIndex)
            {
                case (int)RoofType.Gable:
                    DisplayGable(gableHeight, gableScale, isOpen);
                    break;
                case (int)RoofType.Mansard:
                    DisplayMansard(mansardHeight, mansardScale);
                    break;
                case (int)RoofType.Dormer:
                    DisplayMansard(mansardHeight, mansardScale);
                    DisplayGable(gableHeight, gableScale, isOpen);
                    break;
                case (int)RoofType.MShaped:
                    DisplayMShaped(gableHeight, isFlipped);
                    break;
                case (int)RoofType.Pyramid:
                    DisplayPyramid(pyramidHeight);
                    break;
                case (int)RoofType.PyramidHip:
                    DisplayMansard(mansardHeight, mansardScale);
                    DisplayPyramid(pyramidHeight);
                    break;
            }
        }
        else
        {
            string[] noOptionsDisplay = new string[] { "No Frame Type Available" };
            EditorGUILayout.Popup(0, noOptionsDisplay);
        }

        DisplayTile(roofTileData);
        EditorGUILayout.EndToggleGroup();
        ApplyRoofChanges(roof);

    }

    private void ApplyRoofChanges(Roof roof)
    {
        if (serializedObject.ApplyModifiedProperties())
        {
            if (roof.TryGetComponent(out Building building))
            {
                building.Build();
            }
            else if(roof.TryGetComponent(out WallSection wallSection))
            {
                wallSection.Build();
            }
            else
            {
                roof.BuildFrame();
                roof.OnAnyRoofChange_Invoke();
            }
        }
    }

    private void DisplayMShaped(SerializedProperty height, SerializedProperty rotate)
    {
        m_ShowMShaped = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowMShaped, "M Shaped");
        if (m_ShowMShaped)
        {
            EditorGUILayout.Slider(height, 0, 10, "Height");
            EditorGUILayout.PropertyField(rotate);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DisplayGable(SerializedProperty height, SerializedProperty scale, SerializedProperty isOpen)
    {
        m_ShowGable = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowGable, "Gable");
        if (m_ShowGable)
        {
            EditorGUILayout.Slider(height, 0, 10, "Height");

            if(!isOpen.boolValue)
            {
                EditorGUILayout.Slider(scale, 0, 1, "Scale");
            }

            EditorGUILayout.PropertyField(isOpen);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DisplayPyramid(SerializedProperty height)
    {
        m_ShowPyramid = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowPyramid, "Pyramid");
        if (m_ShowPyramid)
        {
            EditorGUILayout.Slider(height, 0, 10, "Height");
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }//

    private void DisplayMansard(SerializedProperty height, SerializedProperty scale)
    {
        m_ShowMansard = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowMansard, "Mansard");
        if (m_ShowMansard)
        {
            EditorGUILayout.Slider(height, 0, 10, "Height");
            EditorGUILayout.Slider(scale, 0, 2, "Scale");
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DisplayTile(SerializedProperty roofTileData)
    {
        m_ShowTile = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowTile, "Tile");
        if (m_ShowTile)
        {
            //SerializedProperty endProp = roofTileData.GetEndProperty();

            bool enterChildren = true;
            while (roofTileData.NextVisible(enterChildren))
            {
                enterChildren = false;
                EditorGUILayout.PropertyField(roofTileData);

                if (roofTileData.displayName == "Rows")
                    break;

                //if (roofTileData == endProp)
                //    break;
            }

            //EditorGUILayout.Slider(height, 0, 10, "Height");
            //EditorGUILayout.Slider(extend, 0, 10, "Extend");
            //EditorGUILayout.ObjectField(mat, new GUIContent("Material"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }


}
