using System.Collections.Generic;
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
        public OuterFrameData(Shape exteriorShape, List<Shape> interiorShapes, float frameScale, float depth, Vector3 position, Vector3 eulerAngle, Vector3 scale) : base(exteriorShape, interiorShapes, frameScale, depth, position, eulerAngle, scale) {}
        public OuterFrameData(OuterFrameData data) : base(data.ExteriorShape, data.InteriorShapes, data.FrameScale, data.Depth, data.Position, data.EulerAngle, data.Scale) { }

    }
}
