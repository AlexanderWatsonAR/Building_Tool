using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using UnityEngine.Events;

namespace OnlyInvalid.ProcGenBuilding.Window
{
    public class OuterFrame : Frame
    {
        public OuterFrameData OuterFrameData => m_Data as OuterFrameData;

        public override Buildable Initialize(DirtyData data)
        {
            return base.Initialize(data);
        }

        private void OnValidate()
        {
            m_OnDataChanged.Invoke(m_Data);
        }
    }
}
