using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 01-04-2024
// This isn't an April fools.
// Trying to consider different ways to manage the data.
// In this project, we essentially have a data structure 
// which muliple objects can gain access to via the inspector.
// Could we use a non-hirarchical data structure?


public sealed class BuildingManager
{
    private static BuildingManager m_Instance = null;

    private static BuildingData m_Data;

    private BuildingManager() { }

    public static BuildingManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new BuildingManager();
            }

            return m_Instance;
        }
    }

    public static BuildingData Data
    {
        get
        {
            return m_Data;
        }
    }

}
