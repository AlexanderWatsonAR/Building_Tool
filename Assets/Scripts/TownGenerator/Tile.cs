using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder;
using ProMaths = UnityEngine.ProBuilder.Math;
using ProHandle = UnityEngine.ProBuilder.HandleUtility;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.ProBuilder;
using System.Runtime.InteropServices.WindowsRuntime;

public class Tile : MonoBehaviour
{
    // Tile Structure data
    [SerializeField, HideInInspector] private Extrudable[] m_Extrudables;
    [SerializeField, HideInInspector] private bool m_FlipFace, m_ExtendHeightBeginning, m_ExtendHeightEnd, m_ExtendWidthBeginning, m_ExtendWidthEnd, m_SuspendConstruction;
    [SerializeField, HideInInspector] private Material m_Material;
    [SerializeField, HideInInspector] private Vector2 m_ExternalSideOffset, m_InternalSideOffset;
    [SerializeField, HideInInspector] private ProBuilderMesh m_External, m_Internal;
    [SerializeField, HideInInspector] private List<List<Vector3>> m_VerticesData;
    [SerializeField] private float m_Extend, m_Height;
    [SerializeField, Range(1, 5)] private float m_Scale;

    public float Scale => m_Scale;
    public float ExtendDistance => m_Extend;
    public float Height => m_Height;
    public bool ExtendHeightBeginning => m_ExtendHeightBeginning;
    public bool ExtendHeightEnd => m_ExtendHeightEnd;
    public bool ExtendWidthBeginning => m_ExtendWidthBeginning;
    public bool ExtendWidthEnd => m_ExtendWidthEnd;

    private Vector3[] TopPoints
    {
        get
        {
            Vector3[] topPoints = new Vector3[m_VerticesData.Count];

            for(int i = 0; i < m_VerticesData.Count; i++)
            {
                topPoints[i] = m_VerticesData[i][0];
            }

            return topPoints;
        }
    }

    private Vector3[] BottomPoints
    {
        get
        {
            Vector3[] bottomPoints = new Vector3[m_VerticesData.Count];

            for (int i = 0; i < m_VerticesData.Count; i++)
            {
                bottomPoints[i] = m_VerticesData[i][^1];
            }

            return bottomPoints;
        }
    }

    public void SetHeight(float height)
    {
        m_Height = height;
    }

    public void SetExtendDistance(float distance)
    {
        m_Extend = distance;
    }
    public Tile Initialize(Extrudable[] extrudables, bool flipFace = false)
    {
        m_SuspendConstruction = false;
        m_Extrudables = extrudables;  
        m_FlipFace = flipFace;
        ResetToDefault();
        TransformData();

        foreach (Extrudable extruder in m_Extrudables)
        {
            extruder.OnHaveExtrusionPointsChanged += OnHaveExtrusionPointsChanged;

            if (extruder.GetComponent<TransformCurve>() != null)
            {
                TransformCurve t = extruder.GetComponent<TransformCurve>();
                t.OnHasReshaped += OnHasReshaped;
            }
        }

        return this;
    }

    private void OnDestroy()
    {
        foreach (Extrudable extruder in m_Extrudables)
        {
            extruder.OnHaveExtrusionPointsChanged -= OnHaveExtrusionPointsChanged;

            if (extruder.GetComponent<TransformCurve>() != null)
            {
                TransformCurve t = extruder.GetComponent<TransformCurve>();
                t.OnHasReshaped -= OnHasReshaped;
            }
        }
    }

    public void StartConstruction()
    {
        if(!m_SuspendConstruction)
        {
            ResetToDefault();
            TransformData();
            Extend(m_ExtendHeightBeginning, m_ExtendHeightEnd, m_ExtendWidthBeginning, m_ExtendWidthEnd);
            ConstructTile();
        }
    }

    public void StartConstruction(object sender, System.EventArgs e)
    {
        m_SuspendConstruction = false;
        OnHaveExtrusionPointsChanged(sender, e);
    }

    public void SuspendConstruction(object sender, System.EventArgs e)
    {
        m_SuspendConstruction = true;
    }

    public Tile SetMaterial(Material material)
    {
        m_Material = material;
        return this;
    }

    public Tile SetUVOffset(Vector2 inside, Vector2 outside)
    {
        m_ExternalSideOffset = outside;
        m_InternalSideOffset = inside;
        return this;
    }

    private void OnHasReshaped(object sender, System.EventArgs e)
    {
        OnHaveExtrusionPointsChanged(sender, e);
    }

    private void OnHaveExtrusionPointsChanged(object sender, System.EventArgs e)
    {
        if(m_SuspendConstruction)
            return;

        StartConstruction();
    }

    public void ResetToDefault()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (Application.isEditor)
            {
                DestroyImmediate(child);
            }
            else
            {
                Destroy(child);
            }
        }

        if (transform.childCount > 0)
        {
            ResetToDefault();
        }
    }

    private List<List<Vector3>> TransformData()
    {
        m_VerticesData ??= new List<List<Vector3>>();
        m_VerticesData.Clear();

        for (int i = 0; i < m_Extrudables.Length; i++)
        {
            m_VerticesData.Add(m_Extrudables[i].ExtrusionPositions.ToList());
        }

        return m_VerticesData;
    }

    public Tile Extend(bool heightBeginning, bool heightEnd, bool widthBeginning, bool widthEnd)
    {
        m_ExtendHeightBeginning = heightBeginning;
        m_ExtendHeightEnd = heightEnd;
        m_ExtendWidthBeginning = widthBeginning;
        m_ExtendWidthEnd = widthEnd;

        if (heightBeginning)
        {
            List<Vector3> startEnd = new (2);

            int count = m_VerticesData.Count;
            int incriment = count - 1;

            for (int i = 0; i < count; i += incriment)
            {
                Vector3 dir = Vector3Extensions.DirectionToTarget(m_VerticesData[i][1], m_VerticesData[i][0]);
                startEnd.Add(m_VerticesData[i][0] + (dir * m_Extend));
            }

            Vector3[] data = Vector3Extensions.LerpCollection(startEnd[0], startEnd[1], count).ToArray();

            for (int i = 0; i < data.Length; i ++)
            {
                m_VerticesData[i].Insert(0, data[i]);
            }
        }
        if (heightEnd)
        {
            List<Vector3> startEnd = new(2);

            int count = m_VerticesData.Count;
            int incriment = count - 1;

            for (int i = 0; i < count; i += incriment)
            {
                Vector3 dir = Vector3Extensions.DirectionToTarget(m_VerticesData[i][^2], m_VerticesData[i][^1]);
                startEnd.Add(m_VerticesData[i][^1] + (dir * m_Extend));
            }

            Vector3[] data = Vector3Extensions.LerpCollection(startEnd[0], startEnd[1], count).ToArray();

            for (int i = 0; i < data.Length; i++)
            {
                m_VerticesData[i].Add(data[i]);
            }
        }
        if(widthBeginning)
        {
            List<Vector3> data = new List<Vector3>();

            int count = m_VerticesData[0].Count;

            for (int i = 0; i < count; i ++)
            {
                Vector3 dir = Vector3Extensions.DirectionToTarget(m_VerticesData[1][i], m_VerticesData[0][i]);
                data.Add(m_VerticesData[0][i] + (dir * m_Extend));
            }

            m_VerticesData.Insert(0, data);
        }
        if(widthEnd)
        {
            List<Vector3> data = new List<Vector3>();

            int count = m_VerticesData[0].Count;
            for (int i = 0; i < count; i++)
            {
                Vector3 dir = Vector3Extensions.DirectionToTarget(m_VerticesData[^2][i], m_VerticesData[^1][i]);
                data.Add(m_VerticesData[^1][i] + (dir * m_Extend));
            }

            m_VerticesData.Add(data);
        }

        return this;
    }

    private void ConstructTile()
    {
        if (m_Extrudables == null)
        {
            return;
        }

        int width, height;
        height = m_VerticesData[0].Count;
        width = m_VerticesData.Count;

        Vector3[] vertices = new Vector3[width * height];
        int numberOfQuads = (width - 1) * (height - 1);
        int[] triangles = new int[numberOfQuads * 6];

        int count = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                vertices[count] = m_VerticesData[i][j];
                count++;
            }
        }

        int vert = 0, tris = 0;

        for (int i = 0; i < width - 1; i++)
        {
            for (int j = 0; j < height - 1; j++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + (height - 1) + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + (height - 1) + 1;
                triangles[tris + 5] = vert + (height - 1) + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        if (m_FlipFace)
        {
            triangles = triangles.Reverse().ToArray();
        }

        Face[] externalFaces = new Face[] { new Face(triangles) };

        ProBuilderMesh inside = ProBuilderMesh.Create(vertices, new Face[] { new Face(triangles.Reverse()) });
        
        inside.GetComponent<MeshRenderer>().sharedMaterial = m_Material;

        inside.ToMesh();
        inside.Refresh();

        ProBuilderMesh outside = ProBuilderMesh.Create(vertices, externalFaces);

        outside.transform.SetParent(transform, true);
        outside.GetComponent<MeshRenderer>().sharedMaterial = m_Material;
        Vector3 faceNormal = outside.FaceNormal(outside.faces[0]); // dir

        outside.Extrude(outside.faces, ExtrudeMethod.FaceNormal, 0);

        Vertex[] points = outside.GetVertices();
        int[] distinctIndices = outside.faces[0].distinctIndexes.ToArray();

        int[,] indices = new int[m_VerticesData.Count, m_VerticesData[0].Count];

        // Load indices in a 2d array to match the vertices data.
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < distinctIndices.Length; k++)
                {
                    if (m_VerticesData[i][j] == points[distinctIndices[k]].position)
                    {
                        indices[i, j] = distinctIndices[k];
                    }
                }
            }
        }

        for (int i = 0; i < width; i++)
        {
            float index = 0;
            float counter = (float)height / (height - 1);

            // first & second should be on the same column but different rows
            int first = indices[i, 0];
            int second = indices[i, indices.GetLength(1)-1];
            
            Vector3 firstPoint = points[first].position;
            Vector3 secondPoint = points[second].position;

            Vector3 dir = Vector3Extensions.DirectionToTarget(firstPoint, secondPoint);

            Vector3 projectedSecondPoint = secondPoint + (dir * m_Height);

            float y = secondPoint.y - projectedSecondPoint.y;

            for (int j = 0; j < height; j++)
            {
                float t = (float)index / (float)height;

                float a = Mathf.Lerp(m_Height, 0, t);
                float b = Mathf.Lerp(0, m_Height, t);
                float c = Mathf.Lerp(0, y, t);

                Vector3 a2 = Vector3.up * a;
                Vector3 b2 = dir * b;
                Vector3 c2 = Vector3.up * c;

                points[indices[i, j]].position += a2;
                points[indices[i, j]].position += b2;
                points[indices[i, j]].position += c2;

                List<int> shared = outside.GetCoincidentVertices(new int[] { indices[i, j] });

                for (int k = 0; k < shared.Count; k++)
                {
                    points[shared[k]].position = points[indices[i, j]].position;
                }
                index += counter;
            }
        }


        outside.SetVertices(points);
        outside.ToMesh();
        outside.Refresh();

        inside.transform.SetParent(outside.transform, true);

        m_External = outside;
        m_Internal = inside;
    }

    private void OnDrawGizmos()
    {
    }
}
