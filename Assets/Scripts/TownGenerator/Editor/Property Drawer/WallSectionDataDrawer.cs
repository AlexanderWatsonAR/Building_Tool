using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.Rendering;
using Unity.VisualScripting;
using System.Linq;

[CustomPropertyDrawer(typeof(WallSectionData))]
public class WallSectionDataDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        VisualElement container = new VisualElement();

        WallSectionSerializedProperties props = new WallSectionSerializedProperties(data);

        EnumField wallElementField = new EnumField(props.WallElement.GetEnumValue<WallElement>()) { label = "Wall Element"};
        wallElementField.BindProperty(props.WallElement);
        container.Add(wallElementField);
        
        IBuildable buildable = data.serializedObject.targetObject as IBuildable;

        VisualElement wallElementContainer = new VisualElement();

        wallElementField.RegisterValueChangedCallback(evt =>
        {
            if (evt == null)
                return;

            if (evt.previousValue == null && evt.newValue == null)
                return;

            if (evt.newValue == evt.previousValue)
                return;

            wallElementContainer.Clear();

            WallElement wallElement = evt.newValue == null ? (WallElement)evt.previousValue : (WallElement)evt.newValue;

            switch (wallElement)
            {
                case WallElement.Wall:
                    break;
                case WallElement.Doorway:
                    {
                        Foldout gridFoldout = new Foldout() { text = "Grid" };

                        PropertyField cols = new PropertyField() { label = "Columns" };
                        cols.BindProperty(props.DoorColumns);
                        cols.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;

                            int columns = props.DoorColumns.intValue;
                            int rows = props.DoorRows.intValue;
                            int size = columns * rows;

                            if (size != props.Doors.arraySize)
                            {
                                props.Doors.SetUnderlyingValue(new DoorData[size]);
                                buildable.Build();
                            }
                        });

                        PropertyField rows = new PropertyField() { label = "Rows" };
                        rows.BindProperty(props.DoorRows);
                        rows.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;

                            int columns = props.DoorColumns.intValue;
                            int rows = props.DoorRows.intValue;
                            int size = columns * rows;

                            if (size != props.Doors.arraySize)
                            {
                                props.Doors.SetUnderlyingValue(new DoorData[size]);
                                buildable.Build();
                            }
                        });

                        gridFoldout.Add(cols);
                        gridFoldout.Add(rows);
                        wallElementContainer.Add(gridFoldout);

                        Foldout sizePosFold = new Foldout() { text = "Size & Position" };
                        wallElementContainer.Add(sizePosFold);

                        PropertyField offsetField = new PropertyField(props.DoorOffset) { label = "Offset"};
                        offsetField.BindProperty(props.DoorOffset);
                        offsetField.RegisterValueChangeCallback(evt =>
                        {
                            WallSection section = buildable as WallSection;
                            IList<IList<Vector3>> holePoints = section.CalculateDoorway();
                            
                            foreach(DoorData doorData in section.Data.Doors)
                            {
                                doorData.ControlPoints = holePoints[doorData.ID].ToArray();
                            }

                            section.Build();
                        });

                        PropertyField heightField = new PropertyField(props.DoorHeight) { label = "Height"};
                        heightField.BindProperty(props.DoorHeight);
                        heightField.RegisterValueChangeCallback(evt =>
                        {
                            WallSection section = buildable as WallSection;
                            IList<IList<Vector3>> holePoints = section.CalculateDoorway();

                            foreach (DoorData doorData in section.Data.Doors)
                            {
                                doorData.ControlPoints = holePoints[doorData.ID].ToArray();
                            }

                            section.Build();

                        });

                        PropertyField widthField = new PropertyField(props.DoorWidth) { label = "Width"};
                        widthField.BindProperty(props.DoorWidth);
                        widthField.RegisterValueChangeCallback(evt =>
                        {
                            WallSection section = buildable as WallSection;
                            IList<IList<Vector3>> holePoints = section.CalculateDoorway();

                            foreach (DoorData doorData in section.Data.Doors)
                            {
                                doorData.ControlPoints = holePoints[doorData.ID].ToArray();
                            }

                            section.Build();

                        });

                        sizePosFold.Add(offsetField);
                        sizePosFold.Add(heightField);
                        sizePosFold.Add(widthField);

                        Foldout doorFoldout = new Foldout() { text = "Door" };
                        wallElementContainer.Add(doorFoldout);

                        PropertyField activeDoorwayElements = new PropertyField(props.DoorwayElement) { label = "Active Elements"};
                        activeDoorwayElements.BindProperty(props.DoorwayElement);

                        PropertyField doorDataField = new PropertyField(props.DoorData);
                        doorDataField.BindProperty(props.DoorData);

                        Foldout frameFoldout = new Foldout() { text = "Frame" };

                        PropertyField doorFrameDepthField = new PropertyField(props.DoorFrameDepth) { label = "Depth" };
                        doorFrameDepthField.BindProperty(props.DoorFrameDepth);
                        doorFrameDepthField.RegisterValueChangeCallback(evt => buildable.Build());

                        PropertyField doorFrameScaleField = new PropertyField(props.DoorFrameScale) { label = "Scale" };
                        doorFrameScaleField.BindProperty(props.DoorFrameScale);
                        doorFrameScaleField.RegisterValueChangeCallback(evt => buildable.Build());

                        doorFoldout.Add(activeDoorwayElements);
                        doorFoldout.Add(doorDataField);
                        doorFoldout.Add(frameFoldout);

                        frameFoldout.Add(doorFrameDepthField);
                        frameFoldout.Add(doorFrameScaleField);

                    }
                    break;
                case WallElement.Archway:
                    break;
                case WallElement.Window:
                    {
                        Foldout gridFoldout = new Foldout() { text = "Grid" };

                        PropertyField cols = new PropertyField() { label = "Columns" };
                        cols.BindProperty(props.WindowColumns);
                        cols.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;

                            int columns = props.WindowColumns.intValue;
                            int rows = props.WindowRows.intValue;
                            int size = columns * rows;

                            if (size != props.Windows.arraySize)
                            {
                                props.Windows.SetUnderlyingValue(new WindowData[size]);
                                buildable.Build();
                            }
                        });

                        PropertyField rows = new PropertyField() { label = "Rows" };
                        rows.BindProperty(props.WindowRows);
                        rows.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;

                            int columns = props.WindowColumns.intValue;
                            int rows = props.WindowRows.intValue;
                            int size = columns * rows;

                            if (size != props.Windows.arraySize)
                            {
                                props.Windows.SetUnderlyingValue(new WindowData[size]);
                                buildable.Build();
                            }
                        });

                        gridFoldout.Add(cols);
                        gridFoldout.Add(rows);
                        wallElementContainer.Add(gridFoldout);

                        Foldout shapeFold = new Foldout() { text = "Shape" };
                        wallElementContainer.Add(shapeFold);

                        PropertyField sides = new PropertyField() { label = "Sides"};
                        sides.BindProperty(props.WindowSides);
                        sides.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;

                            buildable.Build();
                        });
                        shapeFold.Add(sides);

                        PropertyField height = new PropertyField() { label = "Height"};
                        height.BindProperty(props.WindowHeight);
                        height.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;

                            buildable.Build();
                        });
                        shapeFold.Add(height);

                        PropertyField width = new PropertyField() { label = "Width"};
                        width.BindProperty(props.WindowWidth);
                        width.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;

                                buildable.Build();
                        });
                        shapeFold.Add(width);

                        PropertyField angle = new PropertyField() { label = "Angle"};
                        angle.BindProperty(props.WindowAngle);
                        angle.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;
                                
                            
                            buildable.Build();
                            
                        });

                        shapeFold.Add(angle);

                        Foldout windowFoldout = new Foldout() { text = "Window" };
                        wallElementContainer.Add(windowFoldout);

                        PropertyField windowDataField = new PropertyField(props.WindowData);
                        windowDataField.BindProperty(props.WindowData);
                        windowFoldout.Add(windowDataField);
                    }
                    break;
                case WallElement.Extension:
                    break;
                case WallElement.Empty:
                    break;
            }

            buildable.Build();
        });


        container.Add(wallElementContainer);

        return container;
    }

}
