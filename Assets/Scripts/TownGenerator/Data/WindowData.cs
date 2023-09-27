using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class WindowData
{
    [SerializeField] private Vector3[] m_ControlPoints;
    [SerializeField] private WindowElement m_ActiveElements;
    [SerializeField, Range(1, 5)] private int m_Columns, m_Rows;
    [SerializeField, Range(0, 0.999f)] private float m_OuterFrameScale, m_InnerFrameScale;
    [SerializeField, Range(0, 0.999f)] private float m_OuterFrameDepth, m_InnerFrameDepth, m_PaneDepth;
    [SerializeField] private Material m_OuterFrameMaterial, m_InnerFrameMaterial, m_PaneMaterial, m_ShuttersMaterial;
    [SerializeField, Range(0, 0.999f)] private float m_ShuttersDepth;
    [SerializeField, Range(0, 180)] private float m_ShuttersAngle;


    public Vector3[] ControlPoints => m_ControlPoints;
    public int Columns => m_Columns;
    public int Rows => m_Rows;
    public float OuterFrameScale => m_OuterFrameScale;
    public float InnerFrameScale => m_InnerFrameScale;
    public float OuterFrameDepth => m_OuterFrameDepth;
    public float InnerFrameDepth => m_InnerFrameDepth;
    public float PaneDepth => m_PaneDepth;
    public float ShuttersDepth => m_ShuttersDepth;
    public float ShuttersAngle => m_ShuttersAngle;
    public Material OuterFrameMaterial => m_OuterFrameMaterial;
    public Material InnerFrameMaterial => m_InnerFrameMaterial;
    public Material PaneMaterial => m_PaneMaterial;
    public Material ShuttersMaterial => m_ShuttersMaterial;
    public WindowElement ActiveElements => m_ActiveElements;

    public bool IsOuterFrameActive => IsElementActive(WindowElement.OuterFrame);
    public bool IsInnerFrameActive => IsElementActive(WindowElement.InnerFrame);
    public bool IsPaneActive => IsElementActive(WindowElement.Pane);
    public bool AreShuttersActive => IsElementActive(WindowElement.Shutters);

    public WindowData() : this (WindowElement.Everything, new Vector3[0], 2, 2, 0.95f, 0.95f, 0.4f, 0.2f, 0.1f, 0.2f, 90f, null, null, null, null)
    {

    }
    public WindowData(WindowData data) : this
    (
        data.ActiveElements,
        data.ControlPoints,
        data.Columns,
        data.Rows,
        data.OuterFrameScale,
        data.InnerFrameScale,
        data.OuterFrameDepth,
        data.InnerFrameDepth,
        data.PaneDepth,
        data.ShuttersDepth,
        data.ShuttersAngle,
        data.OuterFrameMaterial,
        data.InnerFrameMaterial,
        data.PaneMaterial,
        data.ShuttersMaterial)
    {

    }
    public WindowData(WindowElement activeElements, IEnumerable<Vector3> controlPoints, int columns, int rows, float outerFrameScale, float innerFrameScale, float outerFrameDepth, float innerFrameDepth, float paneDepth, float shuttersDepth, float shuttersAngle, Material outerFrameMat, Material innerFrameMat, Material paneMat, Material shuttersMat)
    {
        m_ActiveElements = activeElements;
        m_ControlPoints = controlPoints == null ? new Vector3[0] : controlPoints.ToArray();
        m_Columns = columns;
        m_Rows = rows;
        m_OuterFrameScale = outerFrameScale;
        m_InnerFrameScale = innerFrameScale;
        m_OuterFrameDepth = outerFrameDepth;
        m_InnerFrameDepth = innerFrameDepth;
        m_PaneDepth = paneDepth;
        m_ShuttersDepth = shuttersDepth;
        m_ShuttersAngle = shuttersAngle;
        m_OuterFrameMaterial = outerFrameMat;
        m_InnerFrameMaterial = innerFrameMat;
        m_PaneMaterial = paneMat;
        m_ShuttersMaterial = shuttersMat;
    }

    public void SetControlPoints(IEnumerable<Vector3> controlPoints)
    {
        m_ControlPoints = controlPoints.ToArray();
    }

    public void SetMaterials(Material outerFrame, Material innerFrame, Material pane, Material shutters)
    {
        m_OuterFrameMaterial = outerFrame;
        m_InnerFrameMaterial = innerFrame;
        m_PaneMaterial = pane;
        m_ShuttersMaterial = shutters;
    }

    private bool IsElementActive(WindowElement windowElement)
    {
        return m_ActiveElements == WindowElement.Nothing ? false : (m_ActiveElements & windowElement) != 0;
    }

}
