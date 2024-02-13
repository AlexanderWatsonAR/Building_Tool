using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Rendering;
using static System.Collections.Specialized.BitVector32;

[CustomPropertyDrawer(typeof(DoorData))]
public class DoorDataDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        VisualElement content = new VisualElement() { name = "Door Data Content"};

        IBuildable buildable = data.serializedObject.targetObject as IBuildable;

        DoorDataSerializedProperties props = new DoorDataSerializedProperties(data);

        PropertyField scaleField = new PropertyField(props.Scale);
        scaleField.BindProperty(props.Scale);
        scaleField.RegisterValueChangeCallback(evt => 
        {
            DoorData[] doors = GetDoorDataFromBuildable(buildable);

            foreach(DoorData door in doors)
            {
                door.Scale = evt.changedProperty.floatValue;
            }

            Build(buildable);
        });

        Foldout hingeFoldout = new Foldout() { text = "Hinge" };

        PropertyField hingePointField = new PropertyField(props.HingePoint) { label = "Position" };
        hingePointField.BindProperty(props.HingePoint);
        hingePointField.RegisterValueChangeCallback(evt => 
        {
            DoorData[] doors = GetDoorDataFromBuildable(buildable);

            foreach (DoorData door in doors)
            {
                door.HingePoint = evt.changedProperty.GetEnumValue<TransformPoint>();
            }

            Build(buildable);
        });

        PropertyField hingeOffsetField = new PropertyField(props.HingeOffset) { label = "Offset" };
        hingeOffsetField.BindProperty(props.HingeOffset);
        hingeOffsetField.RegisterValueChangeCallback(evt =>
        {
            DoorData[] doors = GetDoorDataFromBuildable(buildable);

            foreach (DoorData door in doors)
            {
                door.HingeOffset = evt.changedProperty.vector3Value;
            }

            Build(buildable);
        });

        PropertyField hingeEulerAnglesField = new PropertyField(props.HingeEulerAngle) { label = "Euler Angles"};
        hingeEulerAnglesField.BindProperty(props.HingeEulerAngle);
        hingeEulerAnglesField.RegisterValueChangeCallback(evt =>
        {
            DoorData[] doors = GetDoorDataFromBuildable(buildable);

            foreach (DoorData door in doors)
            {
                door.HingeEulerAngles = evt.changedProperty.vector3Value;
            }

            Build(buildable);
        });

        if (buildable is Door)
        {
            PropertyField activeElementsField = new PropertyField(props.ActiveElements);
            activeElementsField.BindProperty(props.ActiveElements);
            activeElementsField.RegisterValueChangeCallback(evt =>
            {
                bool isDoorActive = evt.changedProperty.GetEnumValue<DoorElement>().IsElementActive(DoorElement.Door);

                scaleField.SetEnabled(isDoorActive);
                hingeFoldout.SetEnabled(isDoorActive);

                if (evt == null)
                    return;

                buildable.Build();
            });

            content.Add(activeElementsField);
        }


        content.Add(scaleField);
        content.Add(hingeFoldout);
        hingeFoldout.Add(hingePointField);
        hingeFoldout.Add(hingeOffsetField);
        hingeFoldout.Add(hingeEulerAnglesField);

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
                            dataset = wallSection.Doors;
                            break;
                        case WallElement.Archway:
                            dataset = wallSection.ArchDoors;
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
                            dataset = wallSection.Data.Doors;
                            break;
                        case WallElement.Archway:
                            dataset = wallSection.Data.ArchDoors;
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

                    for (int i = 0; i < section.transform.childCount; i++)
                    {
                        if (section.transform.GetChild(i).TryGetComponent(out Door door))
                        {
                            door.Build();
                        }
                    }
                }
                break;
            case Door:
                buildable.Build();
                break;
        }
    }

}
