using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShapeScriptableObject : ScriptableObject
{
    [SerializeReference] protected Shape m_Shape;
    public Shape Shape => m_Shape;

    private void Reset()
    {
        Initialize();
    }

    /// <summary>
    /// Called on Reset.
    /// </summary>
    public abstract void Initialize();
}
