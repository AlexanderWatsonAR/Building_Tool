using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : Shape
{
    PolygonMaker.PivotPoint m_Pivot;

    #region Constructors
    public Square() : base() { }
    public Square(PolygonMaker.PivotPoint pivot) : base()
    {
        m_Pivot = pivot;
    }
    #endregion

    public override Vector3[] ControlPoints()
    {
        return PolygonMaker.Square(m_Pivot);
    }
}
