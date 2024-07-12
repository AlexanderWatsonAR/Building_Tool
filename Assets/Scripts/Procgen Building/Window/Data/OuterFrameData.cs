using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    [System.Serializable]
    public class OuterFrameData : FrameData
    {
        [SerializeReference] Polygon3D m_InnerPolygon3D;

        public Polygon3D InnerPolygon3D
        {
            get => m_InnerPolygon3D;
            set 
            {
                m_InnerPolygon3D = value;
            }
        }

        public OuterFrameData() : base() {}
        public OuterFrameData(float scale) : base(scale) {}
        public OuterFrameData(PolygonData polygon, PolygonData[] holes, Vector3 normal, Vector3 up, float depth, float scale) : base(polygon, holes, normal, up, depth, scale) {}
        public OuterFrameData(OuterFrameData data) : base(data.Polygon, data.Holes, data.Normal, data.Up, data.Depth, data.Scale) { }

    }
}
