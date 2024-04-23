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
        public InnerFrameData(PolygonData polygon, PolygonData[] holes, Vector3 normal, Vector3 up, float depth, float scale, int columns, int rows) : base(polygon, holes, normal, up, depth, scale, columns, rows) { }
        public InnerFrameData(GridFrameData data) : base(data.Polygon, data.Holes, data.Normal, data.Up, data.Depth, data.Scale, data.Columns, data.Rows) { }

    }
}
