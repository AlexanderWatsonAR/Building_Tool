using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestBridge : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject bridge = new GameObject("Bridge", typeof(Bridge));

        BridgeData data = new BridgeData(55, 1234.44f);

        bridge.GetComponent<Bridge>().Initialize(data);
        bridge.GetComponent<Bridge>().Build();
    }

}
