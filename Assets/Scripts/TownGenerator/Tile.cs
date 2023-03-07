using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using System.Linq;
using Unity.VisualScripting;

public class Tile : MonoBehaviour
{
    // Tile Structure data
    [SerializeField, HideInInspector] private Extrudable[] m_Extrudables;
    [SerializeField, HideInInspector] private bool m_FlipFace, m_ExtendHeightBeginning, m_ExtendHeightEnd, m_ExtendWidthBeginning, m_ExtendWidthEnd, m_SuspendConstruction;
    [SerializeField, HideInInspector] private Material m_Material;
    [SerializeField, HideInInspector] private Vector2 m_ExternalSideOffset, m_InternalSideOffset;
    [SerializeField, HideInInspector] private ProBuilderMesh m_External, m_Internal;
    [SerializeField, HideInInspector] private List<List<Vector3>> m_VerticesData;
    

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

    public Tile Extend(bool heightBeginning, bool heightEnd, bool widthBeginning, bool widthEnd)
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

        ProBuilderMesh outside = ProBuilderMesh.Create(vertices, externalFaces);

        outside.transform.SetParent(transform, true);
        outside.GetComponent<MeshRenderer>().sharedMaterial = m_Material;

        //tile.SetMeshColour(m_ExternalSideOffset);

        outside.Extrude(outside.faces, ExtrudeMethod.FaceNormal, 0);

        float tileHeight = 0.25f;

        outside.TranslateVertices(externalFaces[0].distinctIndexes, Vector3.up * tileHeight);

        outside.ToMesh();
        outside.Refresh();

        ProBuilderMesh inside = ProBuilderMesh.Create(vertices, new Face[] { new Face(triangles.Reverse()) });
        inside.transform.SetParent(outside.transform, true);
        inside.GetComponent<MeshRenderer>().sharedMaterial = m_Material;

        Normals.CalculateNormals(inside);

        //mesh.SetMeshColour(m_InternalSideOffset);

        inside.ToMesh();
        inside.Refresh();

        //gameObject.AddComponent<MeshFilter>();
        //gameObject.AddComponent<MeshRenderer>();
        //new MeshImporter(gameObject).Import();

        //ProBuilderMesh tileMesh = ProBuilderMesh.Create();

        //CombineMeshes.Combine(new ProBuilderMesh[] { outside, inside }, tileMesh);

        //if(Application.isEditor)
        //{
        //    DestroyImmediate(outside);
        //    DestroyImmediate(inside);
        //}

    }
}
