using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using UnityEngine.Events;

namespace OnlyInvalid.ProcGenBuilding.Window
{
    public class OuterFrame : Frame
    {
        [SerializeReference] OuterFrameData m_OuterFrameData;

        public OuterFrameData OuterFrameData => m_OuterFrameData;

        public override Buildable Initialize(DirtyData data)
        {
            base.Initialize(data);
            m_OuterFrameData = data as OuterFrameData;
            return this;
        }
    }
}
