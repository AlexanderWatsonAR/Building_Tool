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

[System.Serializable]
public abstract class Shape
{
    public abstract Vector3[] ControlPoints();
}
