using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Rendering;
using static System.Collections.Specialized.BitVector32;
using Unity.VisualScripting;

[CustomPropertyDrawer(typeof(DoorData))]
public class DoorDataDrawer : PropertyDrawer
{
    [SerializeField] DoorData m_PreviousData;

    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        VisualElement content = new VisualElement() { name = "Door Data Content"};

        IBuildable buildable = data.serializedObject.targetObject as IBuildable;

        DoorDataSerializedProperties props = new DoorDataSerializedProperties(data);

        DoorData current = data.GetUnderlyingValue() as DoorData;

        m_PreviousData = current.Clone() as DoorData;

        var hingeData = props.HingeData;

        #region Fields
        PropertyField scaleField = new PropertyField(hingeData.Scale); // this probably shouldn't be in hinge data.
        Foldout hingeFoldout = new Foldout() { text = "Hinge" };
        PropertyField hingePointField = new PropertyField(hingeData.RelativePosition) { label = "Position" };
        PropertyField hingeOffsetField = new PropertyField(hingeData.PositionOffset) { label = "Offset" };
        PropertyField hingeEulerAngleField = new PropertyField(hingeData.EulerAngle) { label = "Euler Angle" };

        #endregion

        #region Binds
        scaleField.BindProperty(hingeData.Scale);
        hingePointField.BindProperty(hingeData.RelativePosition);
        hingeOffsetField.BindProperty(hingeData.PositionOffset);
        hingeEulerAngleField.BindProperty(hingeData.EulerAngle);
        #endregion

        #region Register Value Change Callbacks
        scaleField.RegisterValueChangeCallback(evt =>
        {
            Vector3 scale = evt.changedProperty.vector3Value;

            if (scale == m_PreviousData.HingeData.Scale)
                return;

            m_PreviousData.HingeData.Scale = scale;
        });
        hingePointField.RegisterValueChangeCallback(evt =>
        {
            RelativePosition relativePosition = evt.changedProperty.GetEnumValue<RelativePosition>();

            if (relativePosition == m_PreviousData.HingeData.RelativePosition)
                return;

            m_PreviousData.HingeData.RelativePosition = relativePosition;
        });
        hingeOffsetField.RegisterValueChangeCallback(evt =>
        {
            Vector3 offset = evt.changedProperty.vector3Value;

            if (offset == m_PreviousData.HingeData.PositionOffset)
                return;

            m_PreviousData.HingeData.PositionOffset = offset;
        });
        hingeEulerAngleField.RegisterValueChangeCallback(evt =>
        {
            Vector3 euler = evt.changedProperty.vector3Value;

            if (euler == m_PreviousData.HingeData.EulerAngle)
                return;

            m_PreviousData.HingeData.EulerAngle = euler;
        });
        #endregion


        #region Add Fields to Container
        content.Add(scaleField);
        content.Add(hingeFoldout);
        hingeFoldout.Add(hingePointField);
        hingeFoldout.Add(hingeOffsetField);
        hingeFoldout.Add(hingeEulerAngleField);
        #endregion

        return content;
    }

}
