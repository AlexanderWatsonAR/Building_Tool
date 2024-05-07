using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [System.Serializable]
    public class LinePath : PlanarPath
    {
        public LinePath(Plane plane, float minimumPointDistance) : base(plane, minimumPointDistance)
        {
        }
        public LinePath(List<ControlPoint> controlPoints, Plane plane, float minimumPointDistance) : base(controlPoints, plane, minimumPointDistance)
        {

        }
        public override bool CanPointBeAdded(Vector3 point)
        {
            if (!base.CanPointBeAdded(point))
                return false;

            return true;
        }
    }
}
