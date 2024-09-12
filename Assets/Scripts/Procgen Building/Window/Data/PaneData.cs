using NUnit.Framework;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace OnlyInvalid.ProcGenBuilding.Window
{
    [System.Serializable]
    public class PaneData : Polygon3DAData
    {
        public PaneData() : base() { }
        public PaneData(Shape exteriorShape, List<Shape> interiorShapes, float depth, Vector3 position, Vector3 eulerAngle, Vector3 scale) : base(position, eulerAngle, scale, exteriorShape, interiorShapes, depth) { }
        public PaneData(PaneData data) : this(data.ExteriorShape, data.InteriorShapes, data.Depth, data.Position, data.EulerAngle, data.Scale) { }

    }
}
