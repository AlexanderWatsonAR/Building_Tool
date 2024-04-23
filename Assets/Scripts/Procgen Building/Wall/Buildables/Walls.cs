using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Wall
{
    public class Walls : Buildable
    {
        [SerializeField] WallsData m_WallsData;
        [SerializeField] Wall[] m_Walls;

        public override Buildable Initialize(DirtyData data)
        {
            base.Initialize(data);
            m_WallsData = data as WallsData;
            m_Walls = new Wall[m_WallsData.Walls.Length];
            return this;
        }

        public override void Build()
        {
            m_Walls.BuildCollection();
        }

        public override void Demolish()
        {
            m_Walls.DemolishCollection();
        }
    }
}
