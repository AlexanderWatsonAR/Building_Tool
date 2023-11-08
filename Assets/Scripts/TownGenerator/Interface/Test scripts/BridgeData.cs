using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// test script to understand interfaces
[System.Serializable]
public class BridgeData : IData
{
    private float testFloat;
    private int testInt;

    public float TestFloat => testFloat;
    public int TestInt => testInt;

    public BridgeData(int testA, float testB)
    {
        testInt = testA;
        testFloat = testB;
    }
    
}
