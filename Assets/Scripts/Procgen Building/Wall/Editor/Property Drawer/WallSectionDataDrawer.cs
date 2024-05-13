using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using OnlyInvalid.ProcGenBuilding.Window;
using OnlyInvalid.ProcGenBuilding.Door;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using UnityEngine.ProBuilder.Shapes;
using UnityEditor.PackageManager.UI;
using static PlasticPipe.Server.MonitorStats;

namespace OnlyInvalid.ProcGenBuilding.Wall
{
    [CustomPropertyDrawer(typeof(WallSectionData))]
    public class WallSectionDataDrawer : DataDrawer
    {
        WallSectionDataSerializedProperties m_Props;
        WallSectionData m_CurrentData, m_PreviousData;

        WallElement m_PreviousElement;

        VisualElement m_OpeningContainer, m_DoorwayContainer, m_ArchwayContainer, m_WindowContainer;

        PropertyField m_WallElement;
        PropertyField m_Doorway, m_ActiveDoorwayElements, m_Door, m_DoorFrame;
        Foldout m_DoorFoldout;
        PropertyField m_Archway, m_ActiveArchwayElements, m_ArchDoor, m_ArchDoorFrame;
        Foldout m_ArchDoorFoldout;
        PropertyField m_WindowOpening, m_Window;
        Foldout m_WindowFoldout;

        protected override void AddFieldsToRoot()
        {
            m_Root.Add(m_WallElement);
            m_Root.Add(m_OpeningContainer);

            #region Doorway
            m_DoorwayContainer.Add(m_Doorway);
            m_DoorwayContainer.Add(m_DoorFoldout);
            m_DoorFoldout.Add(m_ActiveDoorwayElements);
            m_DoorFoldout.Add(m_Door);
            m_DoorFoldout.Add(m_DoorFrame);
            #endregion

            #region Archway
            m_ArchwayContainer.Add(m_Archway);
            m_ArchwayContainer.Add(m_ArchDoorFoldout);
            m_DoorFoldout.Add(m_ActiveArchwayElements);
            m_DoorFoldout.Add(m_ArchDoor);
            m_DoorFoldout.Add(m_ArchDoorFrame);
            #endregion

            #region Window
            m_WindowContainer.Add(m_WindowOpening);
            m_WindowContainer.Add(m_WindowFoldout);
            m_WindowFoldout.Add(m_Window);
            #endregion
        }
        protected override void BindFields()
        {
            m_WallElement.BindProperty(m_Props.WallElement);

            #region Doorway
            var doorway = m_Props.Doorway;
            var door = m_Props.Door;
            var doorFrame = m_Props.Frame;

            m_Doorway.BindProperty(doorway.Data);
            m_ActiveDoorwayElements.BindProperty(doorway.ActiveElements);
            m_Door.BindProperty(door.Data);
            m_DoorFrame.BindProperty(doorFrame.Data);
            #endregion

            #region Archway
            var archway = m_Props.Archway;
            var archDoor = m_Props.ArchDoor;
            var archDoorFrame = m_Props.Frame;

            m_Archway.BindProperty(archway.Data);
            m_ArchDoor.BindProperty(archDoor.Data);
            m_ArchDoorFrame.BindProperty(archDoorFrame.Data);
            m_ActiveArchwayElements.BindProperty(archway.ActiveElements);
            #endregion

            #region Window
            var opening = m_Props.WindowOpening;
            var window = m_Props.Window;

            m_WindowOpening.BindProperty(opening.Data);
            m_Window.BindProperty(window.Data);
            #endregion

        }
        protected override void DefineFields()
        {
            m_WallElement = new PropertyField() { label = "Wall Element" };
            m_OpeningContainer = new VisualElement();
            m_DoorwayContainer = new VisualElement();
            m_ArchwayContainer = new VisualElement();
            m_WindowContainer = new VisualElement();

            #region Doorway
            m_Doorway = new PropertyField();
            m_ActiveDoorwayElements = new PropertyField() { label = "Active Elements" };
            m_DoorFoldout = new Foldout() { text = "Door" };
            m_Door = new PropertyField();
            m_DoorFrame = new PropertyField();
            #endregion

            #region Archway
            m_Archway = new PropertyField();
            m_ActiveArchwayElements = new PropertyField(){ label = "Active Elements" };
            m_ArchDoorFoldout = new Foldout() { text = "Door" };
            m_ArchDoor = new PropertyField();
            m_ArchDoorFrame= new PropertyField();
            #endregion

            #region Window
            m_WindowOpening = new PropertyField();
            m_WindowFoldout = new Foldout() { text = "Windows"};
            m_Window = new PropertyField();
            #endregion
        }
        protected override void Initialize(SerializedProperty data)
        {
            m_Props = new WallSectionDataSerializedProperties(data);
            m_CurrentData = data.GetUnderlyingValue() as WallSectionData;
            m_PreviousData = m_CurrentData.Clone() as WallSectionData;
            m_PreviousElement = m_CurrentData.WallElement;
        }
        protected override void RegisterValueChangeCallbacks()
        {
            m_WallElement.RegisterValueChangeCallback(evt =>
            {
                if ((m_CurrentData.WallElement == m_PreviousElement) && m_OpeningContainer.childCount > 0)
                    return;

                if (m_CurrentData.WallElement != m_PreviousElement)
                {
                    m_CurrentData.IsDirty = true;
                    m_PreviousElement = m_CurrentData.WallElement;
                }

                m_OpeningContainer.Clear();

                switch(m_CurrentData.WallElement)
                {
                    case WallElement.Wall:
                        break;
                    case WallElement.Doorway:
                        m_OpeningContainer.Add(m_DoorwayContainer);
                        break;
                    case WallElement.Archway:
                        m_OpeningContainer.Add(m_ArchwayContainer);
                        break;
                    case WallElement.Window:
                        m_OpeningContainer.Add(m_WindowContainer);
                        break;
                }


            });

            #region Doorway
            m_Doorway.RegisterValueChangeCallback(evt =>
            {
                DoorwayData doorwayData = evt.changedProperty.GetUnderlyingValue() as DoorwayData;

                if (doorwayData.Equals(m_PreviousData.Doorway))
                    return;

                m_CurrentData.IsDirty = true;

            });
            m_ActiveDoorwayElements.RegisterValueChangeCallback(evt =>
            {
                bool isDoorActive = m_CurrentData.Doorway.ActiveElements.IsElementActive(DoorwayElement.Door);
                bool isFrameActive = m_CurrentData.Doorway.ActiveElements.IsElementActive(DoorwayElement.Frame);

                m_Door.SetEnabled(isDoorActive);
                m_DoorFrame.SetEnabled(isFrameActive);

                if (m_CurrentData.Doorway.ActiveElements == m_PreviousData.Doorway.ActiveElements)
                    return;

                m_PreviousData.Doorway.ActiveElements = m_CurrentData.Doorway.ActiveElements;

                foreach (DoorData door in m_CurrentData.Doorway.Doors)
                {
                    door.ActiveElements = m_CurrentData.Doorway.ActiveElements.ToDoorElement();
                }

            });
            m_DoorFrame.RegisterValueChangeCallback(evt =>
            {
                FrameData frameData = evt.changedProperty.GetUnderlyingValue() as FrameData;

                if (frameData.Equals(m_PreviousData.DoorFrame))
                    return;

                m_CurrentData.IsDirty = true;
            });
            #endregion

            #region Archway
            m_Archway.RegisterValueChangeCallback(evt =>
            {
                ArchwayData archwayData = evt.changedProperty.GetUnderlyingValue() as ArchwayData;

                if (archwayData.Equals(m_PreviousData.Archway))
                    return;

                m_CurrentData.IsDirty = true;
            });
            m_ActiveArchwayElements.RegisterValueChangeCallback(evt =>
            {
                bool isDoorActive = m_CurrentData.Archway.ActiveElements.IsElementActive(DoorwayElement.Door);
                bool isFrameActive = m_CurrentData.Archway.ActiveElements.IsElementActive(DoorwayElement.Frame);

                m_ArchDoor.SetEnabled(isDoorActive);
                m_ArchDoorFrame.SetEnabled(isFrameActive);

                if (m_CurrentData.Archway.ActiveElements == m_PreviousData.Archway.ActiveElements)
                    return;

                m_PreviousData.Archway.ActiveElements = m_CurrentData.Archway.ActiveElements;

                foreach (DoorData archDoor in m_CurrentData.Archway.Doors)
                {
                    archDoor.ActiveElements = m_CurrentData.Archway.ActiveElements.ToDoorElement();
                }
            });
            m_ArchDoorFrame.RegisterValueChangeCallback(evt =>
            {
                FrameData frameData = evt.changedProperty.GetUnderlyingValue() as FrameData;

                if (frameData.Equals(m_PreviousData.DoorFrame))
                    return;

                m_CurrentData.IsDirty = true;
            });
            #endregion

            #region Window
            m_WindowOpening.RegisterValueChangeCallback(evt =>
            {
                WindowOpeningData openingData = evt.changedProperty.GetUnderlyingValue() as WindowOpeningData;

                if (m_PreviousData.WindowOpening.Equals(openingData))
                    return;

                m_PreviousData.WindowOpening = openingData.Clone() as WindowOpeningData;

                openingData.Windows = null;
                m_CurrentData.IsDirty = true;

            });
            m_Window.RegisterValueChangeCallback(evt =>
            {
                WindowData wallSectionWindow = evt.changedProperty.GetUnderlyingValue() as WindowData;

                if (m_PreviousData.Window.Equals(wallSectionWindow))
                    return;

                m_PreviousData.Window = wallSectionWindow.Clone() as WindowData;

                WindowData[] windows = m_CurrentData.WindowOpening.Windows;

                for (int i = 0; i < windows.Length; i++)
                {
                    WindowData window = windows[i];

                    window.ActiveElements = wallSectionWindow.ActiveElements;

                    if (wallSectionWindow.OuterFrame.IsDirty)
                    {
                        window.OuterFrame.Scale = wallSectionWindow.OuterFrame.Scale;
                        window.OuterFrame.Depth = wallSectionWindow.OuterFrame.Depth;
                        window.OuterFrame.IsHoleDirty = wallSectionWindow.OuterFrame.IsHoleDirty;
                        window.OuterFrame.IsDirty = wallSectionWindow.OuterFrame.IsDirty;
                    }

                    if (wallSectionWindow.InnerFrame.IsDirty)
                    {
                        window.InnerFrame.Scale = wallSectionWindow.InnerFrame.Scale;
                        window.InnerFrame.Depth = wallSectionWindow.InnerFrame.Depth;
                        window.InnerFrame.Columns = wallSectionWindow.InnerFrame.Columns;
                        window.InnerFrame.Rows = wallSectionWindow.InnerFrame.Rows;
                        window.InnerFrame.IsHoleDirty = wallSectionWindow.InnerFrame.IsHoleDirty;
                        window.InnerFrame.IsDirty = wallSectionWindow.InnerFrame.IsDirty;
                    }

                    if (wallSectionWindow.Pane.IsDirty)
                    {
                        window.Pane.Depth = wallSectionWindow.Pane.Depth;
                        window.Pane.IsDirty = wallSectionWindow.Pane.IsDirty;
                    }
                }

            });
            #endregion

        }
    }
}