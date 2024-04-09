using UnityEngine;

[System.Serializable]
public class Gable : RoofShapeData
{
    [SerializeField, Range(0, 1)] float m_Scale;
    [SerializeField] bool m_IsOpen;

    public float Scale { get { return m_Scale; } set { m_Scale = value; } }
    public bool IsOpen { get { return m_IsOpen; } set { m_IsOpen = value; } }

    public Gable() : this (1, true)
    {

    }
    public Gable(float scale, bool isOpen) : base ()
    {
        m_Scale = scale;
        m_IsOpen = isOpen;
    }
    public Gable(RoofTileData[] tiles, float height, float scale, bool isOpen) : base (tiles, height)
    {
        m_Scale = scale;
        m_IsOpen = isOpen;
    }
    public Gable(Gable gableData) : base(gableData)
    {

    }

}
