using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using System.Linq;
using Unity.VisualScripting;

public class RoofTile : MonoBehaviour
{
    // Tile Structure data
    [SerializeField, HideInInspector] private Extrudable[] m_Extrudables;
    [SerializeField, HideInInspector] private bool m_FlipFace, m_ExtendHeightBeginning, m_ExtendHeightEnd, m_ExtendWidthBeginning, m_ExtendWidthEnd, m_SuspendConstruction;
    [SerializeField, HideInInspector] private Material m_Material;
    [SerializeField, HideInInspector] private Vector2 m_ExternalSideOffset, m_InternalSideOffset;
    [SerializeField, HideInInspector] private ProBuilderMesh m_External, m_Internal;
    [SerializeField, HideInInspector] private List<List<Vector3>> m_VerticesData;

    public RoofTile Initialize(Extrudable[] extrudables, bool flipFace = false)
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
            ConstructTile();
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

    public RoofTile SetMaterial(Material material)
    {
        m_Material = material;
        return this;
    }

    public RoofTile SetUVOffset(Vector2 inside, Vector2 outside)
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

        ResetToDefault();
        TransformData();
        Extend(m_ExtendHeightBeginning, m_ExtendHeightEnd, m_ExtendWidthBeginning, m_ExtendWidthEnd);
        ConstructTile();
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

    public RoofTile Extend(bool heightBeginning, bool heightEnd, bool widthBeginning, bool widthEnd)
    {
        m_ExtendHeightBeginning = heightBeginning;
        m_ExtendHeightEnd = heightEnd;
        m_ExtendWidthBeginning = widthBeginning;
        m_ExtendWidthEnd = widthEnd;

        if (heightBeginning)
        {
            for (int i = 0; i < m_VerticesData.Count; i++)
            {
                Vector3 dir = Vector3Extensions.GetDirectionToTarget(m_VerticesData[i][1], m_VerticesData[i][0]);
                m_VerticesData[i].Insert(0, m_VerticesData[i][0] + (dir * 0.75f));
            }
        }
        if (heightEnd)
        {
            for(int i = 0; i < m_VerticesData.Count; i++)
            {
                Vector3 dir = Vector3Extensions.GetDirectionToTarget(m_VerticesData[i][^2], m_VerticesData[i][^1]);
                m_VerticesData[i].Add(m_VerticesData[i][^1] + (dir * 0.75f));
            }
        }
        if(widthBeginning)
        {
            List<Vector3> extension = new (m_VerticesData[0].Count);

            for (int i = 0; i < extension.Capacity; i++)
            {
                Vector3 dir = Vector3Extensions.GetDirectionToTarget(m_VerticesData[1][i], m_VerticesData[0][i]);
                extension.Add(m_VerticesData[0][i] + (dir * 0.75f));
            }

            m_VerticesData.Insert(0, extension);

        }
        if(widthEnd)
        {
            List<Vector3> extension = new(m_VerticesData[0].Count);

            for (int i = 0; i < extension.Capacity; i++)
            {
                Vector3 dir = Vector3Extensions.GetDirectionToTarget(m_VerticesData[^2][i], m_VerticesData[^1][i]);
                extension.Add(m_VerticesData[^1][i] + (dir * 0.75f));
            }

            m_VerticesData.Add(extension);
        }

        return this;
    }

    private void ConstructTile()
    {
        //Debug.Log(name + " constructed");

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

        ProBuilderMesh tile = ProBuilderMesh.Create(vertices, externalFaces);

        tile.transform.SetParent(transform, true);
        tile.GetComponent<MeshRenderer>().sharedMaterial = m_Material;

        //tile.SetMeshColour(m_ExternalSideOffset);

        tile.Extrude(tile.faces, ExtrudeMethod.FaceNormal, 0);

        float tileHeight = 0.25f;

        tile.TranslateVertices(externalFaces[0].distinctIndexes, Vector3.up * tileHeight);

        tile.ToMesh();
        tile.Refresh();

        ProBuilderMesh mesh = ProBuilderMesh.Create(vertices, new Face[] { new Face(triangles.Reverse()) });
        mesh.transform.SetParent(tile.transform, true);
        mesh.GetComponent<MeshRenderer>().sharedMaterial = m_Material;

        Normals.CalculateNormals(mesh);

        //mesh.SetMeshColour(m_InternalSideOffset);

        mesh.ToMesh();
        mesh.Refresh();
    }

    private void Detailing()
    {
        // ToDO: add details like the bevel lining at the top, or the 
    }

    private void ConstructBlockGrid()
    {
        List<Vector3[]> data = new List<Vector3[]>();

        for (int i = 0; i < m_Extrudables.Length; i++)
        {
            data.Add(m_Extrudables[i].ExtrusionPositions);
        }

        Vector3[][] dataPoints = data.ToArray();

        //if (m_reverseData)
        //{
        //    dataPoints.Reverse();
        //}

        int width, height;
        float depth;

        width = dataPoints.GetLength(0);
        height = dataPoints[0].GetLength(0);
        depth = 0.25f;

        Vector3[] vertices = new Vector3[(width * height) * 2];
        List<int> triangles = new List<int>(new int[36]);

        // Define Cube points.
        vertices[0] = dataPoints[0][0];
        vertices[1] = dataPoints[1][0];
        vertices[2] = dataPoints[1][1];
        vertices[3] = dataPoints[0][1];
        // Depth
        vertices[4] = dataPoints[0][1] + (Vector3.up * depth);
        vertices[5] = dataPoints[1][1] + (Vector3.up * depth);
        vertices[6] = dataPoints[1][0] + (Vector3.up * depth);
        vertices[7] = dataPoints[0][0] + (Vector3.up * depth);

        List<Face> faces = new List<Face>();

        // Triangles
        {
            // Front Face
            triangles[0] = 0;
            triangles[1] = 2;
            triangles[2] = 1;
            triangles[3] = 0;
            triangles[4] = 3;
            triangles[5] = 2;

            // Top Face
            triangles[6] = 2;
            triangles[7] = 3;
            triangles[8] = 4;
            triangles[9] = 2;
            triangles[10] = 4;
            triangles[11] = 5;

            // Right Face
            triangles[12] = 1;
            triangles[13] = 2;
            triangles[14] = 5;
            triangles[15] = 1;
            triangles[16] = 5;
            triangles[17] = 6;

            // Left Face
            triangles[18] = 0;
            triangles[19] = 7;
            triangles[20] = 4;
            triangles[21] = 0;
            triangles[22] = 4;
            triangles[23] = 3;

            // Back Face
            triangles[24] = 5;
            triangles[25] = 4;
            triangles[26] = 7;
            triangles[27] = 5;
            triangles[28] = 7;
            triangles[29] = 6;

            // Bottom Face
            triangles[30] = 0;
            triangles[31] = 6;
            triangles[32] = 7;
            triangles[33] = 0;
            triangles[34] = 1;
            triangles[35] = 6;
        }

        for(int i = 0; i < triangles.Count; i+=6)
        {
            faces.Add(new Face(new int[]
            {
                triangles[i], triangles[i + 1], triangles[i + 2],
                triangles[i + 3], triangles[i + 4], triangles[i + 5]
            }));
        }

        ProBuilderMesh mesh = ProBuilderMesh.Create(vertices, faces );
        mesh.SetMaterial(mesh.faces, m_Material);

        mesh.transform.SetParent(transform, true);

        //int count = 8;
        //for (int i = 2; i < height; i++)
        //{
        //    for (int j = 2; j < width; j++)
        //    {
        //        //triangles.AddRange(new int[30]);

        //        int heightIndex = 1;
        //        int widthIndex = 1;

        //        if(i == height -1)
        //            heightIndex = 0;

        //        if (j == width - 1)
        //            widthIndex = 0;

        //        Vector3 top, bottom, topBack, bottomBack;

        //        top = dataPoints[j][i];



        //        //Vector3 topRightVertex = dataPoints[j][i + heightIndex];
        //        //Vector3 bottomRightVertex = dataPoints[j][i + heightIndex];
        //        //Vector3 topLeftVertex = dataPoints[j + widthIndex][i];
        //        //Vector3 bottomLeftVertex = dataPoints[j + widthIndex][i];

        //        //vertices[count] = dataPoints[j][i];
        //        //vertices[count + 1] = dataPoints[j][i + heightIndex];
        //        //vertices[count + 2] = dataPoints[j + 1][i + 1];
        //        //vertices[count + 3] = dataPoints[j + 1][i];

        //        //// Depth
        //        //vertices[count + 4] = dataPoints[j + 1][i] + (Vector3.up * depth);
        //        //vertices[count + 5] = dataPoints[j + 1][i + 1] + (Vector3.up * depth);
        //        //vertices[count + 6] = dataPoints[j][i + 1] + (Vector3.up * depth);
        //        //vertices[count + 7] = dataPoints[j][i] + (Vector3.up * depth);

        //        count += 4;

        //    }
    //}

    }
}
