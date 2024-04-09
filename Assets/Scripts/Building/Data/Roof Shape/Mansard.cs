using UnityEngine;

[System.Serializable]
public class Mansard : RoofShapeData
{
    [SerializeField, Range(0, 1)] float m_Scale;

    public float Scale { get { return m_Scale; } set { m_Scale = value; } }

    public Mansard() : base()
    {

    }
    public Mansard(float scale) : base()
    {
        m_Scale = scale;
    }
    public Mansard(RoofTileData[] tiles, float height, float scale) : base(tiles, height)
    {
        m_Scale = scale;
    }
    public Mansard(Mansard mansardData) : base(mansardData)
    {

    }

}
