using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OnlyInvalid.ProcGenBuilding.Window;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using OnlyInvalid.ProcGenBuilding.Door;

namespace OnlyInvalid.ProcGenBuilding.Wall
{
    public class WallSectionDataSerializedProperties : SerializedPropertyGroup
    {
        #region Member Variables
        readonly WindowOpeningDataSerializedProperties m_WindowOpening;
        readonly ArchwayDataSerializedProperties m_Archway;
        readonly DoorwayDataSerializedProperties m_Doorway;
        readonly ExtensionDataSerializedProperties m_Extension;

        readonly WindowDataSerializedProperties m_Window;
        readonly DoorDataSerializedProperties m_Door;
        readonly DoorDataSerializedProperties m_ArchDoor;
        readonly FrameDataSerializedProperties m_Frame;
        #endregion

        #region Constants
        const string k_WallElement = "m_WallElement";
        const string k_Window = "m_Window";
        const string k_Door = "m_Door";
        const string k_ArchDoor = "m_ArchDoor";
        const string k_WindowOpening = "m_WindowOpening";
        const string k_Doorway = "m_Doorway";
        const string k_Archway = "m_Archway";
        const string k_Extension = "m_Extension";
        const string k_DoorFrame = "m_DoorFrame";
        #endregion

        #region Accessors
        public SerializedProperty WallElement => m_Data.FindPropertyRelative(k_WallElement);
        public DoorDataSerializedProperties Door => m_Door;
        public DoorDataSerializedProperties ArchDoor => m_ArchDoor;
        public WindowDataSerializedProperties Window => m_Window;
        public WindowOpeningDataSerializedProperties WindowOpening => m_WindowOpening;
        public DoorwayDataSerializedProperties Doorway => m_Doorway;
        public ArchwayDataSerializedProperties Archway => m_Archway;
        public ExtensionDataSerializedProperties Extension => m_Extension;
        public FrameDataSerializedProperties Frame => m_Frame;
        #endregion

        public WallSectionDataSerializedProperties(SerializedProperty wallSectionData) : base(wallSectionData)
        {
            m_Window = new WindowDataSerializedProperties(m_Data.FindPropertyRelative(k_Window));
            m_Door = new DoorDataSerializedProperties(m_Data.FindPropertyRelative(k_Door));
            m_ArchDoor = new DoorDataSerializedProperties(m_Data.FindPropertyRelative(k_ArchDoor));

            m_WindowOpening = new WindowOpeningDataSerializedProperties(m_Data.FindPropertyRelative(k_WindowOpening));
            m_Doorway = new DoorwayDataSerializedProperties(m_Data.FindPropertyRelative(k_Doorway));
            m_Archway = new ArchwayDataSerializedProperties(m_Data.FindPropertyRelative(k_Archway));
            m_Extension = new ExtensionDataSerializedProperties(m_Data.FindPropertyRelative(k_Extension));
            m_Frame = new FrameDataSerializedProperties(m_Data.FindPropertyRelative(k_DoorFrame));
        }

    }
}
