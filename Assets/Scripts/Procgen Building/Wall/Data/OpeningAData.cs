using OnlyInvalid.ProcGenBuilding.Polygon3D;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class OpeningAData : Container
{
    #region Members
    [SerializeField] string m_Name;
    [SerializeField] bool m_IsActive;
    #endregion

    #region Accessors
    public string Name { get => m_Name; set => m_Name = value; }
    public bool IsActive { get => m_IsActive; set => m_IsActive = value; }
    #endregion

    #region Constructors
    public OpeningAData(Shape shape, Vector3 position, Vector3 eulerAngle, Vector3 scale) : base(shape, position, eulerAngle, scale)
    {

    }
    public OpeningAData(Shape shape, Polygon2D content, Vector3 position, Vector3 eulerAngle, Vector3 scale) : base(shape, content, position, eulerAngle, scale)
    {

    }
    public OpeningAData(Shape shape, Polygon2D content, Vector3 position, Vector3 eulerAngle, Vector3 scale, string name, bool isActive) : base(shape, content, position, eulerAngle, scale)
    {
        m_Name = name;
        m_IsActive = isActive;
        m_Content.Polygon2DData.SetShape(m_Shape);
    }
    public OpeningAData(OpeningAData data) : this(data.ExteriorShape, data.Content, data.Position, data.EulerAngle, data.Scale, data.Name, data.IsActive)
    {

    }
    #endregion
}
