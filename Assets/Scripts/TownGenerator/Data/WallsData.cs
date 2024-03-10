using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsData : IData
{
    [SerializeField] WallData m_Wall;
    [SerializeField] WallData[] m_Walls;

    public WallData[] Walls { get { return m_Walls; } set { m_Walls = value; } }
    public WallData Wall { get { return m_Wall; } set { m_Wall = value; } }

    public WallsData(WallData[] walls, WallData wall)
    {
        m_Walls = walls;
        m_Wall = wall;
    }

    public WallsData(WallsData data) : this(data.Walls, data.Wall)
    {

    }

}
