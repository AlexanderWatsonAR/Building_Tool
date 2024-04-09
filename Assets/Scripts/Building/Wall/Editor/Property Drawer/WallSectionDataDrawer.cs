using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Unity.VisualScripting;

[CustomPropertyDrawer(typeof(WallSectionData))]
public class WallSectionDataDrawer : PropertyDrawer
{
    [SerializeField] WallSectionData m_PreviousData; // This is a copy of data, used to determine if data values actually change.

    [SerializeField] WallElement m_PreviousElement;

    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        VisualElement root = new VisualElement();

        WallSectionDataSerializedProperties props = new WallSectionDataSerializedProperties(data);

        WallSectionData currentData = data.GetUnderlyingValue() as WallSectionData;
        m_PreviousData = currentData.Clone() as WallSectionData;

        m_PreviousElement = currentData.WallElement;

        PropertyField wallElementField = new PropertyField(props.WallElement) { label = "Wall Element" };
        wallElementField.BindProperty(props.WallElement);
        root.Add(wallElementField);

        VisualElement wallElementContainer = new VisualElement();

        wallElementField.RegisterValueChangeCallback(evt =>
        {
            if ((currentData.WallElement == m_PreviousElement) && wallElementContainer.childCount > 0)
                return;

            if (currentData.WallElement != m_PreviousElement)
            {
                currentData.IsDirty = true;
                m_PreviousElement = currentData.WallElement;
            }         

            wallElementContainer.Clear();

            switch (currentData.WallElement)
            {
                case WallElement.Wall:
                    break;
                case WallElement.Doorway:
                    {
                        var doorway = props.Doorway;
                        var door = props.Door;
                        var frame = props.Frame;

                        #region Fields
                        PropertyField doorwayField = new PropertyField(doorway.Data);
                        PropertyField activeElements = new PropertyField(doorway.ActiveElements, "Active Elements") { label = "Active Elements" };
                        Foldout doorFoldout = new Foldout() { text = "Door" };
                        PropertyField doorDataField = new PropertyField(door.Data);
                        PropertyField doorFrameField = new PropertyField(frame.Data);
                        #endregion

                        #region Bind
                        doorwayField.BindProperty(doorway.Data);
                        activeElements.BindProperty(doorway.ActiveElements);
                        doorDataField.BindProperty(door.Data);
                        doorFrameField.BindProperty(frame.Data);
                        #endregion

                        #region Register Value Change Callbacks
                        doorwayField.RegisterValueChangeCallback(evt =>
                        {
                            DoorwayData doorwayData = evt.changedProperty.GetUnderlyingValue() as DoorwayData;

                            if (doorwayData.Equals(m_PreviousData.Doorway))
                                return;

                            currentData.IsDirty = true;
                        
                        });
                        activeElements.RegisterValueChangeCallback(evt =>
                        {
                            bool isDoorActive = currentData.Doorway.ActiveElements.IsElementActive(DoorwayElement.Door);
                            bool isFrameActive = currentData.Doorway.ActiveElements.IsElementActive(DoorwayElement.Frame);

                            doorDataField.SetEnabled(isDoorActive);
                            doorFrameField.SetEnabled(isFrameActive);

                            if (currentData.Doorway.ActiveElements == m_PreviousData.Doorway.ActiveElements)
                                return;

                            m_PreviousData.Doorway.ActiveElements = currentData.Doorway.ActiveElements;

                            foreach (DoorData door in currentData.Doorway.Doors)
                            {
                                door.ActiveElements = currentData.Doorway.ActiveElements.ToDoorElement();
                            }

                        });
                        doorFrameField.RegisterValueChangeCallback(evt =>
                        {
                            FrameData frameData = evt.changedProperty.GetUnderlyingValue() as FrameData;

                            if (frameData.Equals(frame))
                                return;

                            currentData.IsDirty = true;
                        });
                        #endregion

                        #region Add Fields to Container
                        wallElementContainer.Add(doorwayField);
                        wallElementContainer.Add(doorFoldout);
                        doorFoldout.Add(activeElements);
                        doorFoldout.Add(doorDataField);
                        doorFoldout.Add(doorFrameField);
                        #endregion
                    }
                    break;
                case WallElement.Archway:
                    {
                        var archway = props.Archway;
                        var archDoor = props.ArchDoor;
                        var frame = props.Frame;

                        #region Fields
                        PropertyField archwayField = new PropertyField(archway.Data);
                        PropertyField activeElements = new PropertyField(archway.ActiveElements) { label = "Active Elements" };
                        Foldout doorFoldout = new Foldout() { text = "Door" };
                        PropertyField archDataField = new PropertyField(archDoor.Data);
                        PropertyField archFrameField = new PropertyField(frame.Data);
                        #endregion

                        #region Bind
                        archwayField.BindProperty(archway.Data);
                        archDataField.BindProperty(archDoor.Data);
                        archFrameField.BindProperty(frame.Data);
                        activeElements.BindProperty(archway.ActiveElements);
                        #endregion

                        #region Register Value Change Callback
                        archwayField.RegisterValueChangeCallback(evt => 
                        {
                            ArchwayData archwayData = evt.changedProperty.GetUnderlyingValue() as ArchwayData;

                            if (archwayData.Equals(m_PreviousData.Archway))
                                return;

                            currentData.IsDirty = true;
                        });
                        activeElements.RegisterValueChangeCallback(evt =>
                        {
                            bool isDoorActive = currentData.Archway.ActiveElements.IsElementActive(DoorwayElement.Door);
                            bool isFrameActive = currentData.Archway.ActiveElements.IsElementActive(DoorwayElement.Frame);

                            archDataField.SetEnabled(isDoorActive);
                            archFrameField.SetEnabled(isFrameActive);

                            if (currentData.Archway.ActiveElements == m_PreviousData.Archway.ActiveElements)
                                return;

                            m_PreviousData.Archway.ActiveElements = currentData.Archway.ActiveElements;

                            foreach (DoorData archDoor in currentData.Archway.Doors)
                            {
                                archDoor.ActiveElements = currentData.Archway.ActiveElements.ToDoorElement();
                            }
                        });
                        archFrameField.RegisterValueChangeCallback(evt => 
                        {
                            FrameData frameData = evt.changedProperty.GetUnderlyingValue() as FrameData;

                            if(frameData.Equals(m_PreviousData.DoorFrame))
                                return;

                            currentData.IsDirty = true;
                        });
                        #endregion

                        #region Add Fields to Container
                        wallElementContainer.Add(archwayField);
                        wallElementContainer.Add(doorFoldout);
                        doorFoldout.Add(activeElements);
                        doorFoldout.Add(archDataField);
                        doorFoldout.Add(archFrameField);
                        #endregion
                    }
                    break;
                case WallElement.Window:
                    {
                        var opening = props.WindowOpening;
                        var window = props.Window;

                        #region Fields
                        PropertyField windowOpening = new PropertyField(opening.Data);
                        Foldout windowFoldout = new Foldout() { text = "Windows", tooltip = "Altering this data will impact every window in the grid." };
                        PropertyField windowDataField = new PropertyField(window.Data);
                        #endregion

                        #region Bind
                        windowOpening.BindProperty(opening.Data);
                        windowDataField.BindProperty(window.Data);
                        #endregion

                        #region Register Value Change Callbacks
                        windowOpening.RegisterValueChangeCallback(evt =>
                        {
                            WindowOpeningData openingData = evt.changedProperty.GetUnderlyingValue() as WindowOpeningData;

                            if(m_PreviousData.WindowOpening.Equals(openingData))
                                return;

                            m_PreviousData.WindowOpening = openingData.Clone() as WindowOpeningData;

                            openingData.Windows = null;
                            currentData.IsDirty = true;
                            
                        });
                        windowDataField.RegisterValueChangeCallback(evt => 
                        {
                            WindowData wallSectionWindow = evt.changedProperty.GetUnderlyingValue() as WindowData;

                            if (m_PreviousData.Window.Equals(wallSectionWindow))
                                return;

                            m_PreviousData.Window = wallSectionWindow.Clone() as WindowData;

                            WindowData[] windows = currentData.WindowOpening.Windows;

                            for (int i = 0; i < windows.Length; i++)
                            {
                                WindowData window = windows[i];

                                window.ActiveElements = wallSectionWindow.ActiveElements;

                                if (wallSectionWindow.OuterFrame.IsDirty)
                                {
                                    window.OuterFrame.Scale = wallSectionWindow.OuterFrame.Scale;
                                    window.OuterFrame.Depth = wallSectionWindow.OuterFrame.Depth;
                                    window.OuterFrame.Holes = wallSectionWindow.OuterFrame.Holes;
                                    window.OuterFrame.IsDirty = wallSectionWindow.OuterFrame.IsDirty;
                                }

                                //if(wallSectionWindow.InnerFrame.IsDirty)
                                //{
                                //    window.InnerFrame.Scale = wallSectionWindow.InnerFrame.Scale;
                                //    window.InnerFrame.Depth = wallSectionWindow.InnerFrame.Depth;
                                //    window.InnerFrame.Columns = wallSectionWindow.InnerFrame.Columns;
                                //    window.InnerFrame.Rows = wallSectionWindow.InnerFrame.Rows;
                                //    window.InnerFrame.IsDirty = wallSectionWindow.InnerFrame.IsDirty;
                                //}

                                //if(wallSectionWindow.Pane.IsDirty)
                                //{
                                //    window.Pane.Depth = wallSectionWindow.Pane.Depth;
                                //    window.Pane.IsDirty = wallSectionWindow.Pane.IsDirty;
                                //}

                                //window.OuterFrame = wallSectionWindow.OuterFrame.IsDirty ? wallSectionWindow.OuterFrame.Clone() as FrameData : window.OuterFrame;
                                //window.InnerFrame = wallSectionWindow.InnerFrame.IsDirty ? wallSectionWindow.InnerFrame.Clone() as GridFrameData : window.InnerFrame;
                                //window.Pane = wallSectionWindow.Pane.IsDirty ? wallSectionWindow.Clone() as PaneData : window.Pane;
                                //window.LeftShutter = wallSectionWindow.LeftShutter.IsDirty ? wallSectionWindow.LeftShutter.Clone() as DoorData : window.LeftShutter;
                                //window.RightShutter = wallSectionWindow.RightShutter.IsDirty ? wallSectionWindow.RightShutter.Clone() as DoorData : window.RightShutter;
                            }

                        });
                        #endregion

                        #region Add Fields to Container
                        wallElementContainer.Add(windowOpening);
                        wallElementContainer.Add(windowFoldout);
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
                        //extensionDistance.RegisterValueChangeCallback(evt => buildable.Build());

                        PropertyField extensionHeight = new PropertyField(extension.Height);
                        extensionHeight.BindProperty(extension.Height);
                        //extensionHeight.RegisterValueChangeCallback(evt => buildable.Build());

                        PropertyField extensionWidth = new PropertyField(extension.Width);
                        extensionWidth.BindProperty(extension.Width);
                        //extensionWidth.RegisterValueChangeCallback(evt => buildable.Build());

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
        });

        root.Add(wallElementContainer);

        return root;
    }
}