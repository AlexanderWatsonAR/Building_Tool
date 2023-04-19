using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using System.Linq;
using UnityEngine.ProBuilder.Shapes;

[System.Serializable]
public class Beam : Extrudable
{
    [SerializeField, HideInInspector] private TransformCurve m_TransformCurve;
    [SerializeField, HideInInspector] private Vector3 m_StartPosition, m_EndPosition;
    //[SerializeField, HideInInspector] private ProBuilderMesh m_OriginalMeshState;
    //[SerializeField, HideInInspector] bool m_IsInitialized;

    public event EventHandler OnConnectorBeginReshape;
    public event EventHandler OnConnectorEndReshape;

    private List<Connector> m_Connectors;

    public Vector3 StartPosition => m_StartPosition;
    public Vector3 EndPosition => m_EndPosition;
    public int Steps => m_Steps;

    private Beam Constructor()
    {
        if (!m_IsInitialized)
            Initialize();
        return this;
    }

    protected override void Initialize()
    {
        base.Initialize();
        m_Connectors ??= new();
        
        if(TryGetComponent(out m_TransformCurve))
        {
            m_TransformCurve.OnHasReshaped += TransformCurve_HasReshaped;
        }
        //m_IsInitialized = true;
        //Debug.Log("Beam Init");
    }

    /// <summary>
    /// Start & end pos require local coordinates.
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="endPosition"></param>
    /// <param name="steps"></param>
    /// <returns></returns>
    public Beam Build(Vector3 startPosition, Vector3 endPosition, int steps, bool drawFromDefault = false)
    {
        m_StartPosition = startPosition;
        m_EndPosition = endPosition;
        m_Steps = steps;

        if (drawFromDefault)
        {
            ResetMeshToDefault();
        }

        transform.localPosition = startPosition;
        transform.forward = Vector3Extensions.GetDirectionToTarget(startPosition, endPosition);
        transform.eulerAngles += (Vector3.right * 90);

        float distance = Vector3.Distance(startPosition, endPosition);
        Extrude(m_ProBuilderMesh.faces, ExtrudeMethod.FaceNormal, distance, steps);

        if(m_TransformCurve != null)
            m_TransformCurve.Initialize(m_TransformCurve.TransformCurveData).Reshape();

        return this;
    }

    public Beam Build(Vector3 startPosition, Vector3 endPosition, bool drawFromDefault = false)
    {
        int steps = Mathf.FloorToInt(Vector3.Distance(startPosition, endPosition));
        return Build(startPosition, endPosition, steps, drawFromDefault);
    }

    /// <summary>
    /// Instantiates a new beam object. Building a beam (I.E. Extruding) is done by calling build.
    /// </summary>
    /// <param name="beamPrefab"></param>
    /// <param name="parent"></param>
    /// <param name="worldPositionStays"></param>
    /// <returns></returns>
    public static Beam Create(GameObject beamPrefab, Transform parent, bool worldPositionStays = false)
    {
        return Instantiate(beamPrefab, parent, worldPositionStays).GetComponent<Beam>().Constructor();
    }
    /// <summary>
    /// Instantiates a new beam object. Building a beam (I.E. Extruding) is done by calling build.
    /// </summary>
    /// <param name="beamPrefab"></param>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <param name="worldPositionStays"></param>
    /// <returns></returns>
    public static Beam Create(GameObject beamPrefab, Transform parent, string name, bool worldPositionStays = false)
    {
        Beam beam = Instantiate(beamPrefab, parent, worldPositionStays).GetComponent<Beam>().Constructor();
        beam.name = name;
        return beam;
    }

    public static IEnumerable<Beam> Create(GameObject beamPrefab, Transform parent, int numberOfbeams, bool worldPositionStays = false)
    {
        Beam[] beams = new Beam[numberOfbeams];

        for(int i = 0; i < numberOfbeams; i++)
        {
            beams[i] = Create(beamPrefab, parent, worldPositionStays);
        }
        return beams;
    }
    private void TransformCurve_HasReshaped(object sender, System.EventArgs e)
    {
        UpdateConnectors();
    }

    private void UpdateConnectors()
    {
        if (m_Connectors.Count == 0)
            return;

        OnConnectorBeginReshape?.Invoke(this, EventArgs.Empty);

        Vector3[] localExtrusionPositions = transform.parent.InverseTransformPoints(ExtrusionPositions).ToArray();

        for (int i = 0; i < m_Connectors.Count; i++)
        {
            m_Connectors[i].Rebuild(localExtrusionPositions[i]);
        }

        OnConnectorEndReshape?.Invoke(this, EventArgs.Empty);
    }

    public void AddConnector(Connector connector)
    {
        m_Connectors.Add(connector);
    }

    public void AddRange(IEnumerable<Connector> range)
    {
        m_Connectors.AddRange(range);
    }

    public Connector GetLastConnector()
    {
        return m_Connectors[^1];
    }

    public Connector GetConnectorAtIndex(int index)
    {
        return m_Connectors[index];
    }

    public int ConectorCount()
    {
        return m_Connectors.Count;
    }

#if UNITY_EDITOR
    private void OnDestroy()
    {
        m_TransformCurve.OnHasReshaped -= TransformCurve_HasReshaped;
    }
#endif

}
