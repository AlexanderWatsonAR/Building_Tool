using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorScripting : EditorWindow
{
    //string text = "This is some text for the test window.";
    //string customText = "Enter text here";
    //int slider = 0;
    //bool toggle = true;

    [MenuItem("Tools/Polybuilder")]

    public static void ShowWindow()
    {
        GetWindow(typeof(EditorScripting));
    }

    private void OnGUI()
    {
        if(GUILayout.Button("New Poly building"))//
        {
            GameObject building = new GameObject("Poly Building", typeof(Polytool));
        }

        //GUILayout.Label("Heading", EditorStyles.boldLabel);
        //GUILayout.Label(text);
        //toggle = EditorGUILayout.BeginToggleGroup("Settings", toggle);

        //customText = EditorGUILayout.TextField("Text Field", customText);
        //slider = EditorGUILayout.IntSlider("Custom Slider", slider, -5, 5);

        //EditorGUILayout.EndToggleGroup();


    }

}
