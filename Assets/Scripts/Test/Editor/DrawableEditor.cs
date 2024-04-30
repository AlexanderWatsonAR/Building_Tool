using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(Drawable))]
public class DrawableEditor : Editor
{

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        Drawable drawable = (Drawable)target;

        Vector3[] positions = drawable.Path.Positions;

        ListView points = new ListView(positions);
        points.BindProperty(serializedObject);

        root.Add(points);

        return root;
    }

}
