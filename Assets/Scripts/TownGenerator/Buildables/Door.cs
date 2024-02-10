using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UIElements;
using ProMaths = UnityEngine.ProBuilder.Math;
using Edge = UnityEngine.ProBuilder.Edge;
using System;

public class Door : MonoBehaviour, IBuildable
{
    [SerializeField] private ProBuilderMesh m_DoorMesh;
    [SerializeField] private ProBuilderMesh m_DoorHandleMesh;
    [SerializeReference] private DoorData m_Data;

    public DoorData Data => m_Data;

    public IBuildable Initialize(IData data)
    {
        m_DoorMesh = GetComponent<ProBuilderMesh>();
        m_DoorHandleMesh = ProBuilderMesh.Create();
        m_DoorHandleMesh.transform.SetParent(transform, false);
        m_DoorHandleMesh.GetComponent<Renderer>().material = BuiltinMaterials.defaultMaterial;
        m_DoorHandleMesh.name = "Handle";
        m_Data = data as DoorData;
        return this;
    }

    public void Build()
    {
        if (!m_Data.ActiveElements.IsElementActive(DoorElement.Door))
            return;

        m_DoorMesh.CreateShapeFromPolygon(m_Data.ControlPoints, m_Data.Forward);
        ProBuilderMesh inside = Instantiate(m_DoorMesh);
        inside.faces[0].Reverse();

        m_DoorMesh.Extrude(m_DoorMesh.faces, ExtrudeMethod.FaceNormal, m_Data.Depth);
        m_DoorMesh.ToMesh();
        m_DoorMesh.Refresh();

        CombineMeshes.Combine(new ProBuilderMesh[] { m_DoorMesh, inside }, m_DoorMesh);
        DestroyImmediate(inside.gameObject);

        // Scale
        m_DoorMesh.transform.localScale = Vector3.one * m_Data.Scale;
        m_DoorMesh.LocaliseVertices();
        // Rotate
        m_DoorMesh.transform.localEulerAngles = m_Data.HingeEulerAngles;
        m_DoorMesh.LocaliseVertices(m_Data.HingePosition + m_Data.HingeOffset);
        m_DoorMesh.GetComponent<Renderer>().sharedMaterial = m_Data.Material;
        m_DoorMesh.Refresh();

        if (!m_Data.ActiveElements.IsElementActive(DoorElement.Handle))
            return;

        // Handle
        float size = m_Data.HandleSize * m_Data.HandleScale;
        Vector3[] points = MeshMaker.CreateNPolygon(8, size, size);
        Vector3 position = ProMaths.Average(points);

        // This is aligning the handle points with the door
        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, m_Data.Forward);

        for(int i  = 0; i < points.Length; i++)
        {
            Vector3 v = Quaternion.Euler(rotation.eulerAngles) * (points[i] - position) + position;
            points[i] = v;
        }

        m_DoorHandleMesh.CreateShapeFromPolygon(points, m_Data.Forward);
        m_DoorHandleMesh.Extrude(m_DoorHandleMesh.faces, ExtrudeMethod.FaceNormal, 0.1f);
        m_DoorHandleMesh.ToMesh();
        m_DoorHandleMesh.Refresh();

        IList<Edge> edgeList = m_DoorHandleMesh.faces[0].edges.ToList();

        Bevel.BevelEdges(m_DoorHandleMesh, edgeList, 0.1f);

        m_DoorHandleMesh.ToMesh();
        m_DoorHandleMesh.Refresh();

        m_DoorHandleMesh.transform.localPosition = m_Data.HandlePosition;
        //m_DoorHandleMesh.transform.localPosition = m_Data.Centre + (m_Data.Forward * m_Data.Depth);
        m_DoorHandleMesh.LocaliseVertices();
        m_DoorHandleMesh.Refresh();
        m_DoorHandleMesh.transform.localEulerAngles = m_Data.HingeEulerAngles;
        m_DoorHandleMesh.LocaliseVertices(m_Data.HingePosition + m_Data.HingeOffset);
        m_DoorHandleMesh.Refresh();
    }

    public void Demolish()
    {

    }

}
