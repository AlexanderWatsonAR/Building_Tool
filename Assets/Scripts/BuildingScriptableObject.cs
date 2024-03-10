using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I'm trying to think of building data as largely non game object specific.
// I like the idea of just draw and dropping a serialized object (storing the building data)
// onto a building and it constructing a building from that data.
// This script is a container/wrapper for data that works largely the same as the gameObject equivalent


public class BuildingScriptableObject : ScriptableObject
{
    [SerializeField] BuildingData m_Data;

    public BuildingData Data { get { return m_Data; } set { m_Data = value; } }

    public BuildingScriptableObject()
    {
        m_Data = new BuildingData();
    }

}
