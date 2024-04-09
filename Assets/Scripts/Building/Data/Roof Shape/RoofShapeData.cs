using UnityEngine;

[System.Serializable]
public class RoofShapeData
{
    [SerializeField] RoofTileData[] m_RoofTiles;
    [SerializeField, Range(-10, 10)] float m_Height;

    public RoofTileData[] RoofTiles { get { return m_RoofTiles; } set { m_RoofTiles = value; } }
    public float Height { get { return m_Height; } set { m_Height = value; } }

    public RoofShapeData() : this (null, 1)
    {
    }

    public RoofShapeData(RoofTileData[] tiles, float height)
    {
        m_RoofTiles = tiles;
        m_Height = height;
    }

    public RoofShapeData(RoofShapeData data) : this (data.RoofTiles, data.Height)
    {

    }
}
