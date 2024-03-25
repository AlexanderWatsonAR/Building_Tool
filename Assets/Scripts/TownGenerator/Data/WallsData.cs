using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallsData : IData, ICloneable
{
    [SerializeField] WallData m_Wall; // Is it appropriate to store this here? This is really just an inspector tool.
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

    public object Clone()
    {
        WallsData clone = this.MemberwiseClone() as WallsData;
        clone.Wall = this.Wall.Clone() as WallData;
        return Clone();
    }
}
