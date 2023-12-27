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

        EnumField wallElementField = new EnumField(props.WallElement.GetEnumValue<WallElement>()) { label = "Wall Element" };
        wallElementField.BindProperty(props.WallElement);
        container.Add(wallElementField);

        IBuildable buildable = data.serializedObject.targetObject as IBuildable;

        VisualElement wallElementContainer = new VisualElement();

        wallElementField.RegisterValueChangedCallback(evt =>
        {
            if (evt == null)
                return;

            if (evt.previousValue == null || evt.newValue == null)
                return;

            if (evt.newValue == evt.previousValue)
                return;

            buildable.Demolish();

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

                        PropertyField offsetField = new PropertyField(props.DoorOffset) { label = "Offset" };
                        offsetField.BindProperty(props.DoorOffset);
                        offsetField.RegisterValueChangeCallback(evt =>
                        {
                            WallSectionData sectionData = data.GetUnderlyingValue() as WallSectionData;
                            IList<IList<Vector3>> holePoints = WallSection.CalculateDoorway(sectionData);

                            foreach (DoorData doorData in sectionData.Doors)
                            {
                                doorData.ControlPoints = holePoints[doorData.ID].ToArray();
                            }

                            buildable.Build();
                        });

                        PropertyField heightField = new PropertyField(props.DoorHeight) { label = "Height" };
                        heightField.BindProperty(props.DoorHeight);
                        heightField.RegisterValueChangeCallback(evt =>
                        {
                            WallSectionData sectionData = data.GetUnderlyingValue() as WallSectionData;
                            IList<IList<Vector3>> holePoints = WallSection.CalculateDoorway(sectionData);

                            foreach (DoorData doorData in sectionData.Doors)
                            {
                                doorData.ControlPoints = holePoints[doorData.ID].ToArray();
                            }

                            buildable.Build();
                        });

                        PropertyField widthField = new PropertyField(props.DoorWidth) { label = "Width" };
                        widthField.BindProperty(props.DoorWidth);
                        widthField.RegisterValueChangeCallback(evt =>
                        {
                            WallSectionData sectionData = data.GetUnderlyingValue() as WallSectionData;
                            IList<IList<Vector3>> holePoints = WallSection.CalculateDoorway(sectionData);

                            foreach (DoorData doorData in sectionData.Doors)
                            {
                                doorData.ControlPoints = holePoints[doorData.ID].ToArray();
                            }

                            buildable.Build();
                        });

                        sizePosFold.Add(offsetField);
                        sizePosFold.Add(heightField);
                        sizePosFold.Add(widthField);

                        Foldout doorFoldout = new Foldout() { text = "Door" };
                        wallElementContainer.Add(doorFoldout);

                        PropertyField doorDataField = new PropertyField(props.DoorData);
                        doorDataField.BindProperty(props.DoorData);

                        Foldout frameFoldout = new Foldout() { text = "Frame" };

                        PropertyField doorFrameDepthField = new PropertyField(props.DoorFrameDepth) { label = "Depth" };
                        doorFrameDepthField.BindProperty(props.DoorFrameDepth);
                        doorFrameDepthField.RegisterValueChangeCallback(evt => buildable.Build());

                        PropertyField doorFrameScaleField = new PropertyField(props.DoorFrameScale) { label = "Scale" };
                        doorFrameScaleField.BindProperty(props.DoorFrameScale);
                        doorFrameScaleField.RegisterValueChangeCallback(evt => buildable.Build());

                        PropertyField activeDoorwayElements = new PropertyField(props.DoorwayElement) { label = "Active Elements" };
                        activeDoorwayElements.BindProperty(props.DoorwayElement);
                        activeDoorwayElements.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;

                            DoorData[] doors = props.Doors.GetUnderlyingValue() as DoorData[];

                            DoorwayElement doorwayElement = evt.changedProperty.GetEnumValue<DoorwayElement>();

                            foreach (DoorData data in doors)
                            {
                                data.ActiveElements = doorwayElement.ToDoorElement();
                            }

                            bool isDoorActive = evt.changedProperty.GetEnumValue<DoorwayElement>().IsElementActive(DoorwayElement.Door);
                            bool isFrameActive = evt.changedProperty.GetEnumValue<DoorwayElement>().IsElementActive(DoorwayElement.Frame);

                            doorDataField.SetEnabled(isDoorActive);
                            frameFoldout.SetEnabled(isFrameActive);

                            buildable.Build();
                        });

                        bool isDoorActive = props.DoorwayElement.GetEnumValue<DoorwayElement>().IsElementActive(DoorwayElement.Door);
                        bool isFrameActive = props.DoorwayElement.GetEnumValue<DoorwayElement>().IsElementActive(DoorwayElement.Frame);

                        doorDataField.SetEnabled(isDoorActive);
                        frameFoldout.SetEnabled(isFrameActive);

                        doorFoldout.Add(activeDoorwayElements);
                        doorFoldout.Add(doorDataField);
                        doorFoldout.Add(frameFoldout);

                        frameFoldout.Add(doorFrameDepthField);
                        frameFoldout.Add(doorFrameScaleField);

                    }
                    break;
                case WallElement.Archway:
                    {
                        Foldout gridFoldout = new Foldout() { text = "Grid" };

                        PropertyField cols = new PropertyField() { label = "Columns" };
                        cols.BindProperty(props.ArchColumns);
                        cols.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;

                            int columns = props.ArchColumns.intValue;
                            int rows = props.ArchRows.intValue;
                            int size = columns * rows;

                            if (size != props.ArchDoors.arraySize)
                            {
                                props.ArchDoors.SetUnderlyingValue(new DoorData[size]);
                                buildable.Build();
                            }
                        });

                        PropertyField rows = new PropertyField() { label = "Rows" };
                        rows.BindProperty(props.ArchRows);
                        rows.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;

                            int columns = props.ArchColumns.intValue;
                            int rows = props.ArchRows.intValue;
                            int size = columns * rows;

                            if (size != props.ArchDoors.arraySize)
                            {
                                props.ArchDoors.SetUnderlyingValue(new DoorData[size]);
                                buildable.Build();
                            }
                        });

                        gridFoldout.Add(cols);
                        gridFoldout.Add(rows);
                        wallElementContainer.Add(gridFoldout);

                        Foldout sizePosFold = new Foldout() { text = "Size, Shape & Position" };
                        wallElementContainer.Add(sizePosFold);

                        PropertyField offsetField = new PropertyField(props.ArchOffset) { label = "Offset", tooltip = "Position offset from the centre" };
                        offsetField.BindProperty(props.ArchOffset);
                        offsetField.RegisterValueChangeCallback(evt =>
                        {
                            WallSectionData sectionData = data.GetUnderlyingValue() as WallSectionData;

                            IList<IList<Vector3>> holePoints = WallSection.CalculateArchway(sectionData);

                            foreach (DoorData archData in sectionData.ArchDoors)
                            {
                                archData.ControlPoints = holePoints[archData.ID].ToArray();
                            }

                            buildable.Build();
                        });

                        PropertyField archHeightField = new PropertyField(props.ArchHeight) { label = "Arch Height" };
                        archHeightField.BindProperty(props.ArchHeight);
                        archHeightField.RegisterValueChangeCallback(evt =>
                        {
                            WallSectionData sectionData = data.GetUnderlyingValue() as WallSectionData;

                            IList<IList<Vector3>> holePoints = WallSection.CalculateArchway(sectionData);

                            foreach (DoorData archData in sectionData.ArchDoors)
                            {
                                archData.ControlPoints = holePoints[archData.ID].ToArray();
                            }

                            buildable.Build();

                        });

                        PropertyField archSidesField = new PropertyField(props.ArchSides) { label = "Arch Sides", tooltip = " Number of points on the Arch" };
                        archSidesField.BindProperty(props.ArchSides);
                        archSidesField.RegisterValueChangeCallback(evt =>
                        {
                            WallSectionData sectionData = data.GetUnderlyingValue() as WallSectionData;

                            IList<IList<Vector3>> holePoints = WallSection.CalculateArchway(sectionData);

                            foreach (DoorData archData in sectionData.ArchDoors)
                            {
                                archData.ControlPoints = holePoints[archData.ID].ToArray();
                            }

                            buildable.Build();

                        });

                        PropertyField heightField = new PropertyField(props.ArchDoorHeight) { label = "Height" };
                        heightField.BindProperty(props.ArchDoorHeight);
                        heightField.RegisterValueChangeCallback(evt =>
                        {
                            WallSectionData sectionData = data.GetUnderlyingValue() as WallSectionData;

                            IList<IList<Vector3>> holePoints = WallSection.CalculateArchway(sectionData);

                            foreach (DoorData archData in sectionData.ArchDoors)
                            {
                                archData.ControlPoints = holePoints[archData.ID].ToArray();
                            }

                            buildable.Build();

                        });

                        PropertyField widthField = new PropertyField(props.ArchWidth) { label = "Width" };
                        widthField.BindProperty(props.ArchWidth);
                        widthField.RegisterValueChangeCallback(evt =>
                        {
                            WallSectionData sectionData = data.GetUnderlyingValue() as WallSectionData;

                            IList<IList<Vector3>> holePoints = WallSection.CalculateArchway(sectionData);

                            foreach (DoorData doorData in sectionData.ArchDoors)
                            {
                                doorData.ControlPoints = holePoints[doorData.ID].ToArray();
                            }

                            buildable.Build();

                        });

                        sizePosFold.Add(offsetField);
                        sizePosFold.Add(archHeightField);
                        sizePosFold.Add(archSidesField);
                        sizePosFold.Add(heightField);
                        sizePosFold.Add(widthField);

                        Foldout doorFoldout = new Foldout() { text = "Door" };
                        wallElementContainer.Add(doorFoldout);

                        PropertyField archDataField = new PropertyField(props.ArchDoorData);
                        archDataField.BindProperty(props.DoorData);

                        Foldout frameFoldout = new Foldout() { text = "Frame" };

                        PropertyField doorFrameDepthField = new PropertyField(props.ArchDoorFrameDepth) { label = "Depth" };
                        doorFrameDepthField.BindProperty(props.ArchDoorFrameDepth);
                        doorFrameDepthField.RegisterValueChangeCallback(evt => buildable.Build());

                        PropertyField doorFrameScaleField = new PropertyField(props.ArchDoorFrameScale) { label = "Scale" };
                        doorFrameScaleField.BindProperty(props.ArchDoorFrameScale);
                        doorFrameScaleField.RegisterValueChangeCallback(evt => buildable.Build());

                        PropertyField activeDoorwayElements = new PropertyField(props.ArchwayElement) { label = "Active Elements" };
                        activeDoorwayElements.BindProperty(props.ArchwayElement);
                        activeDoorwayElements.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;

                            DoorData[] doors = props.ArchDoors.GetUnderlyingValue() as DoorData[];

                            DoorwayElement archwayElement = evt.changedProperty.GetEnumValue<DoorwayElement>();

                            foreach (DoorData data in doors)
                            {
                                data.ActiveElements = archwayElement.ToDoorElement();
                            }

                            bool isDoorActive = evt.changedProperty.GetEnumValue<DoorwayElement>().IsElementActive(DoorwayElement.Door);
                            bool isFrameActive = evt.changedProperty.GetEnumValue<DoorwayElement>().IsElementActive(DoorwayElement.Frame);

                            archDataField.SetEnabled(isDoorActive);
                            frameFoldout.SetEnabled(isFrameActive);

                            buildable.Build();
                        });

                        bool isDoorActive = props.ArchwayElement.GetEnumValue<DoorwayElement>().IsElementActive(DoorwayElement.Door);
                        bool isFrameActive = props.ArchwayElement.GetEnumValue<DoorwayElement>().IsElementActive(DoorwayElement.Frame);

                        archDataField.SetEnabled(isDoorActive);
                        frameFoldout.SetEnabled(isFrameActive);

                        doorFoldout.Add(activeDoorwayElements);
                        doorFoldout.Add(archDataField);
                        doorFoldout.Add(frameFoldout);

                        frameFoldout.Add(doorFrameDepthField);
                        frameFoldout.Add(doorFrameScaleField);

                    }
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
                                buildable.Demolish();
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
                                buildable.Demolish();
                                props.Windows.SetUnderlyingValue(new WindowData[size]);
                                buildable.Build();
                            }
                        });

                        gridFoldout.Add(cols);
                        gridFoldout.Add(rows);
                        wallElementContainer.Add(gridFoldout);

                        Foldout shapeFold = new Foldout() { text = "Shape" };
                        wallElementContainer.Add(shapeFold);

                        PropertyField sides = new PropertyField() { label = "Sides" };
                        sides.BindProperty(props.WindowSides);
                        sides.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;

                            buildable.Demolish();

                            WallSectionData sectionData = data.GetUnderlyingValue() as WallSectionData;

                            IList<IList<Vector3>> holePoints = WallSection.CalculateWindow(sectionData);

                            foreach (WindowData winData in sectionData.Windows)
                            {
                                winData.ControlPoints = holePoints[winData.ID].ToArray();
                            }

                            buildable.Build();
                        });
                        shapeFold.Add(sides);

                        PropertyField height = new PropertyField() { label = "Height" };
                        height.BindProperty(props.WindowHeight);
                        height.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;

                            buildable.Demolish();

                            WallSectionData sectionData = data.GetUnderlyingValue() as WallSectionData;

                            IList<IList<Vector3>> holePoints = WallSection.CalculateWindow(sectionData);

                            foreach (WindowData winData in sectionData.Windows)
                            {
                                winData.ControlPoints = holePoints[winData.ID].ToArray();
                            }

                            buildable.Build();
                        });

                        shapeFold.Add(height);

                        PropertyField width = new PropertyField() { label = "Width" };
                        width.BindProperty(props.WindowWidth);
                        width.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;

                            buildable.Demolish();

                            WallSectionData sectionData = data.GetUnderlyingValue() as WallSectionData;

                            IList<IList<Vector3>> holePoints = WallSection.CalculateWindow(sectionData);

                            foreach (WindowData winData in sectionData.Windows)
                            {
                                winData.ControlPoints = holePoints[winData.ID].ToArray();
                            }

                            buildable.Build();
                        });
                        shapeFold.Add(width);

                        PropertyField angle = new PropertyField() { label = "Angle" };
                        angle.BindProperty(props.WindowAngle);
                        angle.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;

                            buildable.Demolish();

                            WallSectionData sectionData = data.GetUnderlyingValue() as WallSectionData;

                            IList<IList<Vector3>> holePoints = WallSection.CalculateWindow(sectionData);

                            foreach (WindowData winData in sectionData.Windows)
                            {
                                winData.ControlPoints = holePoints[winData.ID].ToArray();
                            }

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
                    {
                        Foldout extensionFoldout = new Foldout() { text = "Extension" };
                        PropertyField extensionDistance = new PropertyField(props.ExtensionDistance);
                        extensionDistance.BindProperty(props.ExtensionDistance);
                        extensionDistance.RegisterValueChangeCallback(evt => buildable.Build());

                        PropertyField extensionHeight = new PropertyField(props.ExtensionHeight);
                        extensionHeight.BindProperty(props.ExtensionHeight);
                        extensionHeight.RegisterValueChangeCallback(evt => buildable.Build());

                        PropertyField extensionWidth = new PropertyField(props.ExtensionWidth);
                        extensionWidth.BindProperty(props.ExtensionWidth);
                        extensionWidth.RegisterValueChangeCallback(evt => buildable.Build());

                        Foldout storeyFoldout = new Foldout() { text = "Storey" };

                        PropertyField extensionStoreyData = new PropertyField();
                        extensionStoreyData.BindProperty(props.ExtensionStorey);

                        Foldout roofFoldout = new Foldout() { text = "Roof" };
                        PropertyField extensionRoofData = new PropertyField();
                        extensionRoofData.BindProperty(props.ExtensionRoof);

                        wallElementContainer.Add(extensionFoldout);
                        extensionFoldout.Add(extensionDistance);
                        extensionFoldout.Add(extensionHeight);
                        extensionFoldout.Add(extensionWidth);
                        extensionFoldout.Add(storeyFoldout);
                        storeyFoldout.Add(extensionStoreyData);
                        extensionFoldout.Add(roofFoldout);
                        roofFoldout.Add(extensionRoofData);
                    }
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
