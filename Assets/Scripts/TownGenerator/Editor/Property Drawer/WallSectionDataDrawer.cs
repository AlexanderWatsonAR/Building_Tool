using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.Rendering;
using Unity.VisualScripting;
using System.Linq;
using static PlasticGui.LaunchDiffParameters;
using System.Runtime.Remoting.Messaging;

[CustomPropertyDrawer(typeof(WallSectionData))]
public class WallSectionDataDrawer : PropertyDrawer
{
    [SerializeField] WallSectionData m_PreviousSectionData; // This is a copy of data, used to determine if data values actually change.

    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        VisualElement container = new VisualElement();

        WallSectionDataSerializedProperties props = new WallSectionDataSerializedProperties(data);

        m_PreviousSectionData = new WallSectionData(data.GetUnderlyingValue() as WallSectionData);

        PropertyField wallElementField = new PropertyField(props.WallElement) { label = "Wall Element" };
        wallElementField.BindProperty(props.WallElement);
        container.Add(wallElementField);

        IBuildable buildable = data.serializedObject.targetObject as IBuildable;

        VisualElement wallElementContainer = new VisualElement();

        wallElementField.RegisterValueChangeCallback(evt =>
        {
            WallSectionData currentSectionData = GetWallSectionDataFromBuildable(buildable);

            if ((currentSectionData.WallElement == m_PreviousSectionData.WallElement) && wallElementContainer.childCount > 0)
                return;

            wallElementContainer.Clear();

            switch (currentSectionData.WallElement)
            {
                case WallElement.Wall:
                    break;
                case WallElement.Doorway:
                    {
                        var doorway = props.Doorway;
                        var door = props.Door;
                        var frame = props.Frame;

                        #region Fields
                        Foldout gridFoldout = new Foldout() { text = "Grid" };
                        PropertyField cols = new PropertyField(doorway.Columns, "Columns") { label = "Columns" };
                        PropertyField rows = new PropertyField(doorway.Rows, "Rows") { label = "Rows" };
                        Foldout sizePosFold = new Foldout() { text = "Size & Position" };
                        PropertyField offsetField = new PropertyField(doorway.PositionOffset, "Offset") { label = "Offset" };
                        PropertyField heightField = new PropertyField(doorway.Height, "Height") { label = "Height" };
                        PropertyField widthField = new PropertyField(doorway.Width, "Width") { label = "Width" };
                        Foldout doorFoldout = new Foldout() { text = "Door" };
                        PropertyField doorDataField = new PropertyField(door.Data);
                        Foldout frameFoldout = new Foldout() { text = "Frame" };
                        PropertyField doorFrameDepthField = new PropertyField(frame.Depth, "Depth") { label = "Depth" };
                        PropertyField doorFrameScaleField = new PropertyField(frame.Scale, "Scale") { label = "Scale" };
                        PropertyField activeDoorwayElements = new PropertyField(doorway.ActiveElements, "Active Elements") { label = "Active Elements" };
                        #endregion

                        #region Bind
                        // This seperate binding step will probably be redundant in later versions of unity.
                        cols.BindProperty(doorway.Columns);
                        rows.BindProperty(doorway.Rows);
                        offsetField.BindProperty(doorway.PositionOffset);
                        heightField.BindProperty(doorway.Height);
                        widthField.BindProperty(doorway.Width);
                        doorDataField.BindProperty(door.Data);
                        doorFrameDepthField.BindProperty(frame.Depth);
                        doorFrameScaleField.BindProperty(frame.Scale);
                        activeDoorwayElements.BindProperty(doorway.ActiveElements);
                        #endregion

                        #region Register Value Change Callbacks
                        // We are relying on unity triggering these callbacks when the user clicks on an object with a non-hidden & serialized wall section data.
                        cols.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.Doorway.Columns == m_PreviousSectionData.Doorway.Columns)
                                return;

                            m_PreviousSectionData.Doorway.Columns = currentSectionData.Doorway.Columns;

                            Build(buildable);
                        });
                        rows.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.Doorway.Rows == m_PreviousSectionData.Doorway.Rows)
                                return;

                            m_PreviousSectionData.Doorway.Rows = currentSectionData.Doorway.Rows;

                            Build(buildable);
                        });
                        offsetField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.Doorway.PositionOffset == m_PreviousSectionData.Doorway.PositionOffset)
                                return;

                            m_PreviousSectionData.Doorway.PositionOffset = currentSectionData.Doorway.PositionOffset;

                            // Here we either need to set door to null or update door control points.
                            currentSectionData.Doorway.Doors = null;

                            Build(buildable);
                        });
                        heightField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.Doorway.Height == m_PreviousSectionData.Doorway.Height)
                                return;

                            m_PreviousSectionData.Doorway.Height = currentSectionData.Doorway.Height;

                            currentSectionData.Doorway.Doors = null;

                            Build(buildable);
                        });
                        widthField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.Doorway.Width == m_PreviousSectionData.Doorway.Width)
                                return;

                            m_PreviousSectionData.Doorway.Width = currentSectionData.Doorway.Width;

                            currentSectionData.Doorway.Doors = null;

                            Build(buildable);
                        });
                        doorFrameDepthField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.DoorFrame.Depth == m_PreviousSectionData.DoorFrame.Depth)
                                return;

                            m_PreviousSectionData.DoorFrame.Depth = currentSectionData.DoorFrame.Depth;

                            currentSectionData.Doorway.Doors = null;

                            Build(buildable);
                        });
                        doorFrameScaleField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.DoorFrame.Scale == m_PreviousSectionData.DoorFrame.Scale)
                                return;

                            currentSectionData.Doorway.Doors = null;

                            m_PreviousSectionData.DoorFrame.Scale = currentSectionData.DoorFrame.Scale;
                            Build(buildable);
                        });
                        activeDoorwayElements.RegisterValueChangeCallback(evt =>
                        {
                            bool isDoorActive = currentSectionData.Doorway.ActiveElements.IsElementActive(DoorwayElement.Door);
                            bool isFrameActive = currentSectionData.Doorway.ActiveElements.IsElementActive(DoorwayElement.Frame);

                            doorDataField.SetEnabled(isDoorActive);
                            frameFoldout.SetEnabled(isFrameActive);

                            if (currentSectionData.Doorway.ActiveElements == m_PreviousSectionData.Doorway.ActiveElements)
                                return;

                            m_PreviousSectionData.Doorway.ActiveElements = currentSectionData.Doorway.ActiveElements;

                            foreach (DoorData door in currentSectionData.Doorway.Doors)
                            {
                                door.ActiveElements = currentSectionData.Doorway.ActiveElements.ToDoorElement();
                            }

                            Build(buildable);
                        });
                        #endregion

                        #region Add Fields to Container
                        gridFoldout.Add(cols);
                        gridFoldout.Add(rows);
                        wallElementContainer.Add(gridFoldout);
                        wallElementContainer.Add(sizePosFold);
                        sizePosFold.Add(offsetField);
                        sizePosFold.Add(heightField);
                        sizePosFold.Add(widthField);
                        wallElementContainer.Add(doorFoldout);
                        doorFoldout.Add(activeDoorwayElements);
                        doorFoldout.Add(doorDataField);
                        doorFoldout.Add(frameFoldout);
                        frameFoldout.Add(doorFrameDepthField);
                        frameFoldout.Add(doorFrameScaleField);
                        #endregion
                    }
                    break;
                case WallElement.Archway:
                    {
                        var archway = props.Archway;
                        var archDoor = props.ArchDoor;
                        var frame = props.Frame;

                        #region Fields
                        Foldout gridFoldout = new Foldout() { text = "Grid" };
                        PropertyField cols = new PropertyField(archway.Columns) { label = "Columns" };
                        PropertyField rows = new PropertyField(archway.Rows) { label = "Rows" };
                        Foldout sizePosFold = new Foldout() { text = "Size, Shape & Position" };
                        PropertyField offsetField = new PropertyField(archway.PositionOffset) { label = "Offset", tooltip = "Position offset from the centre" };
                        PropertyField archHeightField = new PropertyField(archway.ArchHeight) { label = "Arch Height" };
                        PropertyField archSidesField = new PropertyField(archway.ArchSides) { label = "Arch Sides", tooltip = " Number of points on the Arch" };
                        PropertyField heightField = new PropertyField(archway.Height) { label = "Height" };
                        PropertyField widthField = new PropertyField(archway.Width) { label = "Width" };
                        Foldout doorFoldout = new Foldout() { text = "Door" };
                        PropertyField archDataField = new PropertyField(archDoor.Data);
                        Foldout frameFoldout = new Foldout() { text = "Frame" };
                        PropertyField doorFrameDepthField = new PropertyField(frame.Depth) { label = "Depth" };
                        PropertyField doorFrameScaleField = new PropertyField(frame.Scale) { label = "Scale" };
                        PropertyField activeDoorwayElements = new PropertyField(archway.ActiveElements) { label = "Active Elements" };
                        #endregion

                        #region Bind
                        cols.BindProperty(archway.Columns);
                        rows.BindProperty(archway.Rows);
                        offsetField.BindProperty(archway.PositionOffset);
                        archHeightField.BindProperty(archway.ArchHeight);
                        archSidesField.BindProperty(archway.ArchSides);
                        heightField.BindProperty(archway.Height);
                        widthField.BindProperty(archway.Width);
                        archDataField.BindProperty(archDoor.Data);
                        doorFrameDepthField.BindProperty(frame.Depth);
                        doorFrameScaleField.BindProperty(frame.Scale);
                        activeDoorwayElements.BindProperty(archway.ActiveElements);
                        #endregion

                        #region Register Value Change Callback
                        cols.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.Archway.Columns == m_PreviousSectionData.Archway.Columns)
                                return;

                            m_PreviousSectionData.Archway.Columns = currentSectionData.Archway.Columns;

                            Build(buildable);
                        });
                        rows.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.Archway.Rows == m_PreviousSectionData.Archway.Rows)
                                return;

                            m_PreviousSectionData.Archway.Rows = currentSectionData.Archway.Rows;

                            Build(buildable);
                        });
                        offsetField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.Archway.PositionOffset == m_PreviousSectionData.Archway.PositionOffset)
                                return;

                            m_PreviousSectionData.Archway.PositionOffset = currentSectionData.Archway.PositionOffset;

                            currentSectionData.Archway.Doors = null;

                            Build(buildable);
                        });
                        archHeightField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.Archway.ArchHeight == m_PreviousSectionData.Archway.ArchHeight)
                                return;

                            m_PreviousSectionData.Archway.ArchHeight = currentSectionData.Archway.ArchHeight;

                            currentSectionData.Archway.Doors = null;

                            Build(buildable);

                        });
                        archSidesField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.Archway.ArchSides == m_PreviousSectionData.Archway.ArchSides)
                                return;

                            m_PreviousSectionData.Archway.ArchSides = currentSectionData.Archway.ArchSides;

                            currentSectionData.Archway.Doors = null;

                            Build(buildable);
                        });
                        heightField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.Archway.Height == m_PreviousSectionData.Archway.Height)
                                return;

                            m_PreviousSectionData.Archway.Height = currentSectionData.Archway.Height;

                            currentSectionData.Archway.Doors = null;

                            Build(buildable);
                        });
                        widthField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.Archway.Width == m_PreviousSectionData.Archway.Width)
                                return;

                            m_PreviousSectionData.Archway.Width = currentSectionData.Archway.Width;

                            currentSectionData.Archway.Doors = null;

                            Build(buildable);
                        });
                        doorFrameDepthField.RegisterValueChangeCallback(evt => 
                        {
                            if (currentSectionData.DoorFrame.Depth == m_PreviousSectionData.DoorFrame.Depth)
                                return;

                            m_PreviousSectionData.DoorFrame.Depth = currentSectionData.DoorFrame.Depth;

                            Build(buildable);
                        });
                        doorFrameScaleField.RegisterValueChangeCallback(evt => 
                        {
                            if (currentSectionData.DoorFrame.Scale == m_PreviousSectionData.DoorFrame.Scale)
                                return;

                            m_PreviousSectionData.DoorFrame.Scale = currentSectionData.DoorFrame.Scale;

                            Build(buildable);
                        });
                        activeDoorwayElements.RegisterValueChangeCallback(evt =>
                        {
                            bool isDoorActive = currentSectionData.Archway.ActiveElements.IsElementActive(DoorwayElement.Door);
                            bool isFrameActive = currentSectionData.Archway.ActiveElements.IsElementActive(DoorwayElement.Frame);

                            archDataField.SetEnabled(isDoorActive);
                            frameFoldout.SetEnabled(isFrameActive);

                            if (currentSectionData.Archway.ActiveElements == m_PreviousSectionData.Archway.ActiveElements)
                                return;

                            m_PreviousSectionData.Archway.ActiveElements = currentSectionData.Archway.ActiveElements;

                            foreach (DoorData archDoor in currentSectionData.Archway.Doors)
                            {
                                archDoor.ActiveElements = currentSectionData.Archway.ActiveElements.ToDoorElement();
                            }

                            Build(buildable);
                        });
                        #endregion

                        #region Add Fields to Container
                        gridFoldout.Add(cols);
                        gridFoldout.Add(rows);
                        wallElementContainer.Add(gridFoldout);
                        wallElementContainer.Add(sizePosFold);
                        sizePosFold.Add(offsetField);
                        sizePosFold.Add(archHeightField);
                        sizePosFold.Add(archSidesField);
                        sizePosFold.Add(heightField);
                        sizePosFold.Add(widthField);
                        wallElementContainer.Add(doorFoldout);
                        doorFoldout.Add(activeDoorwayElements);
                        doorFoldout.Add(archDataField);
                        doorFoldout.Add(frameFoldout);
                        frameFoldout.Add(doorFrameDepthField);
                        frameFoldout.Add(doorFrameScaleField);
                        #endregion
                    }
                    break;
                case WallElement.Window:
                    {
                        var opening = props.WindowOpening;
                        var window = props.Window;

                        #region Fields
                        Foldout gridFoldout = new Foldout() { text = "Grid" };
                        PropertyField cols = new PropertyField(opening.Columns) { label = "Columns" };
                        PropertyField rows = new PropertyField(opening.Rows) { label = "Rows" };
                        Foldout shapeFold = new Foldout() { text = "Shape" };
                        PropertyField sides = new PropertyField(opening.Sides) { label = "Sides" };
                        PropertyField height = new PropertyField(opening.Height) { label = "Height" };
                        PropertyField width = new PropertyField(opening.Width) { label = "Width" };
                        PropertyField angle = new PropertyField(opening.Angle) { label = "Angle" };
                        Foldout windowFoldout = new Foldout() { text = "Window" };
                        // Issue: I'm unable to maniputate the window data in the wall section inspector. 
                        PropertyField windowDataField = new PropertyField(window.Data);
                        #endregion

                        #region Bind
                        cols.BindProperty(opening.Columns);
                        rows.BindProperty(opening.Rows);
                        sides.BindProperty(opening.Sides);
                        height.BindProperty(opening.Height);
                        width.BindProperty(opening.Width);
                        angle.BindProperty(opening.Angle);
                        windowDataField.BindProperty(window.Data);
                        #endregion

                        #region Register Value Change Callbacks
                        cols.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.WindowOpening.Columns == m_PreviousSectionData.WindowOpening.Columns)
                                return;

                            m_PreviousSectionData.WindowOpening.Columns = currentSectionData.WindowOpening.Columns;

                            Build(buildable);
                        });
                        rows.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.WindowOpening.Rows == m_PreviousSectionData.WindowOpening.Rows)
                                return;

                            m_PreviousSectionData.WindowOpening.Rows = currentSectionData.WindowOpening.Rows;

                            Build(buildable);
                        });
                        sides.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.WindowOpening.Sides == m_PreviousSectionData.WindowOpening.Sides)
                                return;

                            m_PreviousSectionData.WindowOpening.Sides = currentSectionData.WindowOpening.Sides;

                            currentSectionData.WindowOpening.Windows = null;

                            Build(buildable);
                        });
                        height.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.WindowOpening.Height == m_PreviousSectionData.WindowOpening.Height)
                                return;

                            m_PreviousSectionData.WindowOpening.Height = currentSectionData.WindowOpening.Height;

                            currentSectionData.WindowOpening.Windows = null;

                            Build(buildable);
                        });
                        width.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.WindowOpening.Width == m_PreviousSectionData.WindowOpening.Width)
                                return;

                            m_PreviousSectionData.WindowOpening.Width = currentSectionData.WindowOpening.Width;

                            currentSectionData.WindowOpening.Windows = null;

                            Build(buildable);
                        });
                        angle.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.WindowOpening.Angle == m_PreviousSectionData.WindowOpening.Angle)
                                return;

                            m_PreviousSectionData.WindowOpening.Angle = currentSectionData.WindowOpening.Angle;

                            currentSectionData.WindowOpening.Windows = null;

                            Build(buildable);
                        });
                        #endregion

                        #region Add Fields to Container
                        gridFoldout.Add(cols);
                        gridFoldout.Add(rows);
                        wallElementContainer.Add(gridFoldout);
                        wallElementContainer.Add(shapeFold);
                        wallElementContainer.Add(windowFoldout);
                        shapeFold.Add(sides);
                        shapeFold.Add(height);
                        shapeFold.Add(width);
                        shapeFold.Add(angle);
                        windowFoldout.Add(windowDataField);
                        #endregion
                    }
                    break;
                case WallElement.Extension:
                    {
                        var extension = props.Extension;

                        Foldout extensionFoldout = new Foldout() { text = "Extension" };
                        PropertyField extensionDistance = new PropertyField(extension.Distance);
                        extensionDistance.BindProperty(extension.Distance);
                        extensionDistance.RegisterValueChangeCallback(evt => buildable.Build());

                        PropertyField extensionHeight = new PropertyField(extension.Height);
                        extensionHeight.BindProperty(extension.Height);
                        extensionHeight.RegisterValueChangeCallback(evt => buildable.Build());

                        PropertyField extensionWidth = new PropertyField(extension.Width);
                        extensionWidth.BindProperty(extension.Width);
                        extensionWidth.RegisterValueChangeCallback(evt => buildable.Build());

                        Foldout storeyFoldout = new Foldout() { text = "Storey" };

                        //PropertyField extensionStoreyData = new PropertyField();
                        //extensionStoreyData.BindProperty(props.ExtensionStorey);

                        //Foldout roofFoldout = new Foldout() { text = "Roof" };
                        //PropertyField extensionRoofData = new PropertyField();
                        //extensionRoofData.BindProperty(props.ExtensionRoof);

                        wallElementContainer.Add(extensionFoldout);
                        extensionFoldout.Add(extensionDistance);
                        extensionFoldout.Add(extensionHeight);
                        extensionFoldout.Add(extensionWidth);
                        extensionFoldout.Add(storeyFoldout);
                        //storeyFoldout.Add(extensionStoreyData);
                        //extensionFoldout.Add(roofFoldout);
                        //roofFoldout.Add(extensionRoofData);
                    }
                    break;
                case WallElement.Empty:
                    break;
            }

            if(currentSectionData.WallElement != m_PreviousSectionData.WallElement)
            {
                Build(buildable);
                m_PreviousSectionData.WallElement = currentSectionData.WallElement;
            }
        });

        container.Add(wallElementContainer);

        return container;
    }

    private WallSectionData GetWallSectionDataFromBuildable(IBuildable buildable)
    {
        WallSectionData sectionData = null;

        switch(buildable)
        {
            case WallSection:
                WallSection wallSection = buildable as WallSection;
                sectionData = wallSection.Data;
                break;
            case Wall:
                // TODO: change to get currently selected wall section.
                Wall wall = buildable as Wall;
                sectionData = wall.Data.Sections[0];
                break;
        }

        return sectionData;
    }

    private void Build(IBuildable buildable)
    {
        switch(buildable)
        {
            case WallSection:
                buildable.Demolish();
                buildable.Build();
                break;
            case Wall:
                // TODO: Get the currently selected wall section & call build on that.
                break;
        }
    }
}
