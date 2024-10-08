using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Polygon2DData : DirtyData
{
    #region Members
    [SerializeField] protected Vector3 m_Position, m_EulerAngle, m_Scale;
    [SerializeReference] protected Shape m_ExteriorShape;
    [SerializeReference] protected List<Shape> m_InteriorShapes;
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
    public virtual Vector3 Normal()
    {
        return Quaternion.Euler(m_EulerAngle) * Vector3.forward;
    }
    public Quaternion Rotation
    {
        get
        {
            return Quaternion.Euler(m_EulerAngle);
        }
    }
    #endregion

    #region Constructors
    public Polygon2DData() : this(Vector3.zero, Vector3.zero, Vector3.one, new Square(), new List<Shape>())
    {
    }
    public Polygon2DData(Vector3 position, Vector3 eulerAngle, Vector3 scale, Shape exteriorShape, List<Shape> interiorShapes)
    {
        m_Position = position;
        m_EulerAngle = eulerAngle;
        m_Scale = scale;
        m_ExteriorShape = exteriorShape;
        m_InteriorShapes = interiorShapes;
    }
    public Polygon2DData(Polygon3DAData data) : this(data.Position, data.EulerAngle, data.Scale, data.ExteriorShape, data.InteriorShapes)
    {

    }
    #endregion

    #region Other Functions
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
    #endregion

}
