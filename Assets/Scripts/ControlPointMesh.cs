using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

public class ControlPointMesh : MonoBehaviour
{
    [SerializeField] private List<List<Vector3>> m_Holes;
    [SerializeField] private float m_Depth;
    [SerializeField] private float minX, minY, maxY, minZ, maxZ;
    [SerializeField] private List<Vector3> controlPoints = new List<Vector3>();

    public List<Vector3> ControlPoints
    {
        get
        {
            if (controlPoints.Count != 0)
                return controlPoints;
            else
                return GetControlPoints();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Holes ??= new List<List<Vector3>>();
    }

    public List<Vector3> GetControlPoints()
    {
        // y & z. x = depth of wall

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        minX = mesh.bounds.min.x;
        minY = mesh.bounds.min.y;
        maxY = mesh.bounds.max.y;
        minZ = mesh.bounds.min.z;
        maxZ = mesh.bounds.max.z;

        m_Depth = Mathf.Abs(mesh.bounds.min.x) + Mathf.Abs(mesh.bounds.max.x);

        GameObject pointA = new GameObject("Point A");
        pointA.transform.SetParent(transform, false);
        pointA.transform.localPosition = new Vector3(minX, minY, minZ);

        GameObject pointB = new GameObject("Point B");
        pointB.transform.SetParent(transform, false);
        pointB.transform.localPosition = new Vector3(minX, minY, maxZ);

        GameObject pointC = new GameObject("Point C");
        pointC.transform.SetParent(transform, false);
        pointC.transform.localPosition = new Vector3(minX, maxY, maxZ);

        GameObject pointD = new GameObject("Point D");
        pointD.transform.SetParent(transform, false);
        pointD.transform.localPosition = new Vector3(minX, maxY, minZ);


        List<Vector3> points = new List<Vector3>()
        {
            pointA.transform.localPosition,
            pointD.transform.localPosition,
            pointC.transform.localPosition,
            pointB.transform.localPosition,
        };

        controlPoints = points;
        if(Application.isEditor)
        {
            DestroyImmediate(pointA);
            DestroyImmediate(pointB);
            DestroyImmediate(pointC);
            DestroyImmediate(pointD);
        }
        else
        {
            Destroy(pointA);
            Destroy(pointB);
            Destroy(pointC);
            Destroy(pointD);
        }
        

        //A,D,C,B
        return points;
    }

    public void InsertOutlines(List<GameObject> outlineList, float offset)
    {
        float absOffset = Mathf.Abs(offset);

        Vector3 originalCenter = GetComponent<Renderer>().bounds.center;

        List<Vector3> controlPoints = ControlPoints;

        for (int i = 0; i < outlineList.Count; i++)
        {
            Vector3[] vertices = outlineList[i].GetComponent<MeshFilter>().sharedMesh.vertices;
            Vector3[] localVertices = new Vector3[vertices.Length];

            for (int j = 0; j < vertices.Length; j++)
            {
                GameObject point = new GameObject("Point " + j.ToString());
                point.transform.SetParent(outlineList[i].transform, false);
                point.transform.localPosition = vertices[j];
                point.transform.SetParent(transform, true);
                //localVertices[j] = point.transform.localPosition;

                if (point.transform.localPosition.y < minY)
                {
                    point.transform.localPosition = new Vector3(point.transform.localPosition.x, minY + 0.01f, point.transform.localPosition.z);
                }

                if (point.transform.localPosition.y > maxY)
                {
                    point.transform.localPosition = new Vector3(point.transform.localPosition.x, maxY - 0.01f, point.transform.localPosition.z);
                }

                if (point.transform.localPosition.z < minZ)
                {
                    point.transform.localPosition = new Vector3(point.transform.localPosition.x, point.transform.localPosition.y, minZ + 0.01f);
                }

                if (point.transform.localPosition.z > maxZ)
                {
                    point.transform.localPosition = new Vector3(point.transform.localPosition.x,  point.transform.localPosition.y, maxZ - 0.01f);
                }

                localVertices[j] = new Vector3(minX + absOffset, point.transform.localPosition.y, point.transform.localPosition.z);

                //localVertices[j] = new Vector3(minX + absOffset, vertices[j].y, vertices[j].z);

                if(Application.isEditor)
                {
                    DestroyImmediate(point);
                }
                else
                {
                    Destroy(point);
                }
                
            }
            AddHole(localVertices);
        }

        Vector2 uvOffset = -gameObject.GetComponent<MeshFilter>().sharedMesh.uv[0];

        ProBuilderMesh meshWithHole = ProBuilderMesh.Create();
        meshWithHole.name = "Mesh With Hole";
        meshWithHole.CreateShapeFromPolygon(controlPoints, m_Depth, false, (IList<IList<Vector3>>) m_Holes);

        gameObject.GetComponent<MeshFilter>().mesh = meshWithHole.GetComponent<MeshFilter>().sharedMesh;

        new MeshImporter(gameObject).Import();

        ProBuilderMesh proBuilderMesh = gameObject.GetComponent<ProBuilderMesh>();

        //AutoUnwrapSettings settings = new AutoUnwrapSettings();
        //settings.scale = Vector2.zero;
        //settings.offset = uvOffset;

        //foreach (Face f in proBuilderMesh.faces)
        //{
        //    f.manualUV = false;
        //    f.uv = settings;
        //}

        proBuilderMesh.ToMesh();
        proBuilderMesh.Refresh();

        proBuilderMesh.SetPivot(originalCenter);

        // Adjust Mesh if its offset

        if (GetComponent<MeshFilter>().sharedMesh.bounds.min.x <= minX + (m_Depth / 2) &&
           GetComponent<MeshFilter>().sharedMesh.bounds.min.x >= minX - (m_Depth / 2))
        {
        }
        else
        {
            float difference = Mathf.Abs(GetComponent<MeshFilter>().mesh.bounds.min.x) - Mathf.Abs(minX);

            Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = GetComponent<MeshFilter>().mesh.vertices[i];

                vertices[i] = new Vector3(vertex.x + difference, vertex.y, vertex.z);
            }

            GetComponent<MeshFilter>().mesh.vertices = vertices;
        }

        if (Application.isEditor)
        {
            DestroyImmediate(meshWithHole.gameObject);
        }
        else
        {
            Destroy(meshWithHole.gameObject);
        }
    }

    public void AddHole(Vector3[] holeVertices)
    {
        if (m_Holes == null)
            m_Holes = new List<List<Vector3>>();

        m_Holes.Add(new List<Vector3>());

        for (int i = 0; i < holeVertices.Length; i++)
        {
            m_Holes[m_Holes.Count - 1].Add(holeVertices[i]);
        }

    }

    public IList<Vector3> GetHoleAtIndex(int index)
    {
        return m_Holes[index];
    }

    public int NumberOfHoles()
    {
        return m_Holes.Count;
    }

}
