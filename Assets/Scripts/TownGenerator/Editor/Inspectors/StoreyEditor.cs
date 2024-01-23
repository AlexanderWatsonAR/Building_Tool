using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(Storey))]
public class StoreyEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        serializedObject.Update();

        // Create a new VisualElement to be the root of our inspector UI
        VisualElement myInspector = new VisualElement();

        // Add a simple label
        myInspector.Add(new PropertyField(serializedObject.FindProperty("m_Wrapper").FindPropertyRelative("Data")));

        // Return the finished inspector UI
        return myInspector;
    }

}
