using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;


[CustomEditor(typeof(GridFrame))]
public class GridFrameEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        serializedObject.Update();
        VisualElement root = new VisualElement();

        SerializedProperty data = serializedObject.FindProperty("m_Data");

        PropertyField dataField = new PropertyField(data);
        dataField.BindProperty(data);

        root.Add(dataField);

        return root;
    }
}
