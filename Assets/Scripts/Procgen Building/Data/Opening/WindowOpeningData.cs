using System;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Window;

[System.Serializable]
public class WindowOpeningData : OpeningData, ICloneable
{
    [SerializeField, Range(3, 16)] int m_Sides = 3;
    [SerializeField, Range(-180, 180)] float m_Angle;
    [SerializeField] bool m_IsSmooth;
    [SerializeField] WindowData[] m_Windows;

    public int Sides { get { return m_Sides; } set { m_Sides = value; } }
    public float Angle { get { return m_Angle; } set { m_Angle = value; } }
    public bool Smooth { get { return m_IsSmooth; } set { m_IsSmooth = value; } }
    public WindowData[] Windows { get { return m_Windows; } set { m_Windows = value; } }

    public WindowOpeningData() : this(4, 0, false, null)
    {
    }

    public WindowOpeningData(int sides, float angle, bool isSmooth, WindowData[] windows) : base()
    {
        m_Sides = sides;
        m_Angle = angle;
        m_IsSmooth = isSmooth;
        m_Windows = windows;
    }

    public WindowOpeningData(float height, float width, int cols, int rows, int sides, float angle, bool isSmooth, WindowData[] windows) : base(height, width, cols, rows)
    {
        m_Sides = sides;
        m_Angle = angle;
        m_IsSmooth = isSmooth;
        m_Windows = windows;
    }

    public WindowOpeningData(WindowOpeningData data) : this(data.Height, data.Width, data.Columns, data.Rows, data.Sides, data.Angle, data.Smooth, data.Windows)
    {

    }

    public new object Clone()
    {
        WindowOpeningData clone = base.Clone() as WindowOpeningData;
        clone.Sides = this.Sides;
        clone.Angle = this.Angle;
        clone.Smooth = this.Smooth;

        if (this.Windows == null || this.Windows.Length == 0)
            return clone;

        if (this.Windows[0] == null)
            return clone;

        clone.Windows = new WindowData[this.Windows.Length];

        for(int i = 0; i < clone.Windows.Length; i++)
        {
            clone.Windows[i] = this.Windows[i].Clone() as WindowData;
        }

        return clone;
    }

    public override bool Equals(object obj)
    {
        if(obj is not WindowOpeningData)
            return false;

        WindowOpeningData other = obj as WindowOpeningData;

        if(base.Equals(obj) &&
           this.Angle == other.Angle &&
           this.Smooth == other.Smooth &&
           this.Sides == other.Sides)
        {
            return true;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
