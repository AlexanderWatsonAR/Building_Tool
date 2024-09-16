using OnlyInvalid.ProcGenBuilding.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Polygon3DAData : DirtyData, ICloneable
{
    #region Members
    #region Transform
    // These transforms may not be needed. Could we just use the transform component?

    [SerializeField] protected Vector3 m_Position, m_EulerAngle, m_Scale;
    #endregion
    // Here we are assuming the exterior shape has a size of 1 and are polygons orientated on the xy plane (normal 0,0,1)
    [SerializeReference] protected Shape m_ExteriorShape;
    [SerializeReference] protected List<Shape> m_InteriorShapes;

    [SerializeField] float m_Depth;
    #endregion

    #region Accessors
    public Vector3 Position => m_Position;
    public Vector3 EulerAngle => m_EulerAngle;
    public Vector3 Scale => m_Scale;
    public Shape ExteriorShape => m_ExteriorShape;
    public List<Shape> InteriorShapes => m_InteriorShapes;
    public Vector3[] LocalControlPoints => m_ExteriorShape.ControlPoints();
    public Vector3[] ControlPoints
    {
        get
        {
            Vector3[] controlPoints = m_ExteriorShape.ControlPoints();
            Matrix4x4 trs = Matrix4x4.TRS(m_Position, Rotation, m_Scale);

            for (int i = 0; i < controlPoints.Length; i++)
            {
                controlPoints[i] = trs.MultiplyPoint3x4(controlPoints[i]);
            }

            return controlPoints;
        }
    }
    public IList<IList<Vector3>> LocalHoles => (IList<IList<Vector3>>)m_InteriorShapes.Select(hole => hole.ControlPoints().ToList());
    public IList<IList<Vector3>> Holes
    {
        get
        {
            IList<IList<Vector3>> holes = new List<IList<Vector3>>();

            if (m_InteriorShapes == null)
                return null;

            Matrix4x4 trs = Matrix4x4.TRS(m_Position, Rotation, m_Scale);

            foreach (var shape in m_InteriorShapes)
            {
                IList<Vector3> hole = shape.ControlPoints();

                for (int i = 0; i < hole.Count; i++)
                {
                    hole[i] = trs.MultiplyPoint3x4(hole[i]);
                }

                holes.Add(hole);
            }

            return holes;
        }
    }
    public Vector3 Normal
    {
        get
        {
            return Quaternion.Euler(m_EulerAngle) * Vector3.forward;
        }
    }
    public float Depth { get => m_Depth; set => m_Depth = value; }
    public Quaternion Rotation
    {
        get
        {
            return Quaternion.Euler(m_EulerAngle);
        }
    }
    #endregion

    #region Constructors
    public Polygon3DAData() : this(Vector3.zero, Vector3.zero, Vector3.one, new Square(), new List<Shape>(), 1)
    {
    }

    public Polygon3DAData(Vector3 position, Vector3 eulerAngle, Vector3 scale, Shape exteriorShape, List<Shape> interiorShapes, float depth)
    {
        m_Position = position;
        m_EulerAngle = eulerAngle;
        m_Scale = scale;
        m_ExteriorShape = exteriorShape;
        m_InteriorShapes = interiorShapes;
        m_Depth = depth;
    }
    public Polygon3DAData(Polygon3DAData data) : this(data.Position, data.EulerAngle, data.Scale, data.ExteriorShape, data.InteriorShapes, data.Depth)
    {

    }
    #endregion

    public void AddInteriorShape(Shape shape)
    {
        m_InteriorShapes.Add(shape);
    }
    public void ClearInterior()
    {
        m_InteriorShapes?.Clear();
    }
    public void SetExteriorShape(Shape shape)
    {
        m_ExteriorShape = shape;
    }
    public void SetTransform(Vector3 position, Vector3 eulerAngle, Vector3 scale)
    {
        m_Position = position;
        m_EulerAngle = eulerAngle;
        m_Scale = scale;
    }
    public object Clone()
    {
        Polygon3DAData clone = MemberwiseClone() as Polygon3DAData;
        clone.SetExteriorShape(m_ExteriorShape);

        foreach(Shape shape in m_InteriorShapes)
        {
            clone.AddInteriorShape(shape);
        }

        return clone;
    }
}