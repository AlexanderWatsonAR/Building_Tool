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
    [SerializeReference] WindowData m_Data;

    // Changes to the outer frame scale affect the inner frame, pane & doors.
    // It may wise to disable the scale in its prop drawer.
    [SerializeField] Frame m_OuterFrame;
    [SerializeField] GridFrame m_InnerFrame;
    [SerializeField] Pane m_Pane;
    [SerializeField] Door m_LeftShutter;
    [SerializeField] Door m_RightShutter;

    public WindowData Data => m_Data;

    public IBuildable Initialize(IData data)
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
        return outerFrame;
    }
    private GridFrame CreateInnerFrame()
    {
        ProBuilderMesh innerFrameMesh = ProBuilderMesh.Create();
        innerFrameMesh.name = "Inner Frame";
        innerFrameMesh.transform.SetParent(transform, false);
        GridFrame innerFrame = innerFrameMesh.AddComponent<GridFrame>();
        innerFrame.Initialize(CalculateInnerFrame());
        return innerFrame;
    }
    private Pane CreatePane()
    {
        ProBuilderMesh paneMesh = ProBuilderMesh.Create();
        paneMesh.name = "Pane";
        paneMesh.transform.SetParent(transform, false);
        Pane pane = paneMesh.AddComponent<Pane>();
        pane.Initialize(CalculatePane());
        return pane;
    }
    private Door CreateLeftShutter(Vector3[] controlPoints)
    {
        ProBuilderMesh leftShutterMesh = ProBuilderMesh.Create();
        leftShutterMesh.name = "Left Shutter";
        leftShutterMesh.transform.SetParent(transform, false);
        Door leftShutter = leftShutterMesh.AddComponent<Door>();
        DoorData data = m_Data.LeftShutter;
        data.Polygon.ControlPoints = controlPoints;
        leftShutter.Initialize(data);
        return leftShutter;
    }
    private Door CreateRightShutter(Vector3[] controlPoints)
    {
        ProBuilderMesh rightShutterMesh = ProBuilderMesh.Create();
        rightShutterMesh.name = "Right Shutter";
        rightShutterMesh.transform.SetParent(transform, false);
        Door rightShutter = rightShutterMesh.AddComponent<Door>();
        DoorData data = m_Data.RightShutter;
        data.Polygon.ControlPoints = controlPoints;
        rightShutter.Initialize(data);
        return rightShutter;
    }
    #endregion

    #region Calculate
    private FrameData CalculateOuterFrame()
    {
        FrameData frameData = m_Data.OuterFrame;
        frameData.Polygon.ControlPoints = m_Data.Polygon.ControlPoints;
        return frameData;
    }
    private GridFrameData CalculateInnerFrame()
    {
        GridFrameData frameData = m_Data.InnerFrame;
        frameData.Polygon.ControlPoints = m_Data.IsOuterFrameActive ? m_OuterFrame.Data.Holes[0].ControlPoints : m_Data.Polygon.ControlPoints;
        return frameData;
    }
    private Polygon3DData CalculatePane()
    {
        Polygon3DData pane = m_Data.Pane;
        pane.Polygon.ControlPoints = m_Data.IsOuterFrameActive ? m_OuterFrame.Data.Holes[0].ControlPoints : m_Data.Polygon.ControlPoints;
        return pane;
    }
    #endregion

    #region Build
    public void BuildOuterFrame()
    {
        if (m_Data.IsOuterFrameActive && (m_OuterFrame == null || m_Data.DoesOuterFrameNeedRebuild))
        {
            m_OuterFrame = m_OuterFrame != null ? m_OuterFrame : CreateOuterFrame();
            m_OuterFrame.Build();
            m_Data.DoesOuterFrameNeedRebuild = false;
        }
    }
    public void BuildInnerFrame()
    {
        if (m_Data.IsInnerFrameActive && (m_InnerFrame == null || m_Data.DoesInnerFrameNeedRebuild))
        {
            m_InnerFrame = m_InnerFrame != null ? m_InnerFrame : CreateInnerFrame();
            m_InnerFrame.Build();
            m_Data.DoesInnerFrameNeedRebuild = false;
        }
    }
    public void BuildPane()
    {
        if (m_Data.IsPaneActive && (m_Pane == null || m_Data.DoesPaneNeedRebuild))
        {
            m_Pane = m_Pane != null ? m_Pane : CreatePane();
            m_Data.DoesPaneNeedRebuild = false;
        }
    }
    public void BuildShutters()
    {
        if (m_Data.AreShuttersActive && (m_LeftShutter == null || m_RightShutter == null || m_Data.DoShuttersNeedRebuild))
        {
            IList<IList<Vector3>> shutterControlPoints;
            Vector3[] points = m_Data.IsOuterFrameActive ? m_OuterFrame.Data.Holes[0].ControlPoints : m_Data.Polygon.ControlPoints;

            float height = m_Data.IsOuterFrameActive ? m_InnerFrame.Data.Height : m_OuterFrame.Data.Height;
            float width = m_Data.IsOuterFrameActive ? m_InnerFrame.Data.Width : m_OuterFrame.Data.Width;
            Vector3 position = m_Data.IsOuterFrameActive ? m_InnerFrame.Data.Position : m_OuterFrame.Data.Position;

            shutterControlPoints = MeshMaker.SpiltPolygon(points, width, height, 2, 1, position, m_Data.Polygon.Normal);

            m_LeftShutter = m_LeftShutter != null ? m_LeftShutter : CreateLeftShutter(shutterControlPoints[1].ToArray());
            m_LeftShutter.Build();

            m_RightShutter = m_RightShutter != null ? m_RightShutter : CreateRightShutter(shutterControlPoints[0].ToArray());
            m_RightShutter.Build();

            m_Data.DoShuttersNeedRebuild = false;
        }
    }
    public void Build()
    {
        Demolish();

        if (m_Data.ActiveElements == WindowElement.Nothing)
            return;

        Debug.Log("Window build ", this);

        BuildOuterFrame();
        BuildInnerFrame();
        BuildPane();
        BuildShutters();

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
            DestroyImmediate(m_OuterFrame.gameObject);
        }
        if (!m_Data.IsInnerFrameActive && m_InnerFrame != null)
        {
            m_InnerFrame.Demolish();
            DestroyImmediate(m_InnerFrame.gameObject);
        }
        if (!m_Data.IsPaneActive && m_Pane != null)
        {
            m_Pane.Demolish();
            DestroyImmediate(m_Pane.gameObject);
        }
        if (!m_Data.AreShuttersActive && m_LeftShutter != null)
        {
            m_LeftShutter.Demolish();
            DestroyImmediate(m_LeftShutter.gameObject);
        }
        if (!m_Data.AreShuttersActive && m_RightShutter != null)
        {
            m_RightShutter.Demolish();
            DestroyImmediate(m_RightShutter.gameObject);
        }
    }
}
