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

        DoorDataSerializedProperties props = new DoorDataSerializedProperties(data);

        PropertyField scaleField = new PropertyField(props.Scale);
        scaleField.BindProperty(props.Scale);
        scaleField.RegisterValueChangeCallback(evt => 
        {
            if (buildable is WallSection)
            {
                WallSection section = buildable as WallSection;

                switch (section.Data.WallElement)
                {
                    case WallElement.Doorway:
                        foreach (DoorData data in section.Data.Doors)
                        {
                            data.Scale = evt.changedProperty.floatValue;
                        }
                        break;
                    case WallElement.Archway:
                        foreach (DoorData data in section.Data.ArchDoors)
                        {
                            data.Scale = evt.changedProperty.floatValue;
                        }
                        break;
                }
            }

            buildable.Build();
        });

        Foldout hingeFoldout = new Foldout() { text = "Hinge" };

        PropertyField hingePointField = new PropertyField(props.HingePoint) { label = "Position" };
        hingePointField.BindProperty(props.HingePoint);
        hingePointField.RegisterValueChangeCallback(evt => 
        {
            if(buildable is WallSection)
            {
                WallSection section = buildable as WallSection;

                switch (section.Data.WallElement)
                {
                    case WallElement.Doorway:
                        foreach(DoorData data in section.Data.Doors)
                        {
                            data.HingePoint = evt.changedProperty.GetEnumValue<TransformPoint>();
                        }
                        break;
                    case WallElement.Archway:
                        foreach (DoorData data in section.Data.ArchDoors)
                        {
                            data.HingePoint = evt.changedProperty.GetEnumValue<TransformPoint>();
                        }
                        break;
                }
            }

            buildable.Build();
        });

        PropertyField hingeOffsetField = new PropertyField(props.HingeOffset) { label = "Offset" };
        hingeOffsetField.BindProperty(props.HingeOffset);
        hingeOffsetField.RegisterValueChangeCallback(evt =>
        {
            if (buildable is WallSection)
            {
                WallSection section = buildable as WallSection;

                switch (section.Data.WallElement)
                {
                    case WallElement.Doorway:
                        foreach (DoorData data in section.Data.Doors)
                        {
                            data.HingeOffset = evt.changedProperty.vector3Value;
                        }
                        break;
                    case WallElement.Archway:
                        foreach (DoorData data in section.Data.ArchDoors)
                        {
                            data.HingeOffset = evt.changedProperty.vector3Value;
                        }
                        break;
                }
            }

            buildable.Build();
        });

        PropertyField hingeEulerAnglesField = new PropertyField(props.HingeEulerAngle) { label = "Euler Angles"};
        hingeEulerAnglesField.BindProperty(props.HingeEulerAngle);
        hingeEulerAnglesField.RegisterValueChangeCallback(evt =>
        {
            if (buildable is WallSection)
            {
                WallSection section = buildable as WallSection;

                switch (section.Data.WallElement)
                {
                    case WallElement.Doorway:
                        foreach (DoorData data in section.Data.Doors)
                        {
                            data.HingeEulerAngles = evt.changedProperty.vector3Value;
                        }
                        break;
                    case WallElement.Archway:
                        foreach (DoorData data in section.Data.ArchDoors)
                        {
                            data.HingeEulerAngles = evt.changedProperty.vector3Value;
                        }
                        break;
                }
            }

            buildable.Build();
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


        //else if (buildable is WallSection)
        //{
        //    SerializedProperty sectionData = data.serializedObject.FindProperty("m_Data");
        //    SerializedProperty activeElementsDoorway = sectionData.FindPropertyRelative("m_ActiveDoorwayElements");
        //    SerializedProperty activeElementsArchway = sectionData.FindPropertyRelative("m_ActiveArchDoorElements");

        //    WallSection section = buildable as WallSection;

        //    switch (section.Data.WallElement)
        //    {
        //        case WallElement.Doorway:
        //            {
        //                PropertyField activeElementsDoorwayField = new PropertyField(activeElementsDoorway) { label = "Active Elements" };
        //                activeElementsDoorwayField.BindProperty(activeElementsDoorway);
        //                activeElementsDoorwayField.RegisterValueChangeCallback(evt =>
        //                {
        //                    bool isDoorActive = evt.changedProperty.GetEnumValue<DoorElement>().IsElementActive(DoorElement.Door);

        //                    scaleField.SetEnabled(isDoorActive);
        //                    hingePointField.SetEnabled(isDoorActive);
        //                    hingeOffsetField.SetEnabled(isDoorActive);
        //                    hingeEulerAnglesField.SetEnabled(isDoorActive);

        //                    buildable.Build();
        //                });

        //                content.Add(activeElementsDoorwayField);
        //            }
        //            break;
        //        case WallElement.Archway:
        //            {
        //                PropertyField activeElementsArchwayField = new PropertyField(activeElementsArchway) { label = "Active Elements" };
        //                activeElementsArchwayField.BindProperty(activeElementsArchway);
        //                activeElementsArchwayField.RegisterValueChangeCallback(evt =>
        //                {
        //                    bool isDoorActive = evt.changedProperty.GetEnumValue<DoorElement>().IsElementActive(DoorElement.Door);

        //                    scaleField.SetEnabled(isDoorActive);
        //                    hingePointField.SetEnabled(isDoorActive);
        //                    hingeOffsetField.SetEnabled(isDoorActive);
        //                    hingeEulerAnglesField.SetEnabled(isDoorActive);

        //                    buildable.Build();
        //                });

        //                content.Add(activeElementsArchwayField);
        //            }
        //            break;
        //    }

        //}

        content.Add(scaleField);
        content.Add(hingeFoldout);
        hingeFoldout.Add(hingePointField);
        hingeFoldout.Add(hingeOffsetField);
        hingeFoldout.Add(hingeEulerAnglesField);

        return content;
    }

}
