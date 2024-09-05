using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using OnlyInvalid.ProcGenBuilding.Door;

namespace OnlyInvalid.ProcGenBuilding.Window
{
    [System.Serializable]
    public class WindowData : DirtyData, ICloneable
    {
        [SerializeField] int m_ID;
        [SerializeField] PolygonData m_Polygon;
        [SerializeField] WindowElement m_ActiveElements;

        #region Outer Frame
        [SerializeField] OuterFrameData m_OuterFrame;

        public OuterFrameData OuterFrame { get { return m_OuterFrame; } set { m_OuterFrame = value; } }
        public bool IsOuterFrameActive => m_ActiveElements.IsElementActive(WindowElement.OuterFrame);
        #endregion

        #region Inner Frame
        [SerializeField] InnerFrameData m_InnerFrame;
        [SerializeField] Material m_InnerFrameMaterial;

        public InnerFrameData InnerFrame { get { return m_InnerFrame; } set { m_InnerFrame = value; } }
        public bool IsInnerFrameActive => m_ActiveElements.IsElementActive(WindowElement.InnerFrame);
        #endregion

        #region Pane
        [SerializeField] PaneData m_Pane;

        public PaneData Pane { get { return m_Pane; } set { m_Pane = value; } }
        public bool IsPaneActive => m_ActiveElements.IsElementActive(WindowElement.Pane);
        #endregion

        #region Shutters
        [SerializeField] DoorData m_LeftShutter;
        [SerializeField] DoorData m_RightShutter;

        public DoorData LeftShutter { get { return m_LeftShutter; } set { m_LeftShutter = value; } }
        public DoorData RightShutter { get { return m_RightShutter; } set { m_RightShutter = value; } }

        public bool AreShuttersActive => m_ActiveElements.IsElementActive(WindowElement.Shutters);
        #endregion

        #region Accessors
        public int ID { get { return m_ID; } set { m_ID = value; } }
        public PolygonData Polygon { get { return m_Polygon; } set { m_Polygon = value; } }
        public WindowElement ActiveElements { get { return m_ActiveElements; } set { m_ActiveElements = value; } }
        #endregion

        public WindowData()
        {
            m_ActiveElements = WindowElement.Everything;
        }

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
        public WindowData(WindowElement activeElements, PolygonData opening, OuterFrameData outerFrame, InnerFrameData innerFrame, PaneData paneData, DoorData leftShutter, DoorData rightShutter)
        {
            m_ActiveElements = activeElements;
            m_Polygon = new PolygonData(opening);
            m_OuterFrame = new OuterFrameData(outerFrame);
            m_InnerFrame = new InnerFrameData(innerFrame);
            m_Pane = new PaneData(paneData);
            m_LeftShutter = new DoorData(leftShutter);
            m_RightShutter = new DoorData(rightShutter);
        }

        public override bool Equals(object obj)
        {
            if (obj is not WindowData other)
                return false;

            if (m_ActiveElements.Equals(other.ActiveElements) &&
               m_OuterFrame.Equals(other.OuterFrame) &&
               m_InnerFrame.Equals(other.InnerFrame) &&
               m_Pane.Equals(other.Pane) &&
               m_LeftShutter.Equals(other.LeftShutter) &&
               m_RightShutter.Equals(other.RightShutter))
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public object Clone()
        {
            WindowData clone = this.MemberwiseClone() as WindowData;
            clone.Polygon = new PolygonData(Polygon);
            clone.OuterFrame = this.OuterFrame.Clone() as OuterFrameData;
            clone.InnerFrame = this.InnerFrame.Clone() as InnerFrameData;
            clone.Pane = this.Pane.Clone() as PaneData;
            clone.LeftShutter = this.LeftShutter.Clone() as DoorData;
            clone.RightShutter = this.RightShutter.Clone() as DoorData;
            return clone;
        }
    }
}
