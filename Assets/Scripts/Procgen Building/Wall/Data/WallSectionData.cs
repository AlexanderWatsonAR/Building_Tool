using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using OnlyInvalid.ProcGenBuilding.Window;
using OnlyInvalid.ProcGenBuilding.Door;

namespace OnlyInvalid.ProcGenBuilding.Wall
{
    [System.Serializable]
    public class WallSectionData : Polygon3DData, ICloneable
    {
        #region Member Variables
        [SerializeField] int m_ID;
        [SerializeField] WallElement m_WallElement;

        [SerializeField] WindowData m_Window;
        [SerializeField] DoorData m_Door;
        [SerializeField] DoorData m_ArchDoor;
        [SerializeField] FrameData m_DoorFrame;

        //[SerializeField] WindowOpeningData m_WindowOpening;
        //[SerializeField] DoorwayData m_Doorway;
        //[SerializeField] ArchwayData m_Archway;
        //[SerializeField] ExtensionData m_Extension;
        #endregion

        #region Accessors
        public WallElement WallElement { get { return m_WallElement; } set { m_WallElement = value; } }
        public WindowData Window { get { return m_Window; } set { m_Window = value; } }
        public DoorData Door { get { return m_Door; } set { m_Door = value; } }
        public DoorData ArchDoor { get { return m_ArchDoor; } set { m_ArchDoor = value; } }
        public FrameData DoorFrame { get { return m_DoorFrame; } set { m_DoorFrame = value; } }
        public int ID { get { return m_ID; } set { m_ID = value; } }
        //public WindowOpeningData WindowOpening { get { return m_WindowOpening; } set { m_WindowOpening = value; } }
        //public DoorwayData Doorway { get { return m_Doorway; } set { m_Doorway = value; } }
        //public ArchwayData Archway { get { return m_Archway; } set { m_Archway = value; } }
        //public ExtensionData Extension { get { return m_Extension; } set { m_Extension = value; } }
        #endregion

        public WallSectionData()
        {
            m_Window = new WindowData();
            m_Door = new DoorData();
            m_ArchDoor = new DoorData();
            m_DoorFrame = new FrameData();
            //m_WindowOpening = new WindowOpeningData();
            //m_Doorway = new DoorwayData();
            //m_Archway = new ArchwayData();
            //m_Extension = new ExtensionData();
        }

        public WallSectionData(WallElement wallElement, PolygonData polygon, PolygonData[] holes, int id, float depth, Vector3 normal, Vector3 up,
            //WindowOpeningData windowOpeningData, DoorwayData doorwayData, ArchwayData archwayData, ExtensionData extensionData,
            WindowData windowData, DoorData doorData, DoorData archDoorData, FrameData doorFrameData) : base(polygon, holes, normal, up, depth)
        {
            m_WallElement = wallElement;
            m_ID = id;
            //m_WindowOpening = windowOpeningData;
            //m_Doorway = doorwayData;
            //m_Archway = archwayData;
            //m_Extension = extensionData;
            m_Window = windowData;
            m_Door = doorData;
            m_ArchDoor = archDoorData;
            m_DoorFrame = doorFrameData;
        }

        public WallSectionData(WallSectionData data) : this
        (
            data.WallElement,
            data.Polygon,
            data.Holes,
            data.ID,
            data.Depth,
            data.Normal,
            data.Up,
            //data.WindowOpening,
            //data.Doorway,
            //data.Archway,
            //data.Extension,
            data.Window,
            data.Door,
            data.ArchDoor,
            data.DoorFrame
        )
        {
        }

        public new object Clone()
        {
            WallSectionData clone = base.Clone() as WallSectionData;
            clone.ID = this.ID;
            clone.WallElement = this.WallElement;
            clone.Window = this.Window.Clone() as WindowData;
            clone.Door = this.Door.Clone() as DoorData;
            clone.ArchDoor = this.ArchDoor.Clone() as DoorData;
            clone.DoorFrame = this.DoorFrame.Clone() as FrameData;
            //clone.WindowOpening = this.WindowOpening.Clone() as WindowOpeningData;
            //clone.Doorway = this.Doorway.Clone() as DoorwayData;
            //clone.Archway = this.Archway.Clone() as ArchwayData;
            //clone.Extension = this.Extension.Clone() as ExtensionData;
            return clone;
        }
    }
}
