using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using OnlyInvalid.ProcGenBuilding.Door;
using OnlyInvalid.ProcGenBuilding.Wall;

namespace OnlyInvalid.ProcGenBuilding.Window
{
    public class Window : Buildable
    {
        [SerializeReference] WindowData m_WindowData;

        [SerializeField] OuterFrame m_OuterFrame;
        [SerializeField] InnerFrame m_InnerFrame; // Thought: This is a grid. Could we have different inner frame types? Pyramid Frame?
        [SerializeField] Pane m_Pane;
        [SerializeField] Door.Door m_LeftShutter;
        [SerializeField] Door.Door m_RightShutter;

        public override Buildable Initialize(DirtyData data)
        {
            m_WindowData = data as WindowData;
            return this;
        }

        #region Create
        private OuterFrame CreateOuterFrame()
        {
            ProBuilderMesh outerFrameMesh = ProBuilderMesh.Create();
            outerFrameMesh.name = "Outer Frame";
            outerFrameMesh.transform.SetParent(transform, false);
            OuterFrame outerFrame = outerFrameMesh.AddComponent<OuterFrame>();
            outerFrame.Initialize(CalculateOuterFrame());
            outerFrame.Data.IsDirty = true;
            outerFrame.AddListener(data =>
            {
                m_WindowData.OuterFrame = data as OuterFrameData;
                m_OnDataChanged.Invoke(m_WindowData);
            });
            return outerFrame;
        }
        private InnerFrame CreateInnerFrame()
        {
            ProBuilderMesh innerFrameMesh = ProBuilderMesh.Create();
            innerFrameMesh.name = "Inner Frame";
            innerFrameMesh.transform.SetParent(transform, false);
            InnerFrame innerFrame = innerFrameMesh.AddComponent<InnerFrame>();
            innerFrame.Initialize(CalculateInnerFrame());
            innerFrame.Data.IsDirty = true;
            innerFrame.AddListener(data =>
            {
                m_WindowData.InnerFrame = data as InnerFrameData;
                m_OnDataChanged.Invoke(m_WindowData);
            });
            return innerFrame;
        }
        private Pane CreatePane()
        {
            ProBuilderMesh paneMesh = ProBuilderMesh.Create();
            paneMesh.name = "Pane";
            paneMesh.transform.SetParent(transform, false);
            Pane pane = paneMesh.AddComponent<Pane>();
            pane.Initialize(CalculatePane());
            pane.Data.IsDirty = true;
            pane.AddListener(data =>
            {
                m_WindowData.Pane = data as PaneData;
                m_OnDataChanged.Invoke(m_WindowData);
            });
            return pane;
        }
        private Door.Door CreateLeftShutter(Vector3[] controlPoints)
        {
            ProBuilderMesh leftShutterMesh = ProBuilderMesh.Create();
            leftShutterMesh.name = "Left Shutter";
            leftShutterMesh.transform.SetParent(transform, false);
            Door.Door leftShutter = leftShutterMesh.AddComponent<Door.Door>();
            DoorData data = m_WindowData.LeftShutter;
            data.SetPolygon(controlPoints, m_WindowData.Polygon.Normal);
            leftShutter.Initialize(data);
            leftShutter.Data.IsDirty = true;
            leftShutter.AddListener(data =>
            {
                m_WindowData.LeftShutter = data as DoorData;
                m_OnDataChanged.Invoke(m_WindowData);
            });
            return leftShutter;
        }
        private Door.Door CreateRightShutter(Vector3[] controlPoints)
        {
            ProBuilderMesh rightShutterMesh = ProBuilderMesh.Create();
            rightShutterMesh.name = "Right Shutter";
            rightShutterMesh.transform.SetParent(transform, false);
            Door.Door rightShutter = rightShutterMesh.AddComponent<Door.Door>();
            rightShutter.Initialize(CalculateShutter(controlPoints));
            rightShutter.AddListener(data =>
            {
                m_WindowData.RightShutter = data as DoorData;
                m_OnDataChanged.Invoke(m_WindowData);
            });
            return rightShutter;
        }
        private DoorData CalculateShutter(Vector3[] controlPoints)
        {
            DoorData shutterData = m_WindowData.RightShutter;
            shutterData.SetPolygon(controlPoints, m_WindowData.Polygon.Normal);
            shutterData.IsDirty = true;
            return shutterData;
        }

        #endregion

        #region Calculate
        private OuterFrameData CalculateOuterFrame()
        {
            OuterFrameData frameData = m_WindowData.OuterFrame.Clone() as OuterFrameData;
            frameData.SetPolygon(m_WindowData.Polygon.ControlPoints, m_WindowData.Polygon.Normal);
            frameData.IsDirty = true;
            return frameData;
        }
        private InnerFrameData CalculateInnerFrame()
        {
            InnerFrameData frameData = m_WindowData.InnerFrame.Clone() as InnerFrameData;
            Vector3[] controlPoints = m_WindowData.IsOuterFrameActive && m_OuterFrame != null ? m_OuterFrame.OuterFrameData.Holes[0].ControlPoints : m_WindowData.Polygon.ControlPoints;
            Vector3 normal = m_WindowData.Polygon.Normal;
            frameData.SetPolygon(controlPoints, normal);
            frameData.IsDirty = true;
            return frameData;
        }
        private PaneData CalculatePane()
        {
            PaneData pane = m_WindowData.Pane.Clone() as PaneData;
            Vector3[] controlPoints = m_WindowData.IsOuterFrameActive && m_OuterFrame != null ? m_OuterFrame.OuterFrameData.Holes[0].ControlPoints : m_WindowData.Polygon.ControlPoints;
            Vector3 normal = m_WindowData.Polygon.Normal;
            pane.SetPolygon(controlPoints, normal);
            pane.IsDirty = true;
            return pane;
        }
        #endregion

        #region Build
        public void Rebuild()
        {
            if (!m_WindowData.IsDirty)
                return;

            transform.DeleteChildren();

            m_WindowData.OuterFrame.IsDirty = true;
            m_WindowData.InnerFrame.IsDirty = true;
            m_WindowData.Pane.IsDirty = true;
            m_WindowData.LeftShutter.IsDirty = true;
            m_WindowData.RightShutter.IsDirty = true;

            BuildOuterFrame();
            BuildInnerFrame();
            BuildPane();
            BuildShutters();

            m_WindowData.IsDirty = false;
        }
        public void BuildOuterFrame()
        {
            if (!m_WindowData.IsOuterFrameActive)
                return;

            if (!m_WindowData.OuterFrame.IsDirty)
                return;

            m_OuterFrame = m_OuterFrame == null ? CreateOuterFrame() : m_OuterFrame;
            m_OuterFrame.Initialize(CalculateOuterFrame()).Build();
            m_WindowData.OuterFrame.IsDirty = false;
        }
        public void BuildInnerFrame()
        {
            if (!m_WindowData.IsInnerFrameActive)
                return;

            if (!m_WindowData.InnerFrame.IsDirty)
                return;

            m_InnerFrame = m_InnerFrame == null ? CreateInnerFrame() : m_InnerFrame;
            m_InnerFrame.Initialize(CalculateInnerFrame()).Build();
            m_WindowData.InnerFrame.IsDirty = false;
        }
        public void BuildPane()
        {
            if (!m_WindowData.IsPaneActive)
                return;

            if (!m_WindowData.Pane.IsDirty)
                return;

            m_Pane = m_Pane == null ? CreatePane() : m_Pane;
            m_Pane.Initialize(CalculatePane()).Build();
            m_WindowData.Pane.IsDirty = false;
        }
        public void BuildShutters()
        {
            if (!m_WindowData.AreShuttersActive)
                return;

            if (!m_WindowData.LeftShutter.IsDirty && !m_WindowData.RightShutter.IsDirty)
                return;

            if(m_WindowData.IsOuterFrameActive && m_OuterFrame == null)
            {
                m_WindowData.OuterFrame.IsDirty = true;
                BuildOuterFrame();
            }

            if (m_WindowData.IsInnerFrameActive && m_InnerFrame == null)
            {
                m_WindowData.InnerFrame.IsDirty = true;
                BuildInnerFrame();
            }

            IList<IList<Vector3>> shutterControlPoints;

            Vector3[] points = m_WindowData.IsOuterFrameActive ? m_OuterFrame.OuterFrameData.Holes[0].ControlPoints : m_WindowData.Polygon.ControlPoints;

            float height = m_WindowData.IsOuterFrameActive ? m_InnerFrame.InnerFrameData.Height : m_OuterFrame.OuterFrameData.Height;
            float width = m_WindowData.IsOuterFrameActive ? m_InnerFrame.InnerFrameData.Width : m_OuterFrame.OuterFrameData.Width;
            float depth = m_WindowData.IsOuterFrameActive ? m_InnerFrame.InnerFrameData.Depth : m_OuterFrame.OuterFrameData.Depth;
            Vector3 position = m_WindowData.IsOuterFrameActive ? m_InnerFrame.InnerFrameData.Position : m_OuterFrame.OuterFrameData.Position;

            for (int i = 0; i < points.Length; i++)
            {
                points[i] += m_WindowData.Polygon.Normal * depth;
            }

            position += m_WindowData.Polygon.Normal * depth;

            shutterControlPoints = MeshMaker.SpiltPolygon(points, width, height, 2, 1, position, m_WindowData.Polygon.Normal);

            Vector3[] leftPoints = shutterControlPoints[1].ToArray();
            Vector3[] rightPoints = shutterControlPoints[0].ToArray();

            m_LeftShutter = m_LeftShutter == null ? CreateLeftShutter(leftPoints) : m_LeftShutter;
            m_LeftShutter.Initialize(CalculateShutter(leftPoints)).Build();
            m_WindowData.LeftShutter.IsDirty = false;

            m_RightShutter = m_RightShutter == null ? CreateRightShutter(rightPoints) : m_RightShutter;
            m_RightShutter.Initialize(CalculateShutter(rightPoints)).Build();
            m_WindowData.RightShutter.IsDirty = false;


        }
        public override void Build()
        {
            if (m_WindowData == null)
                return;

            Demolish();

            if (m_WindowData.ActiveElements == WindowElement.Nothing)
                return;

            BuildOuterFrame();
            BuildInnerFrame();
            BuildPane();
            BuildShutters();

            // If window data is dirty rebuild all.
            // Changes to the outer frame will cause window data to be dirty.

            Rebuild();

        }
        #endregion

        private void OnValidate()
        {
            m_OnDataChanged.Invoke(m_WindowData);
        }

        /// <summary>
        /// This method removes only the window components that are inactive
        /// </summary>
        public override void Demolish()
        {

            if (!m_WindowData.IsOuterFrameActive && m_OuterFrame != null)
            {
                m_OuterFrame.Demolish();
            }
            if (!m_WindowData.IsInnerFrameActive && m_InnerFrame != null)
            {
                m_InnerFrame.Demolish();
            }
            if (!m_WindowData.IsPaneActive && m_Pane != null)
            {
                m_Pane.Demolish();
            }
            if (!m_WindowData.AreShuttersActive && m_LeftShutter != null)
            {
                m_LeftShutter.Demolish();
            }
            if (!m_WindowData.AreShuttersActive && m_RightShutter != null)
            {
                m_RightShutter.Demolish();
            }
        }
    }
}
