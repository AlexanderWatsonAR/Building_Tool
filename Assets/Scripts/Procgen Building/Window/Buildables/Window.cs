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
        [SerializeField] InnerFrame m_InnerFrame;
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
            outerFrame.AddDataListener(data => 
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
            innerFrame.AddDataListener(data =>
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
            pane.AddDataListener(data =>
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
            leftShutter.AddDataListener(data =>
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
            DoorData data = m_WindowData.RightShutter;
            data.SetPolygon(controlPoints, m_WindowData.Polygon.Normal);
            rightShutter.Initialize(data);
            rightShutter.Data.IsDirty = true;
            rightShutter.AddDataListener(data =>
            {
                m_WindowData.RightShutter = data as DoorData;
                m_OnDataChanged.Invoke(m_WindowData);
            });
            return rightShutter;
        }
        #endregion

        #region Calculate
        private OuterFrameData CalculateOuterFrame()
        {
            OuterFrameData frameData = m_WindowData.OuterFrame;
            frameData.SetPolygon(m_WindowData.Polygon.ControlPoints, m_WindowData.Polygon.Normal);
            return frameData;
        }
        private InnerFrameData CalculateInnerFrame()
        {
            InnerFrameData frameData = m_WindowData.InnerFrame;
            OuterFrameData outerFrameData = m_OuterFrame.Data as OuterFrameData;
            Vector3[] controlPoints = m_WindowData.IsOuterFrameActive ? outerFrameData.Holes[0].ControlPoints : m_WindowData.Polygon.ControlPoints;
            Vector3 normal = m_WindowData.Polygon.Normal;
            frameData.SetPolygon(controlPoints, normal);

            return frameData;
        }
        private PaneData CalculatePane()
        {
            PaneData pane = m_WindowData.Pane;
            OuterFrameData outerFrameData = m_OuterFrame.Data as OuterFrameData;
            Vector3[] controlPoints = m_WindowData.IsOuterFrameActive ? outerFrameData.Holes[0].ControlPoints : m_WindowData.Polygon.ControlPoints;
            Vector3 normal = m_WindowData.Polygon.Normal;

            pane.SetPolygon(controlPoints, normal);

            return pane;
        }
        #endregion

        #region Build

        #region Rebuild
        public void Rebuild()
        {
            if (!m_WindowData.IsDirty)
                return;

            transform.DeleteChildren();

            RebuildOuterFrame();
            RebuildInnerFrame();
            RebuildPane();
            RebuildShutters();

            m_WindowData.IsDirty = false;
        }
        public void RebuildOuterFrame()
        {
            if (m_OuterFrame != null)
            {
                m_OuterFrame.Demolish();
            }

            BuildOuterFrame();
        }
        public void RebuildInnerFrame()
        {
            if (m_InnerFrame != null)
            {
                m_InnerFrame.Demolish();
            }

            BuildInnerFrame();
        }
        public void RebuildPane()
        {
            if (m_Pane != null)
            {
                m_Pane.Demolish();
            }

            BuildPane();
        }
        public void RebuildShutters()
        {
            if (!m_WindowData.AreShuttersActive && m_LeftShutter != null)
            {
                m_LeftShutter.Demolish();
            }
            if (!m_WindowData.AreShuttersActive && m_RightShutter != null)
            {
                m_RightShutter.Demolish();
            }

            BuildShutters();
        }
        #endregion

        public void BuildOuterFrame()
        {
            if (m_WindowData.IsOuterFrameActive && (m_OuterFrame == null || m_WindowData.OuterFrame.IsDirty))
            {
                m_OuterFrame = m_OuterFrame != null ? m_OuterFrame : CreateOuterFrame(); // What is we need to recalc the points
                m_OuterFrame.Build();
                m_OuterFrame.Data.IsDirty = false;
            }
        }
        public void BuildInnerFrame()
        {
            if (m_WindowData.IsInnerFrameActive && (m_InnerFrame == null || m_WindowData.InnerFrame.IsDirty))
            {
                m_InnerFrame = m_InnerFrame != null ? m_InnerFrame : CreateInnerFrame();
                m_InnerFrame.Build();
            }
        }
        public void BuildPane()
        {
            if (m_WindowData.IsPaneActive && (m_Pane == null || m_WindowData.Pane.IsDirty))
            {
                m_Pane = m_Pane != null ? m_Pane : CreatePane();
                m_Pane.Build();
            }
        }
        public void BuildShutters()
        {
            if (m_WindowData.AreShuttersActive && (m_LeftShutter == null || m_RightShutter == null || m_WindowData.LeftShutter.IsDirty || m_WindowData.RightShutter.IsDirty))
            {
                IList<IList<Vector3>> shutterControlPoints;

                GridFrameData innerFrame = m_InnerFrame.Data as GridFrameData;
                FrameData outerFrame = m_OuterFrame.Data as FrameData;

                Vector3[] points = m_WindowData.IsOuterFrameActive ? outerFrame.Holes[0].ControlPoints : m_WindowData.Polygon.ControlPoints;

                float height = m_WindowData.IsOuterFrameActive ? innerFrame.Height : outerFrame.Height;
                float width = m_WindowData.IsOuterFrameActive ? innerFrame.Width : outerFrame.Width;
                float depth = m_WindowData.IsOuterFrameActive ? innerFrame.Depth : outerFrame.Depth;
                Vector3 position = m_WindowData.IsOuterFrameActive ? innerFrame.Position : outerFrame.Position;

                for (int i = 0; i < points.Length; i++)
                {
                    points[i] += m_WindowData.Polygon.Normal * depth;
                }

                position += m_WindowData.Polygon.Normal * depth;

                shutterControlPoints = MeshMaker.SpiltPolygon(points, width, height, 2, 1, position, m_WindowData.Polygon.Normal);

                m_LeftShutter = m_LeftShutter != null ? m_LeftShutter : CreateLeftShutter(shutterControlPoints[1].ToArray());
                m_LeftShutter.Build();

                m_RightShutter = m_RightShutter != null ? m_RightShutter : CreateRightShutter(shutterControlPoints[0].ToArray());
                m_RightShutter.Build();

            }
        }
        public override void Build()
        {
            Demolish();

            if (m_WindowData.ActiveElements == WindowElement.Nothing)
                return;

            BuildOuterFrame();
            BuildInnerFrame();
            BuildPane();
            BuildShutters();

            Rebuild(); // If window data is dirty rebuild all

        }
        #endregion
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
