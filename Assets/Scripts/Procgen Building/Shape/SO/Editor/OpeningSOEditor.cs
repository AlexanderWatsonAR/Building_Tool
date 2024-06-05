using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [CustomEditor(typeof(OpeningSO))]
    public class OpeningSOEditor : Editor
    {

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement() { name = nameof(OpeningSO) + " Root" };

            OpeningSO openingScriptableObject = target as OpeningSO;

            #region Properties
            SerializedProperty opening = serializedObject.FindProperty("m_Opening");
            SerializedProperty shapeSO = serializedObject.FindProperty("m_ShapeSO");
            SerializedProperty shape = opening.FindPropertyRelative("m_Shape");
            SerializedProperty height = opening.FindPropertyRelative("m_Height");
            SerializedProperty width = opening.FindPropertyRelative("m_Width");
            SerializedProperty angle = opening.FindPropertyRelative("m_Angle");
            SerializedProperty position = opening.FindPropertyRelative("m_Position");
            #endregion

            #region Fields
            PropertyField shapeSOField = new PropertyField(shapeSO) { label = "Shape" };
            shapeSOField.RegisterValueChangeCallback(evt =>
            {
                ShapeSO callbackShapeSO = evt.changedProperty.GetUnderlyingValue() as ShapeSO;

                if (callbackShapeSO == null)
                    return;

                if (callbackShapeSO.Shape == openingScriptableObject.Opening.Shape)
                    return;

                openingScriptableObject.Opening.Shape = callbackShapeSO.Shape;

            });
            PropertyField heightField = new PropertyField(height);
            PropertyField widthField = new PropertyField(width);
            PropertyField angleField = new PropertyField(angle);
            PropertyField positionField = new PropertyField(position);
            #endregion

            #region Add To Root
            root.Add(shapeSOField);
            root.Add(heightField);
            root.Add(widthField);
            root.Add(angleField);
            root.Add(positionField);
            #endregion

            return root;
        }
    }
}
