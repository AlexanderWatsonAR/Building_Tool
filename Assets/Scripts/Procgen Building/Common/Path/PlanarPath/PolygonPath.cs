using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [System.Serializable]
    public class PolygonPath : PlanarPath
    {
        public PolygonPath(Vector3 normal, float minimumPointDistance = 1) : base(new Plane(normal, 0), minimumPointDistance)
        {

        }
        public PolygonPath(Plane plane, float minimumPointDistance = 1) : base(plane, minimumPointDistance)
        {

        }
        public PolygonPath(List<ControlPoint> controlPoints, Plane plane, float minimumPointDistance = 1) : base(controlPoints, plane, minimumPointDistance)
        {

        }

    }
}