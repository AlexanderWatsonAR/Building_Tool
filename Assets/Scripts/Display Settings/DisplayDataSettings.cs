using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DisplayDataSettings
{
    [SerializeField] static DisplayData m_DisplayData;

    public static DisplayData Data
    {
        get
        {
            m_DisplayData ??= ScriptableObject.CreateInstance<DisplayData>().Initialize();
            return m_DisplayData;
        }
        set
        {
            m_DisplayData = value;
        }
    }
}
