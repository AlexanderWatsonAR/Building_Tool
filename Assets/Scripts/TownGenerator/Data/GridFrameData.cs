using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class GridFrameData : FrameData
{
    [SerializeField, Range(1, 5)] int m_Columns, m_Rows;

    public int Columns { get { return m_Columns; } set { m_Columns = value; } }
    public int Rows { get { return m_Rows; } set { m_Rows = value; } }

    public GridFrameData() : this(1,1)
    {

    }
    public GridFrameData(int columns, int rows) : base()
    {
        m_Columns = columns;
        m_Rows = rows;
    }
    public GridFrameData(PolygonData polygon, PolygonData[] holes, Vector3 normal, Vector3 up, float height, float width, float depth, float scale, int columns, int rows, Vector3 position) : base(polygon, holes, normal, up, height, width, depth, position, scale)
    {
        m_Columns = columns;
        m_Rows = rows;
    }
    public GridFrameData(GridFrameData data) :this(data.Polygon, data.Holes, data.Normal, data.Up, data.Height, data.Width, data.Depth, data.Scale, data.Columns, data.Rows, data.Position)
    {

    }

    public override bool Equals(object obj)
    {
        GridFrameData other = obj as GridFrameData;

        if (other == null)
            return false;

        if(m_Columns == other.Columns &&
           m_Rows == other.Rows &&
           base.Equals(obj))
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
