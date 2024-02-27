using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WindowDataSerializedProperties : SerializedPropertyGroup
{
    #region Member Variables
    [SerializeField] readonly FrameDataSerializedProperties m_OuterFrame;
    [SerializeField] readonly GridFrameDataSerializedProperties m_InnerFrame;
    [SerializeField] readonly Polygon3DDataSerializedProperties m_Pane;
    [SerializeField] readonly DoorDataSerializedProperties m_LeftShutter;
    [SerializeField] readonly DoorDataSerializedProperties m_RightShutter;
    #endregion

    #region Constants
    const string k_ActiveElements = "m_ActiveElements";
    const string k_OuterFrame = "m_OuterFrame";
    const string k_InnerFrame = "m_InnerFrame";
    const string k_Pane = "m_Pane";
    const string k_LeftShutter = "m_LeftShutter";
    const string k_RightShutter = "m_RightShutter";
    #endregion

    #region Accessors
    public SerializedProperty ActiveElements => m_Data.FindPropertyRelative(k_ActiveElements);
    public FrameDataSerializedProperties OuterFrame => m_OuterFrame;
    public GridFrameDataSerializedProperties InnerFrame => m_InnerFrame;
    public Polygon3DDataSerializedProperties Pane => m_Pane;
    public DoorDataSerializedProperties LeftShutter => m_LeftShutter;
    public DoorDataSerializedProperties RightShutter => m_RightShutter;
    #endregion

    public WindowDataSerializedProperties(SerializedProperty windowData) : base(windowData)
    {
        m_OuterFrame = new FrameDataSerializedProperties(m_Data.FindPropertyRelative(k_OuterFrame));
        m_InnerFrame = new GridFrameDataSerializedProperties(m_Data.FindPropertyRelative(k_InnerFrame));
        m_Pane = new Polygon3DDataSerializedProperties(m_Data.FindPropertyRelative(k_Pane));
        m_LeftShutter = new DoorDataSerializedProperties(m_Data.FindPropertyRelative(k_LeftShutter));
        m_RightShutter = new DoorDataSerializedProperties(m_Data.FindPropertyRelative(k_RightShutter));

    }
}
