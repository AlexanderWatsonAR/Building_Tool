using OnlyInvalid.ProcGenBuilding.Polygon3D;
using System;
using System.Collections;
using UnityEngine;

public class Container : Polygon2DData
{
    #region Members
    [SerializeReference] protected Polygon2D m_Content;
    #endregion

    #region Accessors
    public Polygon2D Content { get => m_Content;}
    #endregion

    #region Constructors
    public Container(Shape shape, Vector3 position, Vector3 eulerAngle, Vector3 scale) : this(shape, null, position, eulerAngle, scale)
    {
    }
    public Container(Shape shape, Polygon2D content, Vector3 position, Vector3 eulerAngle, Vector3 scale) : base(position, eulerAngle, scale, shape, null)
    {
        m_Content = content;
    }
    public Container(Container data) : this(data.ExteriorShape, data.Content, data.Position, data.EulerAngle, data.Scale)
    {

    }
    #endregion

    public void SetContent(Polygon2D content)
    {
        m_Content = content;
        m_Content.Polygon2DData.SetShape(this.ExteriorShape);
        m_Content.transform.localPosition = this.Position;
        m_Content.transform.localEulerAngles = this.m_EulerAngle;
        m_Content.transform.localScale = new Vector3(this.m_Scale.x, this.Scale.y, 1);
       // m_Content.Polygon2DData.SetTransform(this.Position, this.EulerAngle, this.Scale);
    }
}