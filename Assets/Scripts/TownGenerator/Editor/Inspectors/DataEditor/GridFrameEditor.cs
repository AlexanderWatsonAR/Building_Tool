using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;


[CustomEditor(typeof(GridFrame))]
public class GridFrameEditor : DataEditor
{
    public override VisualElement CreateInspectorGUI()
    {
        return base.CreateInspectorGUI();
    }
}
