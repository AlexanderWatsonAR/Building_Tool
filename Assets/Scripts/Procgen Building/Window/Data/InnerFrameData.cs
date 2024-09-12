using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    [System.Serializable]
    public class InnerFrameData : GridFrameData
    {
        public InnerFrameData() : base() { }
        public InnerFrameData(int columns, int rows) : base(columns, rows) { }
        public InnerFrameData(Shape exteriorShape, List<Shape> interiorShapes, int columns, int rows, float frameScale, float depth, Vector3 position, Vector3 eulerAngle, Vector3 scale) : base(exteriorShape, interiorShapes, columns, rows, frameScale, depth, position, eulerAngle, scale) { }
        public InnerFrameData(GridFrameData data) : base(data.ExteriorShape, data.InteriorShapes, data.Columns, data.Rows, data.FrameScale, data.Depth, data.Position, data.EulerAngle, data.Scale) { }

    }
}
