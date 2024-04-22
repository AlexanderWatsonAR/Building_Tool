using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Window
{
    public class InnerFrame : GridFrame
    {
        [SerializeReference] InnerFrameData m_InnerFrameData;

        public InnerFrameData InnerFrameData => m_InnerFrameData;

        public override Buildable Initialize(DirtyData data)
        {
            base.Initialize(data);
            m_InnerFrameData = data as InnerFrameData;
            return this;
        }
    }
}
