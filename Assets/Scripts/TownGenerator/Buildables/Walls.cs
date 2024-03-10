using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walls : MonoBehaviour, IBuildable
{
    [SerializeField] WallsData m_Data;
    [SerializeField] Wall[] m_Walls;

    public IBuildable Initialize(IData data)
    {
        m_Data = data as WallsData;
        m_Walls = new Wall[m_Data.Walls.Length];
        return this;
    }

    public void Build()
    {
        m_Walls.BuildCollection();
    }

    public void Demolish()
    {
        m_Walls.DemolishCollection();
    }

}
