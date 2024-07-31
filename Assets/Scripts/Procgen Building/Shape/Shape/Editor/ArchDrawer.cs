using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(Arch))]
public class ArchDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        VisualElement root = new VisualElement();

        SerializedProperty archHeight = data.FindPropertyRelative("m_TopHeight");
        SerializedProperty baseHeight = data.FindPropertyRelative("m_BottomHeight");
        SerializedProperty sides = data.FindPropertyRelative("m_Sides");

        FloatField archHeightField = new FloatField("Arch Height");
        FloatField baseHeightField = new FloatField("Base Height");
        SliderInt sidesField = new SliderInt()
        {
            label = "Sides",
            lowValue = 3,
            highValue = 16,
        };

        archHeightField.BindProperty(archHeight);
        baseHeightField.BindProperty(baseHeight);
        sidesField.BindProperty(sides);

        root.Add(archHeightField);
        root.Add(baseHeightField);
        root.Add(sidesField);

        return root;
    }
}
