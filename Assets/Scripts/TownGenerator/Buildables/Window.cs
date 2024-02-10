using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UIElements;

public class Window : MonoBehaviour, IBuildable
{
    [SerializeReference] private WindowData m_Data;

    [SerializeField] private ProBuilderMesh m_OuterFrame;
    [SerializeField] private ProBuilderMesh m_InnerFrame;
    [SerializeField] private ProBuilderMesh m_Pane;
    [SerializeField] private ProBuilderMesh m_LeftShutter, m_RightShutter;

    public WindowData Data => m_Data;

    public IBuildable Initialize(IData data)
    {
        m_Data = data as WindowData;
        
        // Height variable may need calculating based on the forward vector
        Extensions.MinMax(m_Data.ControlPoints, out Vector3 min, out Vector3 max);
        m_Data.Height = max.y - min.y;
        m_Data.Width = max.x - min.x + (max.z - min.z);
        m_Data.Position = Vector3.Lerp(min, max, 0.5f);

        return this;
    }

    public void SetPosition(Vector3 position)
    {
        if(m_OuterFrame != null)
        {
            m_OuterFrame.transform.position = position;
            m_OuterFrame.LocaliseVertices();
            m_OuterFrame.Refresh();
        }

        if(m_InnerFrame != null)
        {
            m_InnerFrame.transform.position = position;
            m_InnerFrame.LocaliseVertices();
            m_InnerFrame.Refresh();
        }
        if(m_Pane != null)
        {
            m_Pane.transform.position = position;
            m_Pane.LocaliseVertices();
            m_Pane.Refresh();
        }
        if(m_LeftShutter != null)
        {
            m_LeftShutter.transform.position = position;
            m_LeftShutter.LocaliseVertices();
            m_LeftShutter.Refresh();
        }
        if (m_RightShutter != null)
        {
            m_RightShutter.transform.position = position;
            m_RightShutter.LocaliseVertices();
            m_RightShutter.Refresh();
        }
    }

    #region Calculate
    public static IList<IList<Vector3>> CalculateOuterFrame(WindowData data)
    {
        IList<IList<Vector3>> holePoints = new List<IList<Vector3>>();
        holePoints.Add(data.ControlPoints.ScalePolygon(data.OuterFrameScale, data.Position));
        return holePoints;
    }
    public static IList<IList<Vector3>> CalculateInnerFrame(WindowData data)
    {
        Vector3[] points = data.IsOuterFrameActive ? data.ControlPoints.ScalePolygon(data.OuterFrameScale, data.Position) : data.ControlPoints;

        return MeshMaker.SpiltPolygon(points, data.Width, data.Height, data.InnerFrameColumns, data.InnerFrameRows, data.Position, data.Forward);
    }
    private DoorData CalculateRightShutterData(IEnumerable<Vector3> controlPoints)
    {
        Vector3 right = Vector3.Cross(m_Data.Forward, Vector3.up);

        DoorData rightShutterData = new DoorData()
        {
            ControlPoints = controlPoints.ToArray(),
            Forward = m_Data.Forward,
            Right = right,
            Depth = m_Data.ShuttersDepth,
            Scale = 1,
            HingePoint = TransformPoint.Right,
            HingeEulerAngles = -Vector3.up * m_Data.ShuttersAngle,
            ActiveElements = DoorElement.Everything,
            Material = m_Data.ShuttersMaterial
        };

        return rightShutterData;
    }
    private DoorData CalculateLeftShutterData(IEnumerable<Vector3> controlPoints)
    {
        Vector3 right = Vector3.Cross(m_Data.Forward, Vector3.up);

        DoorData leftShutterData = new DoorData()
        {
            ControlPoints = controlPoints.ToArray(),
            Forward = m_Data.Forward,
            Right = right,
            Depth = m_Data.ShuttersDepth,
            Scale = 1,
            HingePoint = TransformPoint.Left,
            HingeEulerAngles = Vector3.up * m_Data.ShuttersAngle,
            ActiveElements = DoorElement.Everything,
            Material = m_Data.ShuttersMaterial
        };

        return leftShutterData;
    }
    #endregion

    #region Build
    private ProBuilderMesh BuildOuterFrame()
    {
        ProBuilderMesh outerFrame = ProBuilderMesh.Create();
        outerFrame.name = "Outer Frame";
        outerFrame.transform.SetParent(transform, false);
        outerFrame.GetComponent<Renderer>().sharedMaterial = m_Data.OuterFrameMaterial;
        return outerFrame;
    }
    private ProBuilderMesh BuildInnerFrame()
    {
        ProBuilderMesh innerFrame = ProBuilderMesh.Create();
        innerFrame.name = "Inner Frame";
        innerFrame.transform.SetParent(transform, false);
        innerFrame.GetComponent<Renderer>().sharedMaterial = m_Data.InnerFrameMaterial;
        return innerFrame;
    }
    private ProBuilderMesh BuildPane()
    {
        ProBuilderMesh pane = ProBuilderMesh.Create();
        pane.transform.SetParent(transform, false);
        pane.name = "Pane";
        pane.GetComponent<Renderer>().sharedMaterial = m_Data.PaneMaterial;
        return pane;
    }
    private ProBuilderMesh BuildLeftShutter()
    {
        ProBuilderMesh leftShutter = ProBuilderMesh.Create();
        leftShutter.name = "Left Shutter";
        leftShutter.transform.SetParent(transform, false);
        return leftShutter;
    }
    private ProBuilderMesh BuildRightShutter()
    {
        ProBuilderMesh rightShutter = ProBuilderMesh.Create();
        rightShutter.name = "Right Shutter";
        rightShutter.transform.SetParent(transform, false);
        return rightShutter;
    }
    public void Build()
    {
        if (m_Data.ActiveElements == WindowElement.Nothing)
            return;

        Vector3[] points = m_Data.IsOuterFrameActive ? CalculateOuterFrame(m_Data)[0].ToArray() : m_Data.ControlPoints;

        if (m_Data.IsOuterFrameActive && (m_OuterFrame == null || m_OuterFrame.positions.Count == 0 || m_Data.DoesOuterFrameNeedRebuild))
        {
            m_OuterFrame ??= BuildOuterFrame();

            IList<IList<Vector3>> holePoints = CalculateOuterFrame(m_Data);
            m_OuterFrame.CreateShapeFromPolygon(m_Data.ControlPoints, m_Data.Forward, holePoints);
            m_OuterFrame.Solidify(m_Data.OuterFrameDepth);
            m_Data.DoesOuterFrameNeedRebuild = false;
        }

        if (m_Data.IsInnerFrameActive && (m_InnerFrame == null || m_InnerFrame.positions.Count == 0 || m_Data.DoesInnerFrameNeedRebuild))
        {
            // Inner Frame
            m_InnerFrame ??= BuildInnerFrame();

            m_Data.InnerFrameHolePoints = CalculateInnerFrame(m_Data);

            IList<IList<Vector3>> holePoints = new List<IList<Vector3>>();

            foreach(IList<Vector3> holePoint in m_Data.InnerFrameHolePoints)
            {
                holePoints.Add(holePoint.ScalePolygon(m_Data.InnerFrameScale));
            }

            m_InnerFrame.CreateShapeFromPolygon(points, m_Data.Forward, holePoints);
            m_InnerFrame.Solidify(m_Data.InnerFrameDepth);
            m_Data.DoesInnerFrameNeedRebuild = false;
        }

        if (m_Data.IsPaneActive && (m_Pane == null || m_Pane.positions.Count == 0 || m_Data.DoesPaneNeedRebuild))
        {
            m_Pane ??= BuildPane();
            m_Pane.CreateShapeFromPolygon(points, m_Data.Forward);
            m_Pane.Solidify(m_Data.PaneDepth);
            m_Data.DoesPaneNeedRebuild = false;
        }

        if(m_Data.AreShuttersActive && (m_LeftShutter == null || m_LeftShutter.positions.Count == 0 ||
                                        m_RightShutter == null || m_RightShutter.positions.Count == 0 ||
                                        m_Data.DoShuttersNeedRebuild))
        {
            IList<IList<Vector3>> shutterVertices;

            if (m_LeftShutter == null || m_RightShutter == null)
            {
                shutterVertices = MeshMaker.SpiltPolygon(points, m_Data.Width, m_Data.Height, 2, 1, m_Data.Position, m_Data.Forward);

                //for (int i = 0; i < shutterVertices.Count; i++)
                //{
                //    IList<Vector3> shutter = shutterVertices[i];

                //    for (int j = 0; j < shutter.Count; j++)
                //    {
                //        shutter[j] += m_Data.Forward * m_Data.OuterFrameDepth;
                //    }

                //    shutterVertices[i] = shutter;
                //}
            }
            else
            {
                shutterVertices = new List<IList<Vector3>>();

                shutterVertices.Add(m_RightShutter.GetComponent<Door>().Data.ControlPoints);
                shutterVertices.Add(m_LeftShutter.GetComponent<Door>().Data.ControlPoints);
            }

            m_RightShutter ??= BuildRightShutter();
            m_LeftShutter ??= BuildLeftShutter();

            m_RightShutter.transform.localPosition = Vector3.zero + (m_Data.Forward * m_Data.OuterFrameDepth);
            m_LeftShutter.transform.localPosition = Vector3.zero + (m_Data.Forward * m_Data.OuterFrameDepth);

            Door rightShutter = m_RightShutter.GetOrAddComponent<Door>();
            Door leftShutter = m_LeftShutter.GetOrAddComponent<Door>();

            if (rightShutter.Data == null)
            {
                rightShutter.Initialize(CalculateRightShutterData(shutterVertices[0]));
            }
            else
            {
                rightShutter.Data.Depth = m_Data.ShuttersDepth;
                rightShutter.Data.HingeEulerAngles = -Vector3.up * m_Data.ShuttersAngle;
            }
            
            if(leftShutter.Data == null)
            {
                leftShutter.Initialize(CalculateLeftShutterData(shutterVertices[1]));
            }
            else
            {
                leftShutter.Data.Depth = m_Data.ShuttersDepth;
                leftShutter.Data.HingeEulerAngles = Vector3.up * m_Data.ShuttersAngle;
            }

            rightShutter.Build();
            leftShutter.Build();

            m_Data.DoShuttersNeedRebuild = false;
        }
    }
    #endregion
    /// <summary>
    /// This method removes only the window components that are inactive
    /// </summary>
    public void Demolish()
    {
        if (!m_Data.IsOuterFrameActive && m_OuterFrame != null)
        {
            m_OuterFrame.Clear();
            m_OuterFrame.ToMesh();
            m_OuterFrame.Refresh();
        }
        if (!m_Data.IsInnerFrameActive && m_InnerFrame != null)
        {
            m_InnerFrame.Clear();
            m_InnerFrame.ToMesh();
            m_InnerFrame.Refresh();
        }
        if (!m_Data.IsPaneActive && m_Pane != null)
        {
            m_Pane.Clear();
            m_Pane.ToMesh();
            m_Pane.Refresh();
        }
        if (!m_Data.AreShuttersActive && m_LeftShutter != null)
        {
            m_LeftShutter.Clear();
            m_LeftShutter.ToMesh();
            m_LeftShutter.Refresh();
        }
        if (!m_Data.AreShuttersActive && m_RightShutter != null)
        {
            m_RightShutter.Clear();
            m_RightShutter.ToMesh();
            m_RightShutter.Refresh();
        }
    }
}
