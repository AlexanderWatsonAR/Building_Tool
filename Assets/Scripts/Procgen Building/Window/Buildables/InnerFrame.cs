using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Window
{
    public class InnerFrame : GridFrame
    {
        [SerializeReference] InnerFrameData m_InnerFrameData;

        public InnerFrameData InnerFrameData => m_InnerFrameData;

        private void OnValidate()
        {
            m_OnDataChanged.Invoke(m_InnerFrameData);
        }
    }
}
