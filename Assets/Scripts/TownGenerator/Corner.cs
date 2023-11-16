using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corner : MonoBehaviour, IBuildable
{
    [SerializeField] private CornerData m_Data;

    public IBuildable Initialize(IData data)
    {
        m_Data = data as CornerData;
        return this;
    }

    public void Build()
    {
        
    }


}
