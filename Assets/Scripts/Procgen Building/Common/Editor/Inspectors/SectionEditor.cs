using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;
using OnlyInvalid.CustomVisualElements;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [CustomEditor(typeof(Section))]
    public class SectionEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            SerializedProperty buildables = serializedObject.FindProperty("m_Buildables");

            PropertyField buildablesField = new PropertyField(buildables);

            root.Add(buildablesField);

            SerializedProperty sectionData = serializedObject.FindProperty("m_SectionData");
            SerializedProperty openings = sectionData.FindPropertyRelative("m_Openings");

            SerializedProperty firstOpening = openings.GetArrayElementAtIndex(0);

            HeaderFoldout foldout = new HeaderFoldout("My Foldout");
            foldout.AddItem(new Label("Item 0"));
            foldout.AddItem(new Label("Item 1"));
            foldout.AddItem(new Label("Item 2"));
            foldout.AddItem(new Label("Item 3"));

            

            Foldout f = new Foldout();

            f.text = "Unity Foldout";
            f.Add(new Label("Item 0"));
            f.Add(new Label("Item 1"));
            f.Add(new Label("Item 2"));
            f.Add(new Label("Item 3"));

            root.Add(foldout);
            root.Add(f);

            return root;
        }

    }
}
