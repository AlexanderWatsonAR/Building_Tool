using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    [System.Serializable]
    public class OuterFrameData : FrameData
    {
        public OuterFrameData() : base() {}
        public OuterFrameData(float scale) : base(scale) {}
        public OuterFrameData(PolygonData polygon, PolygonData[] holes, Vector3 normal, Vector3 up, float depth, float scale) : base(polygon, holes, normal, up, depth, scale) {}
        public OuterFrameData(OuterFrameData data) : base(data.Polygon, data.Holes, data.Normal, data.Up, data.Depth, data.Scale) { }

    }
}
