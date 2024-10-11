using OnlyInvalid.ProcGenBuilding.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Polygon3DAData : Polygon2DData, ICloneable
{
    #region Members
    [SerializeField] protected float m_Depth;
    #endregion

    #region Accessors
    public float Depth { get => m_Depth; set => m_Depth = value; }
    #endregion

    #region Constructors
    public Polygon3DAData() : base(Vector3.zero, Vector3.zero, Vector3.one, new Square(), new List<Polygon2DData>())
    {
        m_Depth = 1;
    }
    public Polygon3DAData(Vector3 position, Vector3 eulerAngle, Vector3 scale, Shape exteriorShape, List<Polygon2DData> interiorShapes, float depth): base(position, eulerAngle, scale, exteriorShape, interiorShapes)
    {
        m_Depth = depth;
    }
    public Polygon3DAData(Polygon3DAData data) : this(data.Position, data.EulerAngle, data.Scale, data.ExteriorShape, data.InteriorShapes, data.Depth)
    {

    }
    #endregion

    public object Clone()
    {
        Polygon3DAData clone = MemberwiseClone() as Polygon3DAData;
        clone.SetShape(m_Shape);

        //foreach(Shape shape in m_InteriorShapes)
        //{
        //    clone.AddToInterior(shape);
        //}

        return clone;
    }
}