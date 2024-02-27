using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class WindowData : IData
{
    [SerializeField] int m_ID;
    [SerializeField] PolygonData m_Polygon;
    [SerializeField] WindowElement m_ActiveElements;

    #region Outer Frame
    [SerializeField] FrameData m_OuterFrame;
    [SerializeField] bool m_DoesOuterFrameNeedRebuild;

    public FrameData OuterFrame { get { return m_OuterFrame; } set { m_OuterFrame = value; } }
    public bool IsOuterFrameActive => m_ActiveElements.IsElementActive(WindowElement.OuterFrame);
    public bool DoesOuterFrameNeedRebuild { get { return m_DoesOuterFrameNeedRebuild; } set { m_DoesOuterFrameNeedRebuild = value; } }
    #endregion

    #region Inner Frame
    [SerializeField] GridFrameData m_InnerFrame;
    [SerializeField] Material m_InnerFrameMaterial;
    [SerializeField] bool m_DoesInnerFrameNeedRebuild;

    public GridFrameData InnerFrame { get { return m_InnerFrame; } set { m_InnerFrame = value; } }
    public bool IsInnerFrameActive => m_ActiveElements.IsElementActive(WindowElement.InnerFrame);
    public bool DoesInnerFrameNeedRebuild { get { return m_DoesInnerFrameNeedRebuild; } set { m_DoesInnerFrameNeedRebuild = value; } }
    #endregion

    #region Pane
    [SerializeField] Polygon3DData m_Pane;
    [SerializeField] bool m_DoesPaneNeedRebuild;

    public Polygon3DData Pane {get { return m_Pane; } set { m_Pane = value; } }
    public bool IsPaneActive => m_ActiveElements.IsElementActive(WindowElement.Pane);
    public bool DoesPaneNeedRebuild { get { return m_DoesPaneNeedRebuild; } set { m_DoesPaneNeedRebuild = value; } }
    #endregion

    #region Shutters
    [SerializeField] DoorData m_LeftShutter;
    [SerializeField] DoorData m_RightShutter;

    public DoorData LeftShutter { get { return m_LeftShutter; } set { m_LeftShutter = value; } }
    public DoorData RightShutter { get { return m_RightShutter; } set { m_RightShutter = value; } }

    [SerializeField] bool m_DoShuttersNeedRebuild;

    public bool AreShuttersActive => m_ActiveElements.IsElementActive(WindowElement.Shutters);
    public bool DoShuttersNeedRebuild { get { return m_DoShuttersNeedRebuild; } set { m_DoShuttersNeedRebuild = value; } }
    #endregion

    #region Accessors
    public int ID { get { return m_ID; } set { m_ID = value; } }
    public PolygonData Polygon { get { return m_Polygon; } set { m_Polygon = value; } }
    public WindowElement ActiveElements { get { return m_ActiveElements; } set { m_ActiveElements = value; } }
    #endregion

    public WindowData() { }

    public WindowData(WindowData data) : this
    (
        data.ActiveElements,
        data.Polygon,
        data.OuterFrame,
        data.InnerFrame,
        data.Pane,
        data.LeftShutter,
        data.RightShutter
    )
    {
    }
    public WindowData(WindowElement activeElements, PolygonData opening, FrameData outerFrame, GridFrameData innerFrame, Polygon3DData paneData, DoorData leftShutter, DoorData rightShutter)
    {
        m_ActiveElements = activeElements;
        m_Polygon = opening;
        m_OuterFrame = outerFrame;
        m_InnerFrame = innerFrame;
        m_Pane = paneData;
        m_LeftShutter = leftShutter;
        m_RightShutter = rightShutter;
    }
}
