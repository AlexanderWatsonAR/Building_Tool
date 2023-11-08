using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour, IBuildingComponent
{
    BridgeData bridgeData;


    public void Initialize(IData data)
    {
        bridgeData = data as BridgeData;
    }

    public void Build()
    {
        Debug.Log("Test int: " + bridgeData.TestInt);
        Debug.Log("Test float: " + bridgeData.TestFloat);
    }

}
