using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Connectors are beams that share the same start position.
/// </summary>
[System.Serializable]
public class Connector
{
    [SerializeField, HideInInspector] private List<Beam> m_BeamList;

    public Connector()
    {
        m_BeamList??= new ();
    }
    /// <summary>
    /// Rebuilds beam connections. Assumes the end position is the same.
    /// </summary>
    /// <param name="beamIndex"></param>
    /// <param name="startPosition"></param>
    /// <param name="drawFromDefault"></param>
    /// <returns></returns>
    public Connector Rebuild(Vector3 startPosition, bool drawFromDefault = true)
    {
        foreach (Beam beam in m_BeamList)
        {
            beam.Build(startPosition, beam.EndPosition, beam.Steps, drawFromDefault);
        }
        return this;
    }

    public Connector(IEnumerable<Beam> beamsRange)
    {
        m_BeamList ??= new();
        AddRange(beamsRange);
    }

    public void AddChild(Beam beam)
    {
        m_BeamList.Add(beam);
    }

    public void AddRange(IEnumerable<Beam> beams)
    {
        m_BeamList.AddRange(beams);
    }

    public Beam GetBeamAtIndex(int index)
    {
        return m_BeamList[index];
    }

    public int GetBeamCount()
    {
        return m_BeamList.Count;
    }
}
