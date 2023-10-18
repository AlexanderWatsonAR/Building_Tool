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

public class Door : MonoBehaviour
{
    [SerializeField] private ProBuilderMesh m_DoorMesh;
    [SerializeField] private ProBuilderMesh m_DoorHandleMesh;
    [SerializeField] private DoorData m_Data;

    public DoorData DoorData => m_Data;

    public Door Initialize(DoorData data)
    {
        m_DoorMesh = GetComponent<ProBuilderMesh>();
        m_DoorHandleMesh = ProBuilderMesh.Create();
        m_DoorHandleMesh.transform.SetParent(transform, false);
        m_DoorHandleMesh.GetComponent<Renderer>().material = BuiltinMaterials.defaultMaterial;
        m_DoorHandleMesh.name = "Handle";
        m_Data = data;
        return this;
    }

    public Door Build()
    {
        m_DoorMesh.CreateShapeFromPolygon(m_Data.ControlPoints, 0, false);
        m_DoorMesh.ToMesh();
        m_DoorMesh.Refresh();
        m_DoorMesh.MatchFaceToNormal(m_Data.Forward);

        m_DoorMesh.Extrude(m_DoorMesh.faces, ExtrudeMethod.FaceNormal, m_Data.Depth);
        m_DoorMesh.ToMesh();
        m_DoorMesh.Refresh();

        ProBuilderMesh inside = ProBuilderMesh.Create();
        inside.transform.SetParent(transform, false);
        inside.CreateShapeFromPolygon(m_Data.ControlPoints, 0, true);
        inside.ToMesh();
        inside.Refresh();
        inside.MatchFaceToNormal(-m_Data.Forward);
        
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

        m_DoorHandleMesh.CreateShapeFromPolygon(points, 0, false);
        m_DoorHandleMesh.ToMesh();
        m_DoorHandleMesh.Refresh();
        m_DoorHandleMesh.MatchFaceToNormal(m_Data.Forward);
        m_DoorHandleMesh.Extrude(m_DoorHandleMesh.faces, ExtrudeMethod.FaceNormal, 0.1f);
        m_DoorHandleMesh.ToMesh();

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

        return this;
    }

}
