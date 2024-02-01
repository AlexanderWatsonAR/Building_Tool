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

        VisualElement myInspector = new VisualElement();

        myInspector.Add(new PropertyField(serializedObject.FindProperty("m_Data")));

        return myInspector;
    }

}
