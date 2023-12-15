using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Rendering;

[CustomPropertyDrawer(typeof(DoorData))]
public class DoorDataDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        VisualElement content = new VisualElement() { name = "Door Data Content"};

        IBuildable buildable = data.serializedObject.targetObject as IBuildable;

        SerializedProperty activeElements = data.FindPropertyRelative("m_ActiveElements");
        SerializedProperty doorScale = data.FindPropertyRelative("m_Scale");
        SerializedProperty hingeOffset = data.FindPropertyRelative("m_HingeOffset");
        SerializedProperty hingeEulerAngles = data.FindPropertyRelative("m_HingeEulerAngles");
        SerializedProperty hingePoint = data.FindPropertyRelative("m_HingePoint");

        if (buildable is Door)
        {
            PropertyField activeElementsField = new PropertyField(activeElements);
            activeElementsField.BindProperty(activeElements);
            content.Add(activeElementsField);
        }

        PropertyField scaleField = new PropertyField(doorScale);
        scaleField.BindProperty(doorScale);

        Foldout hingeFoldout = new Foldout() { text = "Hinge" };

        PropertyField hingePointField = new PropertyField(hingePoint) { label = "Position" };
        hingePointField.BindProperty(hingePoint);

        PropertyField hingeOffsetField = new PropertyField(hingeOffset) { label = "Offset" };
        hingeOffsetField.BindProperty(hingeOffset);

        PropertyField hingeEulerAnglesField = new PropertyField(hingeEulerAngles) { label = "Euler Angles"};
        hingeEulerAnglesField.BindProperty(hingeEulerAngles);
       
        content.Add(scaleField);
        content.Add(hingeFoldout);
        hingeFoldout.Add(hingePointField);
        hingeFoldout.Add(hingeOffsetField);
        hingeFoldout.Add(hingeEulerAnglesField);

        if(data.serializedObject.ApplyModifiedProperties())
        {
            buildable.Build();
        }

        return content;
    }

}
