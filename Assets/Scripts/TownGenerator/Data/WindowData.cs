using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class WindowData : IData
{
    [SerializeField] private int m_ID;
    [SerializeField] private Vector3[] m_ControlPoints;
    [SerializeField, HideInInspector] private Vector3 m_Normal;
    [SerializeField] private WindowElement m_ActiveElements;

    #region Outer Frame
    [SerializeField] FrameData m_OuterFrame;
    [SerializeField] private bool m_DoesOuterFrameNeedRebuild;

    public FrameData OuterFrame { get { return m_OuterFrame; } set { m_OuterFrame = value; } }
    public bool IsOuterFrameActive => m_ActiveElements.IsElementActive(WindowElement.OuterFrame);
    public bool DoesOuterFrameNeedRebuild { get { return m_DoesOuterFrameNeedRebuild; } set { m_DoesOuterFrameNeedRebuild = value; } }
    #endregion

    #region Inner Frame
    [SerializeField] GridFrameData m_InnerFrame;
    [SerializeField] private Material m_InnerFrameMaterial;
    [SerializeField] private bool m_DoesInnerFrameNeedRebuild;

    public GridFrameData InnerFrame { get { return m_InnerFrame; } set { m_InnerFrame = value; } }
    public bool IsInnerFrameActive => m_ActiveElements.IsElementActive(WindowElement.InnerFrame);
    public bool DoesInnerFrameNeedRebuild { get { return m_DoesInnerFrameNeedRebuild; } set { m_DoesInnerFrameNeedRebuild = value; } }
    #endregion

    #region Pane
    [SerializeField] Polygon3DData m_Pane;
    [SerializeField] private bool m_DoesPaneNeedRebuild;

    public Polygon3DData Pane {get { return m_Pane; } set { m_Pane = value; } }
    public bool IsPaneActive => m_ActiveElements.IsElementActive(WindowElement.Pane);
    public bool DoesPaneNeedRebuild { get { return m_DoesPaneNeedRebuild; } set { m_DoesPaneNeedRebuild = value; } }
    #endregion

    #region Shutters
    [SerializeField] DoorData m_LeftShutter;
    [SerializeField] DoorData m_RightShutter;

    public DoorData LeftShutter { get { return m_LeftShutter; } set { m_LeftShutter = value; } }
    public DoorData RightShutter { get { return m_RightShutter; } set { m_RightShutter = value; } }

    [SerializeField] private bool m_DoShuttersNeedRebuild;

    public bool AreShuttersActive => m_ActiveElements.IsElementActive(WindowElement.Shutters);
    public bool DoShuttersNeedRebuild { get { return m_DoShuttersNeedRebuild; } set { m_DoShuttersNeedRebuild = value; } }
    #endregion

    #region Accessors
    public int ID { get { return m_ID; } set { m_ID = value; } }
    public Vector3[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
    public Vector3 Normal
    {
        get 
        { 
            return m_Normal;
        }
        set
        {
            m_Normal = value;
            m_OuterFrame.Normal = value;
            m_InnerFrame.Normal = value;
            m_Pane.Normal = value;
        }
    }
    public WindowElement ActiveElements { get { return m_ActiveElements; } set { m_ActiveElements = value; } }
    #endregion

    public WindowData() : this (WindowElement.Everything, null, Vector3.forward, null, null, null, null, null)
    {

    }
    public WindowData(WindowData data) : this
    (
        data.ActiveElements,
        data.ControlPoints,
        data.Normal,
        data.OuterFrame,
        data.InnerFrame,
        data.Pane,
        data.LeftShutter,
        data.RightShutter
    )
    {
    }
    public WindowData(WindowElement activeElements, Vector3[] controlPoints, Vector3 normal, FrameData outerFrame, GridFrameData innerFrame, Polygon3DData paneData, DoorData leftShutter, DoorData rightShutter)
    {
        m_ActiveElements = activeElements;
        m_Normal = normal;
        m_ControlPoints = controlPoints;
        m_OuterFrame = outerFrame;
        m_InnerFrame = innerFrame;
        m_Pane = paneData;
        m_LeftShutter = leftShutter;
        m_RightShutter = rightShutter;
    }
}
