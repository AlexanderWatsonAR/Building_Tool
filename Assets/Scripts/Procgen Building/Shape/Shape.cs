using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// The shape class contains data displayed in the inspector.
/// The user will adjust this data to configure the shape of the opening
/// </summary>

namespace OnlyInvalid.ProcGenBuilding.Common
{

    public abstract class Shape : ScriptableObject
    {
        public abstract Vector3[] ControlPoints();
    }
}
