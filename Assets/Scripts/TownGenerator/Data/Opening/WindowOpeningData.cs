using UnityEngine;

[System.Serializable]
public class WindowOpeningData : OpeningData
{
    [SerializeField, Range(3, 16)] int m_Sides = 3;
    [SerializeField, Range(-180, 180)] float m_Angle;
    [SerializeField] bool m_IsSmooth;
    [SerializeField] WindowData[] m_Windows;

    public int Sides { get { return m_Sides; } set { m_Sides = value; } }
    public float Angle { get { return m_Angle; } set { m_Angle = value; } }
    public bool Smooth { get { return m_IsSmooth; } set { m_IsSmooth = value; } }
    public WindowData[] Windows { get { return m_Windows; } set { m_Windows = value; } }

    public WindowOpeningData() : this(4, 0, false)
    {
    }

    public WindowOpeningData(int sides, float angle, bool isSmooth) : base()
    {
        m_Sides = sides;
        m_Angle = angle;
        m_IsSmooth = isSmooth;
    }

    public WindowOpeningData(float height, float width, int cols, int rows, int sides, float angle, bool isSmooth) : base(height, width, cols, rows)
    {
        m_Sides = sides;
        m_Angle = angle;
        m_IsSmooth = isSmooth;
    }

    public WindowOpeningData(WindowOpeningData data) : this(data.Height, data.Width, data.Columns, data.Rows, data.Sides, data.Angle, data.m_IsSmooth)
    {

    }

}
