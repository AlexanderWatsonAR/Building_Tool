using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(NPolygon))]
public class NPolygonDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty nPolygon)
    {
        SerializedProperty sides = nPolygon.FindPropertyRelative("m_Sides");

        VisualElement root = new VisualElement();

        SliderInt slider = new SliderInt()
        {
            label = "Sides",
            lowValue = 3,
            highValue = 18,
            showInputField = true,
        };

        slider.BindProperty(sides);

        root.Add(slider);

        return root;

        
    }
}
