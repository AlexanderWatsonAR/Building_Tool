using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pane : Polygon3D
{
    [SerializeField] Polygon3DData m_Data;

    public Polygon3DData Data => m_Data;

    public override IBuildable Initialize(IData data)
    {
        base.Initialize(data);
        m_Data = data as Polygon3DData;
        return this;
    }


}
