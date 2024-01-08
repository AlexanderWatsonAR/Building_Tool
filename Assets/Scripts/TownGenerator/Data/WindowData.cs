using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class WindowData : IData
{
    [SerializeField] private int m_ID;
    [SerializeField] private Vector3[] m_ControlPoints;
    [SerializeField, HideInInspector] private Vector3 m_Forward;
    [SerializeField] private WindowElement m_ActiveElements;
    [SerializeField] private Vector3 m_Position;
    [SerializeField] private float m_Height, m_Width;

    #region Outer Frame
    [SerializeField, Range(0, 0.999f)] private float m_OuterFrameScale;
    [SerializeField, Range(0, 0.999f)] private float m_OuterFrameDepth;
    [SerializeField] private bool m_IsOuterFrameActive;
    [SerializeField] private Material m_OuterFrameMaterial;
    [SerializeField] private bool m_DoesOuterFrameNeedRebuild;

    public float OuterFrameScale { get { return m_OuterFrameScale; } set { m_OuterFrameScale = value; } }
    public float OuterFrameDepth { get { return m_OuterFrameDepth; } set { m_OuterFrameDepth = value; } }
    public bool IsOuterFrameActive => m_ActiveElements.IsElementActive(WindowElement.OuterFrame);
    public Material OuterFrameMaterial { get { return m_OuterFrameMaterial; } set { m_OuterFrameMaterial = value; } }
    public bool DoesOuterFrameNeedRebuild { get { return m_DoesOuterFrameNeedRebuild; } set { m_DoesOuterFrameNeedRebuild = value; } }
    #endregion

    #region Inner Frame
    [SerializeField, Range(1, 5)] private int m_InnerFrameColumns, m_InnerFrameRows;
    [SerializeField, Range(0, 0.999f)] private float m_InnerFrameScale;
    [SerializeField, Range(0, 0.999f)] private float m_InnerFrameDepth;
    [SerializeField] private Material m_InnerFrameMaterial;
    [SerializeField] private bool m_IsInnerFrameActive;
    [SerializeField] private Vector3[][] m_InnerFrameHolePoints;
    [SerializeField] private bool m_DoesInnerFrameNeedRebuild;

    public int InnerFrameColumns { get { return m_InnerFrameColumns; } set { m_InnerFrameColumns = value; } }
    public int InnerFrameRows { get { return m_InnerFrameRows; } set { m_InnerFrameRows = value; } }
    public float InnerFrameScale { get { return m_InnerFrameScale; } set { m_InnerFrameScale = value; } }
    public float InnerFrameDepth { get { return m_InnerFrameDepth; } set { m_InnerFrameDepth = value; } }
    public Material InnerFrameMaterial { get { return m_InnerFrameMaterial; } set { m_InnerFrameMaterial = value; } }
    public bool IsInnerFrameActive => m_ActiveElements.IsElementActive(WindowElement.InnerFrame);
    public IList<IList<Vector3>> InnerFrameHolePoints { get{ return m_InnerFrameHolePoints; } set { m_InnerFrameHolePoints = value.Select(list => list.ToArray()).ToArray(); } }
    
    public bool DoesInnerFrameNeedRebuild { get { return m_DoesInnerFrameNeedRebuild; } set { m_DoesInnerFrameNeedRebuild = value; } }
    #endregion

    #region Pane
    [SerializeField, Range(0, 0.999f)] private float m_PaneDepth;
    [SerializeField] private bool m_IsPaneActive;
    [SerializeField] private Material m_PaneMaterial;
    [SerializeField] private bool m_DoesPaneNeedRebuild;

    public bool IsPaneActive => m_ActiveElements.IsElementActive(WindowElement.Pane);
    public float PaneDepth { get { return m_PaneDepth; } set { m_PaneDepth = value; } }
    public Material PaneMaterial { get { return m_PaneMaterial; } set { m_PaneMaterial = value; } }
    public bool DoesPaneNeedRebuild { get { return m_DoesPaneNeedRebuild; } set { m_DoesPaneNeedRebuild = value; } }
    #endregion

    #region Shutters
    [SerializeField, Range(0, 0.999f)] private float m_ShuttersDepth;
    [SerializeField, Range(0, 180)] private float m_ShuttersAngle;
    [SerializeField] private Material m_ShuttersMaterial;
    [SerializeField] private bool m_AreShuttersActive;
    [SerializeField] private bool m_DoShuttersNeedRebuild;

    public float ShuttersDepth { get { return m_ShuttersDepth; } set { m_ShuttersDepth = value; } }
    public float ShuttersAngle { get { return m_ShuttersAngle; } set { m_ShuttersAngle = value; } }
    public bool AreShuttersActive => m_ActiveElements.IsElementActive(WindowElement.Shutters);
    public Material ShuttersMaterial { get { return m_ShuttersMaterial; } set { m_ShuttersMaterial = value; } }
    public bool DoShuttersNeedRebuild { get { return m_DoShuttersNeedRebuild; } set { m_DoShuttersNeedRebuild = value; } }
    #endregion

    #region Accessors
    public int ID { get { return m_ID; } set { m_ID = value; } }
    public Vector3[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
    public Vector3 Forward { get { return m_Forward; } set { m_Forward = value; } }
    public WindowElement ActiveElements { get { return m_ActiveElements; } set { m_ActiveElements = value; } }
    public Vector3 Position { get { return m_Position; } set { m_Position = value; } }
    public float Height { get { return m_Height; } set { m_Height = value; } }
    public float Width { get { return m_Width; } set { m_Width = value; } }
    #endregion

    public WindowData() : this (WindowElement.Everything, Vector3.forward, null, 2, 2, 0.95f, 0.95f, 0.4f, 0.2f, 0.1f, 0.2f, 90f, null, null, null, null)
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
    public WindowData(WindowElement activeElements, Vector3 forward, Vector3[] controlPoints, int columns, int rows, float outerFrameScale, float innerFrameScale, float outerFrameDepth,
        float innerFrameDepth, float paneDepth, float shuttersDepth, float shuttersAngle,
        Material outerFrameMat, Material innerFrameMat, Material paneMat, Material shuttersMat)
    {
        m_ActiveElements = activeElements;
        m_Forward = forward;
        m_ControlPoints = controlPoints;
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

        m_DoesInnerFrameNeedRebuild = true;
        m_DoesOuterFrameNeedRebuild = true;
        m_DoesPaneNeedRebuild = true;
        m_DoShuttersNeedRebuild = true;
    }

    public void ClearInnerFrameHolePoints()
    {
        m_InnerFrameHolePoints = new Vector3[0][];
    }
}
