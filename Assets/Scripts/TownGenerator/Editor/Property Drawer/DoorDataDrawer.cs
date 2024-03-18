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

            DoorData[] doors = GetDataFromBuildable(buildable);

            foreach (DoorData door in doors)
            {
                door.HingeData.Scale = scale;
            }

            Build(buildable);
        });
        hingePointField.RegisterValueChangeCallback(evt =>
        {
            RelativePosition relativePosition = evt.changedProperty.GetEnumValue<RelativePosition>();

            if (relativePosition == m_PreviousData.HingeData.RelativePosition)
                return;

            m_PreviousData.HingeData.RelativePosition = relativePosition;

            DoorData[] doors = GetDataFromBuildable(buildable);

            foreach (DoorData door in doors)
            {
                door.HingeData.RelativePosition = relativePosition;
            }

            Build(buildable);
        });
        hingeOffsetField.RegisterValueChangeCallback(evt =>
        {
            Vector3 offset = evt.changedProperty.vector3Value;

            if (offset == m_PreviousData.HingeData.PositionOffset)
                return;

            m_PreviousData.HingeData.PositionOffset = offset;

            DoorData[] doors = GetDataFromBuildable(buildable);

            foreach (DoorData door in doors)
            {
                door.HingeData.PositionOffset = offset;
            }

            Build(buildable);
        });
        hingeEulerAngleField.RegisterValueChangeCallback(evt =>
        {
            Vector3 euler = evt.changedProperty.vector3Value;

            if (euler == m_PreviousData.HingeData.EulerAngle)
                return;

            m_PreviousData.HingeData.EulerAngle = euler;

            DoorData[] doors = GetDataFromBuildable(buildable);

            foreach (DoorData door in doors)
            {
                door.HingeData.EulerAngle = euler;
            }

            Build(buildable);
        });
        #endregion

        if (buildable is Door)
        {
            PropertyField activeElementsField = new PropertyField(props.ActiveElements);
            activeElementsField.BindProperty(props.ActiveElements);
            activeElementsField.RegisterValueChangeCallback(evt =>
            {
                DoorElement activeElements = evt.changedProperty.GetEnumValue<DoorElement>();

                if (activeElements == m_PreviousData.ActiveElements)
                    return;

                m_PreviousData.ActiveElements = activeElements;

                bool isDoorActive = activeElements.IsElementActive(DoorElement.Door);

                scaleField.SetEnabled(isDoorActive);
                hingeFoldout.SetEnabled(isDoorActive);

                buildable.Build();
            });

            content.Add(activeElementsField);
        }

        #region Add Fields to Container
        content.Add(scaleField);
        content.Add(hingeFoldout);
        hingeFoldout.Add(hingePointField);
        hingeFoldout.Add(hingeOffsetField);
        hingeFoldout.Add(hingeEulerAngleField);
        #endregion

        return content;
    }
    private DoorData[] GetDataFromBuildable(IBuildable buildable)
    {
        DoorData[] dataset = new DoorData[0];

        switch (buildable)
        {
            case Wall:
                {
                    // TODO: instead of the first section index, get the one currently selected in the wall inspector.
                    Wall wall = buildable as Wall;
                    WallSectionData wallSection = wall.Data.Sections[0];

                    switch (wallSection.WallElement)
                    {
                        case WallElement.Doorway:
                            dataset = wallSection.Doorway.Doors;
                            break;
                        case WallElement.Archway:
                            dataset = wallSection.Archway.Doors;
                            break;
                    }

                }
                break;
            case WallSection:
                {
                    WallSection wallSection = buildable as WallSection;

                    switch (wallSection.Data.WallElement)
                    {
                        case WallElement.Doorway:
                            dataset = wallSection.Data.Doorway.Doors;
                            break;
                        case WallElement.Archway:
                            dataset = wallSection.Data.Archway.Doors;
                            break;
                    }
                }
                break;
            case Door:
                {
                    Door door = buildable as Door;
                    dataset = new DoorData[] { door.Data };
                }
                break;
        }

        return dataset;
    }
    private void Build(IBuildable buildable)
    {
        switch (buildable)
        {
            case Wall:
                // TODO the the wall section that is selected in the inspector & do the section build case.
                break;
            case WallSection:
                {
                    WallSection section = buildable as WallSection;
                    section.BuildChildren();
                }
                break;
            case Door:
                buildable.Build();
                break;
        }
    }

}
