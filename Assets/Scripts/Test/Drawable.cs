using OnlyInvalid.ProcGenBuilding.Building;
using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.EditorTools;

public class Drawable : MonoBehaviour, IDrawable
{
    [SerializeField] PolygonPath m_Path;
    public PlanarPath Path => m_Path;

    Drawable()
    {
        Plane plane = new Plane(Vector3.up, 0);

        m_Path = new PolygonPath(plane);
    }
}

public interface IDrawable
{
    PlanarPath Path { get; }
}
