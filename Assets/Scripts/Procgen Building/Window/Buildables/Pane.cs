using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Window
{
    public class Pane : Polygon3D.Polygon3D
    {
        [SerializeReference] PaneData m_PaneData;

        public PaneData PaneData => m_PaneData;

        public override Buildable Initialize(DirtyData data)
        {
            m_PaneData = data as PaneData;
            return base.Initialize(data);
        }

    }
}
