using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    public abstract class BaseFrame : Polygon3D
    {
        protected abstract void CalculateInside();
    }
}