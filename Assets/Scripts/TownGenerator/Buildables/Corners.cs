using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corners : MonoBehaviour, IBuildable
{
    [SerializeField] CornersData m_Data;
    [SerializeField] Corner[] m_Corners;

    public IBuildable Initialize(IData data)
    {
        m_Data = data as CornersData;
        m_Corners = new Corner[m_Data.Corners.Length];
        return this;
    }

    public void Build()
    {
        m_Corners.BuildCollection();
    }

    public void Demolish()
    {
        m_Corners.DemolishCollection();
    }


}
