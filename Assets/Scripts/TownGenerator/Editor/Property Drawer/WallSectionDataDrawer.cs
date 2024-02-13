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
                        cols.BindProperty(props.DoorColumns);

                        PropertyField rows = new PropertyField(props.DoorRows, "Rows") { label = "Rows" };
                        rows.BindProperty(props.DoorRows);

                        Foldout sizePosFold = new Foldout() { text = "Size & Position" };

                        PropertyField offsetField = new PropertyField(props.DoorOffset, "Offset") { label = "Offset" };
                        offsetField.BindProperty(props.DoorOffset);

                        PropertyField heightField = new PropertyField(props.DoorHeight, "Height") { label = "Height" };
                        heightField.BindProperty(props.DoorHeight);

                        PropertyField widthField = new PropertyField(props.DoorWidth, "Width") { label = "Width" };
                        widthField.BindProperty(props.DoorWidth);

                        Foldout doorFoldout = new Foldout() { text = "Door" };

                        PropertyField doorDataField = new PropertyField(props.DoorData);
                        doorDataField.BindProperty(props.DoorData);

                        Foldout frameFoldout = new Foldout() { text = "Frame" };

                        PropertyField doorFrameDepthField = new PropertyField(props.DoorFrameDepth, "Depth") { label = "Depth" };
                        doorFrameDepthField.BindProperty(props.DoorFrameDepth);

                        PropertyField doorFrameScaleField = new PropertyField(props.DoorFrameScale, "Scale") { label = "Scale" };
                        doorFrameScaleField.BindProperty(props.DoorFrameScale);

                        PropertyField activeDoorwayElements = new PropertyField(props.DoorwayElement, "Active Elements") { label = "Active Elements" };
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
                                props.Windows.SetUnderlyingValue(new WindowData[size]);
                                buildable.Demolish();
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
                                if (buildable is WallSection)
                                {
                                    WallSection section = buildable as WallSection;
                                    section.transform.DeleteChildren();
                                }
                                else
                                {
                                    buildable.Demolish();
                                }

                                props.Windows.SetUnderlyingValue(new WindowData[size]);
                                buildable.Build();
                            }
                        });

                        gridFoldout.Add(cols);
                        gridFoldout.Add(rows);
                        
                        Foldout shapeFold = new Foldout() { text = "Shape" };

                        PropertyField sides = new PropertyField() { label = "Sides" };
                        sides.BindProperty(props.WindowSides);
                        sides.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;

                            props.WindowSides.SetUnderlyingValue(evt.changedProperty.intValue);

                            WallSectionData sectionData = data.GetUnderlyingValue() as WallSectionData;

                            IList<IList<Vector3>> holePoints = WallSection.CalculateWindow(sectionData);

                            foreach (WindowData winData in sectionData.Windows)
                            {
                                winData.ControlPoints = holePoints[winData.ID].ToArray();
                                winData.ClearInnerFrameHolePoints();
                                winData.DoesOuterFrameNeedRebuild = winData.IsOuterFrameActive;
                                winData.DoesInnerFrameNeedRebuild = winData.IsInnerFrameActive;
                                winData.DoesPaneNeedRebuild = winData.IsPaneActive;
                                winData.DoShuttersNeedRebuild = winData.AreShuttersActive;
                            }

                            if(buildable is WallSection)
                            {
                                WallSection section = buildable as WallSection;

                                section.BuildSection(holePoints);

                                for(int i = 0; i < section.transform.childCount; i++)
                                {
                                    if(section.transform.GetChild(i).TryGetComponent(out Window window))
                                    {
                                        window.Initialize(sectionData.Windows[window.Data.ID]);
                                        window.Build();
                                    }
                                }
                            }
                        });
     

                        PropertyField height = new PropertyField() { label = "Height" };
                        height.BindProperty(props.WindowHeight);
                        height.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;

                            WallSectionData sectionData = data.GetUnderlyingValue() as WallSectionData;

                            IList<IList<Vector3>> holePoints = WallSection.CalculateWindow(sectionData);

                            bool rebuild = false;

                            foreach (WindowData winData in sectionData.Windows)
                            {
                                if (!winData.ControlPoints.SequenceEqual(holePoints[winData.ID]))
                                {
                                    rebuild = true;
                                }
                                else
                                {
                                    continue;
                                }

                                winData.ControlPoints = holePoints[winData.ID].ToArray();
                            }

                            if(rebuild)
                            {
                                buildable.Demolish();
                                buildable.Build();
                            }
                            
                        });

                        PropertyField width = new PropertyField() { label = "Width" };
                        width.BindProperty(props.WindowWidth);
                        width.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;

                            bool rebuild = false;

                            WallSectionData sectionData = data.GetUnderlyingValue() as WallSectionData;

                            IList<IList<Vector3>> holePoints = WallSection.CalculateWindow(sectionData);

                            foreach (WindowData winData in sectionData.Windows)
                            {
                                if(!winData.ControlPoints.SequenceEqual(holePoints[winData.ID]))
                                {
                                    rebuild = true;
                                }
                                else
                                {
                                    continue;
                                }

                                winData.ControlPoints = holePoints[winData.ID].ToArray();
                            }

                            if(rebuild)
                            {
                                buildable.Demolish();
                                buildable.Build();
                            }
                                
                        });


                        PropertyField angle = new PropertyField() { label = "Angle" };
                        angle.BindProperty(props.WindowAngle);
                        angle.RegisterValueChangeCallback(evt =>
                        {
                            if (evt == null)
                                return;

                            bool rebuild = false;

                            WallSectionData sectionData = data.GetUnderlyingValue() as WallSectionData;

                            IList<IList<Vector3>> holePoints = WallSection.CalculateWindow(sectionData);

                            foreach (WindowData winData in sectionData.Windows)
                            {
                                if (!winData.ControlPoints.SequenceEqual(holePoints[winData.ID]))
                                {
                                    rebuild = true;
                                }
                                else
                                {
                                    continue;
                                }

                                winData.ControlPoints = holePoints[winData.ID].ToArray();
                            }

                            if(rebuild)
                            {
                                buildable.Demolish();
                                buildable.Build();
                            }
                            
                        });

                        Foldout windowFoldout = new Foldout() { text = "Window" };

                        PropertyField windowDataField = new PropertyField(props.WindowData);
                        windowDataField.BindProperty(props.WindowData);

                        wallElementContainer.Add(gridFoldout);
                        wallElementContainer.Add(shapeFold);
                        wallElementContainer.Add(windowFoldout);

                        shapeFold.Add(sides);
                        shapeFold.Add(height);
                        shapeFold.Add(width);
                        shapeFold.Add(angle);

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

            if(currentSectionData.WallElement == m_PreviousSectionData.WallElement)
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
