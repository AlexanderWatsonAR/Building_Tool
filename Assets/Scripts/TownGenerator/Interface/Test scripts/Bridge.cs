using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour, IBuildable
{
    BridgeData bridgeData;


    public IBuildable Initialize(IData data)
    {
        bridgeData = data as BridgeData;
        return this;
    }

    public void Build()
    {
        Debug.Log("Test int: " + bridgeData.TestInt);
        Debug.Log("Test float: " + bridgeData.TestFloat);
    }

}
