using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Window : MonoBehaviour, IBuildable
{
    [SerializeReference] WindowData m_Data;

    [SerializeField] Frame m_OuterFrame;
    [SerializeField] GridFrame m_InnerFrame;
    [SerializeField] Pane m_Pane;
    [SerializeField] Door m_LeftShutter;
    [SerializeField] Door m_RightShutter;

    public WindowData Data => m_Data;

    public IBuildable Initialize(DirtyData data)
    {
        m_Data = data as WindowData;
        return this;
    }

    #region Create
    private Frame CreateOuterFrame()
    {
        ProBuilderMesh outerFrameMesh = ProBuilderMesh.Create();
        outerFrameMesh.name = "Outer Frame";
        outerFrameMesh.transform.SetParent(transform, false);
        Frame outerFrame = outerFrameMesh.AddComponent<Frame>();
        outerFrame.Initialize(CalculateOuterFrame());
        outerFrame.Data.IsDirty = true;
        return outerFrame;
    }
    private GridFrame CreateInnerFrame()
    {
        ProBuilderMesh innerFrameMesh = ProBuilderMesh.Create();
        innerFrameMesh.name = "Inner Frame";
        innerFrameMesh.transform.SetParent(transform, false);
        GridFrame innerFrame = innerFrameMesh.AddComponent<GridFrame>();
        innerFrame.Initialize(CalculateInnerFrame());
        innerFrame.Data.IsDirty = true;
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
        return pane;
    }
    private Door CreateLeftShutter(Vector3[] controlPoints)
    {
        ProBuilderMesh leftShutterMesh = ProBuilderMesh.Create();
        leftShutterMesh.name = "Left Shutter";
        leftShutterMesh.transform.SetParent(transform, false);
        Door leftShutter = leftShutterMesh.AddComponent<Door>();
        DoorData data = m_Data.LeftShutter;
        data.SetPolygon(controlPoints, m_Data.Polygon.Normal);
        leftShutter.Initialize(data);
        leftShutter.Data.IsDirty = true;
        return leftShutter;
    }
    private Door CreateRightShutter(Vector3[] controlPoints)
    {
        ProBuilderMesh rightShutterMesh = ProBuilderMesh.Create();
        rightShutterMesh.name = "Right Shutter";
        rightShutterMesh.transform.SetParent(transform, false);
        Door rightShutter = rightShutterMesh.AddComponent<Door>();
        DoorData data = m_Data.RightShutter;
        data.SetPolygon(controlPoints, m_Data.Polygon.Normal);
        rightShutter.Initialize(data);
        rightShutter.Data.IsDirty = true;
        return rightShutter;
    }
    #endregion

    #region Calculate
    private FrameData CalculateOuterFrame()
    {
        FrameData frameData = m_Data.OuterFrame;
        frameData.SetPolygon(m_Data.Polygon.ControlPoints, m_Data.Polygon.Normal);
        return frameData;
    }
    private GridFrameData CalculateInnerFrame()
    {
        GridFrameData frameData = m_Data.InnerFrame;
        Vector3[] controlPoints = m_Data.IsOuterFrameActive ? m_OuterFrame.Data.Holes[0].ControlPoints : m_Data.Polygon.ControlPoints;
        Vector3 normal = m_Data.Polygon.Normal;
        frameData.SetPolygon(controlPoints, normal);

        return frameData;
    }
    private Polygon3DData CalculatePane()
    {
        Polygon3DData pane = m_Data.Pane;
        Vector3[] controlPoints = m_Data.IsOuterFrameActive ? m_OuterFrame.Data.Holes[0].ControlPoints : m_Data.Polygon.ControlPoints;
        Vector3 normal = m_Data.Polygon.Normal;

        pane.SetPolygon(controlPoints, normal);

        return pane;
    }
    #endregion

    #region Build

    #region Rebuild
    public void Rebuild()
    {
        if (!m_Data.IsDirty)
            return;

        transform.DeleteChildren();

        RebuildOuterFrame();
        RebuildInnerFrame();
        RebuildPane();
        RebuildShutters();

        m_Data.IsDirty = false;
    }
    public void RebuildOuterFrame()
    {
        if(m_OuterFrame != null)
        {
            m_OuterFrame.Demolish();
        }

        BuildOuterFrame();
    }
    public void RebuildInnerFrame()
    {
        if(m_InnerFrame != null)
        {
            m_InnerFrame.Demolish();
        }

        BuildInnerFrame();
    }
    public void RebuildPane()
    {
        if(m_Pane != null)
        {
            m_Pane.Demolish();
        }

        BuildPane();
    }
    public void RebuildShutters()
    {
        if (!m_Data.AreShuttersActive && m_LeftShutter != null)
        {
            m_LeftShutter.Demolish();
        }
        if (!m_Data.AreShuttersActive && m_RightShutter != null)
        {
            m_RightShutter.Demolish();
        }

        BuildShutters();
    }
    #endregion

    public void BuildOuterFrame()
    {
        if (m_Data.IsOuterFrameActive && (m_OuterFrame == null || m_Data.OuterFrame.IsDirty))
        {
            m_OuterFrame = m_OuterFrame != null ? m_OuterFrame : CreateOuterFrame(); // What is we need to recalc the points
            m_OuterFrame.Build();
            m_OuterFrame.Data.IsDirty = false;
        }
    }
    public void BuildInnerFrame()
    {
        if (m_Data.IsInnerFrameActive && (m_InnerFrame == null || m_Data.InnerFrame.IsDirty))
        {
            m_InnerFrame = m_InnerFrame != null ? m_InnerFrame : CreateInnerFrame();
            m_InnerFrame.Build();
        }
    }
    public void BuildPane()
    {
        if (m_Data.IsPaneActive && (m_Pane == null || m_Data.Pane.IsDirty))
        {
            m_Pane = m_Pane != null ? m_Pane : CreatePane();
            m_Pane.Build();
        }
    }
    public void BuildShutters()
    {
        if (m_Data.AreShuttersActive && (m_LeftShutter == null || m_RightShutter == null || m_Data.LeftShutter.IsDirty || m_Data.RightShutter.IsDirty))
        {
            IList<IList<Vector3>> shutterControlPoints;
            Vector3[] points = m_Data.IsOuterFrameActive ? m_OuterFrame.Data.Holes[0].ControlPoints : m_Data.Polygon.ControlPoints;

            float height = m_Data.IsOuterFrameActive ? m_InnerFrame.Data.Height : m_OuterFrame.Data.Height;
            float width = m_Data.IsOuterFrameActive ? m_InnerFrame.Data.Width : m_OuterFrame.Data.Width;
            float depth = m_Data.IsOuterFrameActive ? m_InnerFrame.Data.Depth : m_OuterFrame.Data.Depth;
            Vector3 position = m_Data.IsOuterFrameActive ? m_InnerFrame.Data.Position : m_OuterFrame.Data.Position;

            for(int i = 0; i < points.Length; i++)
            {
                points[i] += m_Data.Polygon.Normal * depth;
            }

            position += m_Data.Polygon.Normal * depth;

            shutterControlPoints = MeshMaker.SpiltPolygon(points, width, height, 2, 1, position, m_Data.Polygon.Normal);

            m_LeftShutter = m_LeftShutter != null ? m_LeftShutter : CreateLeftShutter(shutterControlPoints[1].ToArray());
            m_LeftShutter.Build();

            m_RightShutter = m_RightShutter != null ? m_RightShutter : CreateRightShutter(shutterControlPoints[0].ToArray());
            m_RightShutter.Build();

        }
    }
    public void Build()
    {
        Demolish();

        if (m_Data.ActiveElements == WindowElement.Nothing)
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
    public void Demolish()
    {
        if (!m_Data.IsOuterFrameActive && m_OuterFrame != null)
        {
            m_OuterFrame.Demolish();
        }
        if (!m_Data.IsInnerFrameActive && m_InnerFrame != null)
        {
            m_InnerFrame.Demolish();
        }
        if (!m_Data.IsPaneActive && m_Pane != null)
        {
            m_Pane.Demolish();
        }
        if (!m_Data.AreShuttersActive && m_LeftShutter != null)
        {
            m_LeftShutter.Demolish();
        }
        if (!m_Data.AreShuttersActive && m_RightShutter != null)
        {
            m_RightShutter.Demolish();
        }
    }
}
