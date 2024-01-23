using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoreyDataWrapper
{
    public StoreyData Data;

    public StoreyDataWrapper(StoreyData data)
    {
        this.Data = data;
    }

    // This wrapper class is here purely so that a reference to storey data can be maintained after serialization. 

    //[SerializeReference] private StoreyData m_Data;
    //public StoreyData Data { get { return m_Data; } set { m_Data = value; } }
}
