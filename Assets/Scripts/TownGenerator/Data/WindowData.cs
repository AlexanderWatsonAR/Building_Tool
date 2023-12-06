using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class WindowData : IData
{
    [SerializeField] private Vector3[] m_ControlPoints;
    [SerializeField, HideInInspector] private Vector3 m_Forward;
    [SerializeField] private WindowElement m_ActiveElements;
    [SerializeField, Range(1, 5)] private int m_InnerFrameColumns, m_InnerFrameRows;
    [SerializeField, Range(0, 0.999f)] private float m_OuterFrameScale, m_InnerFrameScale;
    [SerializeField, Range(0, 0.999f)] private float m_OuterFrameDepth, m_InnerFrameDepth, m_PaneDepth;
    [SerializeField] private Material m_OuterFrameMaterial, m_InnerFrameMaterial, m_PaneMaterial, m_ShuttersMaterial;
    [SerializeField, Range(0, 0.999f)] private float m_ShuttersDepth;
    [SerializeField, Range(0, 180)] private float m_ShuttersAngle;

    [SerializeField] private bool m_IsOuterFrameActive;
    [SerializeField] private bool m_IsInnerFrameActive;
    [SerializeField] private bool m_IsPaneActive;
    [SerializeField] private bool m_AreShuttersActive;

    public Vector3[] ControlPoints => m_ControlPoints;
    public Vector3 Forward { get { return m_Forward; } set { m_Forward = value; } }
    public int InnerFrameColumns => m_InnerFrameColumns;
    public int InnerFrameRows => m_InnerFrameRows;
    public float OuterFrameScale => m_OuterFrameScale;
    public float InnerFrameScale => m_InnerFrameScale;
    public float OuterFrameDepth { get { return m_OuterFrameDepth; } set { m_OuterFrameDepth = value; } }
    public float InnerFrameDepth { get { return m_InnerFrameDepth; } set { m_InnerFrameDepth = value; } }
    public float PaneDepth { get { return m_PaneDepth; } set { m_PaneDepth = value; } }
    public float ShuttersDepth { get { return m_ShuttersDepth; } set { m_ShuttersDepth = value; } }
    public float ShuttersAngle => m_ShuttersAngle;
    public Material OuterFrameMaterial { get { return m_OuterFrameMaterial; } set { m_OuterFrameMaterial = value; } }
    public Material InnerFrameMaterial { get { return m_InnerFrameMaterial; } set { m_InnerFrameMaterial = value; } }
    public Material PaneMaterial { get { return m_PaneMaterial; } set { m_PaneMaterial = value; } }
    public Material ShuttersMaterial { get { return m_ShuttersMaterial; } set { m_ShuttersMaterial = value; } }
    public WindowElement ActiveElements => m_ActiveElements;

    public bool IsOuterFrameActive => m_ActiveElements.IsElementActive(WindowElement.OuterFrame);
    public bool IsInnerFrameActive => m_ActiveElements.IsElementActive(WindowElement.InnerFrame);
    public bool IsPaneActive => m_ActiveElements.IsElementActive(WindowElement.Pane);
    public bool AreShuttersActive => m_ActiveElements.IsElementActive(WindowElement.Shutters);

    public WindowData() : this (WindowElement.Everything, Vector3.forward, new Vector3[0], 2, 2, 0.95f, 0.95f, 0.4f, 0.2f, 0.1f, 0.2f, 90f, null, null, null, null)
    {

    }
    public WindowData(WindowData data) : this
    (
        data.ActiveElements,
        data.Forward,
        data.ControlPoints,
        data.InnerFrameColumns,
        data.InnerFrameRows,
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
        data.ShuttersMaterial
    )
    {
    }
    public WindowData(WindowElement activeElements, Vector3 forward, IEnumerable<Vector3> controlPoints, int columns, int rows, float outerFrameScale, float innerFrameScale, float outerFrameDepth, float innerFrameDepth, float paneDepth, float shuttersDepth, float shuttersAngle, Material outerFrameMat, Material innerFrameMat, Material paneMat, Material shuttersMat)
    {
        m_ActiveElements = activeElements;
        m_Forward = forward;
        m_ControlPoints = controlPoints == null ? new Vector3[0] : controlPoints.ToArray();
        m_InnerFrameColumns = columns;
        m_InnerFrameRows = rows;
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
}
