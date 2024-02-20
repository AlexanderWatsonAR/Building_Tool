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
    [SerializeField] DoorData m_CurrentDoorData;
    [SerializeField] DoorData m_PreviousDoorData;

    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        VisualElement content = new VisualElement() { name = "Door Data Content"};

        IBuildable buildable = data.serializedObject.targetObject as IBuildable;

        DoorDataSerializedProperties props = new DoorDataSerializedProperties(data);

        m_CurrentDoorData = data.GetUnderlyingValue() as DoorData;
        m_PreviousDoorData = new DoorData(m_CurrentDoorData);

        #region Fields
        PropertyField scaleField = new PropertyField(props.Scale);
        Foldout hingeFoldout = new Foldout() { text = "Hinge" };
        PropertyField hingePointField = new PropertyField(props.HingePoint) { label = "Position" };
        PropertyField hingeOffsetField = new PropertyField(props.HingeOffset) { label = "Offset" };
        PropertyField hingeEulerAnglesField = new PropertyField(props.HingeEulerAngle) { label = "Euler Angles" };

        #endregion

        #region Binds
        scaleField.BindProperty(props.Scale);
        hingePointField.BindProperty(props.HingePoint);
        hingeOffsetField.BindProperty(props.HingeOffset);
        hingeEulerAnglesField.BindProperty(props.HingeEulerAngle);

        #endregion

        #region Register Value Change Callbacks
        scaleField.RegisterValueChangeCallback(evt =>
        {
            if (m_CurrentDoorData.Scale == m_PreviousDoorData.Scale)
                return;

            m_PreviousDoorData.Scale = m_CurrentDoorData.Scale;

            DoorData[] doors = GetDoorDataFromBuildable(buildable);

            foreach (DoorData door in doors)
            {
                door.Scale = evt.changedProperty.floatValue;
            }

            Build(buildable);
        });
        hingePointField.RegisterValueChangeCallback(evt =>
        {
            if (m_CurrentDoorData.HingePoint == m_PreviousDoorData.HingePoint)
                return;

            m_PreviousDoorData.HingePoint = m_CurrentDoorData.HingePoint;

            DoorData[] doors = GetDoorDataFromBuildable(buildable);

            foreach (DoorData door in doors)
            {
                door.HingePoint = evt.changedProperty.GetEnumValue<TransformPoint>();
            }

            Build(buildable);
        });
        hingeOffsetField.RegisterValueChangeCallback(evt =>
        {
            if (m_CurrentDoorData.HingeOffset == m_PreviousDoorData.HingeOffset)
                return;

            m_PreviousDoorData.HingeOffset = m_CurrentDoorData.HingeOffset;

            DoorData[] doors = GetDoorDataFromBuildable(buildable);

            foreach (DoorData door in doors)
            {
                door.HingeOffset = evt.changedProperty.vector3Value;
            }

            Build(buildable);
        });
        hingeEulerAnglesField.RegisterValueChangeCallback(evt =>
        {
            if (m_CurrentDoorData.HingeEulerAngles == m_PreviousDoorData.HingeEulerAngles)
                return;

            m_PreviousDoorData.HingeEulerAngles = m_CurrentDoorData.HingeEulerAngles;

            DoorData[] doors = GetDoorDataFromBuildable(buildable);

            foreach (DoorData door in doors)
            {
                door.HingeEulerAngles = evt.changedProperty.vector3Value;
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
                if (m_CurrentDoorData.ActiveElements == m_PreviousDoorData.ActiveElements)
                    return;

                m_PreviousDoorData.ActiveElements = m_CurrentDoorData.ActiveElements;

                bool isDoorActive = evt.changedProperty.GetEnumValue<DoorElement>().IsElementActive(DoorElement.Door);

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
        hingeFoldout.Add(hingeEulerAnglesField);
        #endregion

        return content;
    }


    private DoorData[] GetDoorDataFromBuildable(IBuildable buildable)
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
