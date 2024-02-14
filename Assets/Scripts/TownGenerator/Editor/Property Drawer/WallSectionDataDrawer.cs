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
    [SerializeField] private WallSectionData m_PreviousSectionData; // This is a copy of data, used to determine if data values actually change.

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
                        #region Fields
                        Foldout gridFoldout = new Foldout() { text = "Grid" };
                        PropertyField cols = new PropertyField(props.DoorColumns, "Columns") { label = "Columns" };
                        PropertyField rows = new PropertyField(props.DoorRows, "Rows") { label = "Rows" };
                        Foldout sizePosFold = new Foldout() { text = "Size & Position" };
                        PropertyField offsetField = new PropertyField(props.DoorOffset, "Offset") { label = "Offset" };
                        PropertyField heightField = new PropertyField(props.DoorHeight, "Height") { label = "Height" };
                        PropertyField widthField = new PropertyField(props.DoorWidth, "Width") { label = "Width" };
                        Foldout doorFoldout = new Foldout() { text = "Door" };
                        PropertyField doorDataField = new PropertyField(props.DoorData);
                        Foldout frameFoldout = new Foldout() { text = "Frame" };
                        PropertyField doorFrameDepthField = new PropertyField(props.DoorFrameDepth, "Depth") { label = "Depth" };
                        PropertyField doorFrameScaleField = new PropertyField(props.DoorFrameScale, "Scale") { label = "Scale" };
                        PropertyField activeDoorwayElements = new PropertyField(props.DoorwayElement, "Active Elements") { label = "Active Elements" };

                        #endregion

                        #region Bind
                        cols.BindProperty(props.DoorColumns);
                        rows.BindProperty(props.DoorRows);
                        offsetField.BindProperty(props.DoorOffset);
                        heightField.BindProperty(props.DoorHeight);
                        widthField.BindProperty(props.DoorWidth);
                        doorDataField.BindProperty(props.DoorData);
                        doorFrameDepthField.BindProperty(props.DoorFrameDepth);
                        doorFrameScaleField.BindProperty(props.DoorFrameScale);
                        activeDoorwayElements.BindProperty(props.DoorwayElement);
                        #endregion

                        #region Register Value Change Callbacks
                        // We are relying on unity triggering these callbacks when the user clicks on an object with a non-hidden & serialized wall section data.
                        cols.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.DoorColumns == m_PreviousSectionData.DoorColumns)
                                return;

                            m_PreviousSectionData.DoorColumns = currentSectionData.DoorColumns;

                            Build(buildable);
                        });
                        rows.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.DoorRows == m_PreviousSectionData.DoorRows)
                                return;

                            m_PreviousSectionData.DoorRows = currentSectionData.DoorRows;

                            Build(buildable);
                        });
                        offsetField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.DoorSideOffset == m_PreviousSectionData.DoorSideOffset)
                                return;

                            m_PreviousSectionData.DoorSideOffset = currentSectionData.DoorSideOffset;

                            // Here we either need to set door to null or update door control points.
                            currentSectionData.Doors = null;

                            Build(buildable);
                        });
                        heightField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.DoorPedimentHeight == m_PreviousSectionData.DoorPedimentHeight)
                                return;

                            m_PreviousSectionData.DoorPedimentHeight = currentSectionData.DoorPedimentHeight;

                            currentSectionData.Doors = null;

                            Build(buildable);
                        });
                        widthField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.DoorSideWidth == m_PreviousSectionData.DoorSideWidth)
                                return;

                            m_PreviousSectionData.DoorSideWidth = currentSectionData.DoorSideWidth;

                            currentSectionData.Doors = null;

                            Build(buildable);
                        });
                        doorFrameDepthField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.DoorFrameDepth == m_PreviousSectionData.DoorFrameDepth)
                                return;

                            m_PreviousSectionData.DoorFrameDepth = currentSectionData.DoorFrameDepth;

                            currentSectionData.Doors = null;

                            Build(buildable);
                        });
                        doorFrameScaleField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.DoorFrameInsideScale == m_PreviousSectionData.DoorFrameInsideScale)
                                return;

                            currentSectionData.Doors = null;

                            m_PreviousSectionData.DoorFrameInsideScale = currentSectionData.DoorFrameInsideScale;
                            Build(buildable);
                        });
                        activeDoorwayElements.RegisterValueChangeCallback(evt =>
                        {
                            bool isDoorActive = currentSectionData.ActiveDoorElements.IsElementActive(DoorwayElement.Door);
                            bool isFrameActive = currentSectionData.ActiveDoorElements.IsElementActive(DoorwayElement.Frame);

                            doorDataField.SetEnabled(isDoorActive);
                            frameFoldout.SetEnabled(isFrameActive);

                            if (currentSectionData.ActiveDoorElements == m_PreviousSectionData.ActiveDoorElements)
                                return;

                            m_PreviousSectionData.ActiveDoorElements = currentSectionData.ActiveDoorElements;

                            foreach (DoorData door in currentSectionData.Doors)
                            {
                                door.ActiveElements = currentSectionData.ActiveDoorElements.ToDoorElement();
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
                        #region Fields
                        Foldout gridFoldout = new Foldout() { text = "Grid" };
                        PropertyField cols = new PropertyField() { label = "Columns" };
                        PropertyField rows = new PropertyField() { label = "Rows" };
                        Foldout sizePosFold = new Foldout() { text = "Size, Shape & Position" };
                        PropertyField offsetField = new PropertyField(props.ArchOffset) { label = "Offset", tooltip = "Position offset from the centre" };
                        PropertyField archHeightField = new PropertyField(props.ArchHeight) { label = "Arch Height" };
                        PropertyField archSidesField = new PropertyField(props.ArchSides) { label = "Arch Sides", tooltip = " Number of points on the Arch" };
                        PropertyField heightField = new PropertyField(props.ArchDoorHeight) { label = "Height" };
                        PropertyField widthField = new PropertyField(props.ArchWidth) { label = "Width" };
                        Foldout doorFoldout = new Foldout() { text = "Door" };
                        PropertyField archDataField = new PropertyField(props.ArchDoorData);
                        Foldout frameFoldout = new Foldout() { text = "Frame" };
                        PropertyField doorFrameDepthField = new PropertyField(props.ArchDoorFrameDepth) { label = "Depth" };
                        PropertyField doorFrameScaleField = new PropertyField(props.ArchDoorFrameScale) { label = "Scale" };
                        PropertyField activeDoorwayElements = new PropertyField(props.ArchwayElement) { label = "Active Elements" };
                        #endregion

                        #region Bind
                        cols.BindProperty(props.ArchColumns);
                        rows.BindProperty(props.ArchRows);
                        offsetField.BindProperty(props.ArchOffset);
                        archHeightField.BindProperty(props.ArchHeight);
                        archSidesField.BindProperty(props.ArchSides);
                        heightField.BindProperty(props.ArchDoorHeight);
                        widthField.BindProperty(props.ArchWidth);
                        archDataField.BindProperty(props.DoorData);
                        doorFrameDepthField.BindProperty(props.ArchDoorFrameDepth);
                        doorFrameScaleField.BindProperty(props.ArchDoorFrameScale);
                        activeDoorwayElements.BindProperty(props.ArchwayElement);
                        #endregion

                        #region Register Value Change Callback
                        cols.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.ArchColumns == m_PreviousSectionData.ArchColumns)
                                return;

                            m_PreviousSectionData.ArchColumns = currentSectionData.ArchColumns;

                            Build(buildable);
                        });
                        rows.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.ArchRows == m_PreviousSectionData.ArchRows)
                                return;

                            m_PreviousSectionData.ArchRows = currentSectionData.ArchRows;

                            Build(buildable);
                        });
                        offsetField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.ArchSideOffset == m_PreviousSectionData.ArchSideOffset)
                                return;

                            m_PreviousSectionData.ArchSideOffset = currentSectionData.ArchSideOffset;

                            currentSectionData.ArchDoors = null;

                            Build(buildable);
                        });
                        archHeightField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.ArchHeight == m_PreviousSectionData.ArchHeight)
                                return;

                            m_PreviousSectionData.ArchHeight = currentSectionData.ArchHeight;

                            currentSectionData.ArchDoors = null;

                            Build(buildable);

                        });
                        archSidesField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.ArchSides == m_PreviousSectionData.ArchSides)
                                return;

                            m_PreviousSectionData.ArchSides = currentSectionData.ArchSides;

                            currentSectionData.ArchDoors = null;

                            Build(buildable);
                        });
                        heightField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.ArchPedimentHeight == m_PreviousSectionData.ArchPedimentHeight)
                                return;

                            m_PreviousSectionData.ArchPedimentHeight = currentSectionData.ArchPedimentHeight;

                            currentSectionData.ArchDoors = null;

                            Build(buildable);
                        });
                        widthField.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.ArchSideWidth == m_PreviousSectionData.ArchSideWidth)
                                return;

                            m_PreviousSectionData.ArchSideWidth = currentSectionData.ArchSideWidth;

                            currentSectionData.ArchDoors = null;

                            Build(buildable);
                        });
                        doorFrameDepthField.RegisterValueChangeCallback(evt => 
                        {
                            if (currentSectionData.ArchDoorFrameDepth == m_PreviousSectionData.ArchDoorFrameDepth)
                                return;

                            m_PreviousSectionData.ArchDoorFrameDepth = currentSectionData.ArchDoorFrameDepth;

                            Build(buildable);
                        });
                        doorFrameScaleField.RegisterValueChangeCallback(evt => 
                        {
                            if (currentSectionData.ArchDoorFrameInsideScale == m_PreviousSectionData.ArchDoorFrameInsideScale)
                                return;

                            m_PreviousSectionData.ArchDoorFrameInsideScale = currentSectionData.ArchDoorFrameInsideScale;

                            Build(buildable);
                        });
                        activeDoorwayElements.RegisterValueChangeCallback(evt =>
                        {
                            bool isDoorActive = currentSectionData.ActiveDoorElements.IsElementActive(DoorwayElement.Door);
                            bool isFrameActive = currentSectionData.ActiveDoorElements.IsElementActive(DoorwayElement.Frame);

                            archDataField.SetEnabled(isDoorActive);
                            frameFoldout.SetEnabled(isFrameActive);

                            if (currentSectionData.ActiveArchDoorElements == m_PreviousSectionData.ActiveArchDoorElements)
                                return;

                            m_PreviousSectionData.ActiveArchDoorElements = currentSectionData.ActiveArchDoorElements;

                            foreach (DoorData archDoor in currentSectionData.ArchDoors)
                            {
                                archDoor.ActiveElements = currentSectionData.ActiveArchDoorElements.ToDoorElement();
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
                        #region Fields
                        Foldout gridFoldout = new Foldout() { text = "Grid" };
                        PropertyField cols = new PropertyField() { label = "Columns" };
                        PropertyField rows = new PropertyField() { label = "Rows" };
                        Foldout shapeFold = new Foldout() { text = "Shape" };
                        PropertyField sides = new PropertyField() { label = "Sides" };
                        PropertyField height = new PropertyField() { label = "Height" };
                        PropertyField width = new PropertyField() { label = "Width" };
                        PropertyField angle = new PropertyField() { label = "Angle" };
                        Foldout windowFoldout = new Foldout() { text = "Window" };
                        PropertyField windowDataField = new PropertyField(props.WindowData);
                        #endregion

                        #region Bind
                        rows.BindProperty(props.WindowRows);
                        cols.BindProperty(props.WindowColumns);
                        sides.BindProperty(props.WindowSides);
                        height.BindProperty(props.WindowHeight);
                        width.BindProperty(props.WindowWidth);
                        angle.BindProperty(props.WindowAngle);
                        windowDataField.BindProperty(props.WindowData);
                        #endregion

                        #region Register Value Change Callbacks
                        cols.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.WindowColumns == m_PreviousSectionData.WindowColumns)
                                return;

                            m_PreviousSectionData.WindowColumns = currentSectionData.WindowColumns;

                            Build(buildable);
                        });
                        rows.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.WindowRows == m_PreviousSectionData.WindowRows)
                                return;

                            m_PreviousSectionData.WindowRows = currentSectionData.WindowRows;

                            Build(buildable);
                        });
                        sides.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.WindowSides == m_PreviousSectionData.WindowSides)
                                return;

                            m_PreviousSectionData.WindowSides = currentSectionData.WindowSides;

                            currentSectionData.Windows = null;

                            Build(buildable);
                        });
                        height.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.WindowHeight == m_PreviousSectionData.WindowHeight)
                                return;

                            m_PreviousSectionData.WindowHeight = currentSectionData.WindowHeight;

                            currentSectionData.Windows = null;

                            Build(buildable);
                        });
                        width.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.WindowWidth == m_PreviousSectionData.WindowWidth)
                                return;

                            m_PreviousSectionData.WindowWidth = currentSectionData.WindowWidth;

                            currentSectionData.Windows = null;

                            Build(buildable);
                        });
                        angle.RegisterValueChangeCallback(evt =>
                        {
                            if (currentSectionData.WindowAngle == m_PreviousSectionData.WindowAngle)
                                return;

                            m_PreviousSectionData.WindowAngle = currentSectionData.WindowAngle;

                            currentSectionData.Windows = null;

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
