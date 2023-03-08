using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder;
using ProMaths = UnityEngine.ProBuilder.Math;
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
    [SerializeField] private float m_Extend;

    public float ExtendDistance => m_Extend;
    public bool ExtendHeightBeginning => m_ExtendHeightBeginning;
    public bool ExtendHeightEnd => m_ExtendHeightEnd;
    public bool ExtendWidthBeginning => m_ExtendWidthBeginning;
    public bool ExtendWidthEnd => m_ExtendWidthEnd;


    public Tile Initialize(Extrudable[] extrudables, bool flipFace = false)
    {
        m_Extend = 1;
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
                Vector3 dir = Vector3Extensions.GetDirectionToTarget(m_VerticesData[i][1], m_VerticesData[i][0]);
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
                Vector3 dir = Vector3Extensions.GetDirectionToTarget(m_VerticesData[i][^2], m_VerticesData[i][^1]);
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
            List<Vector3> startEnd = new(2);

            int count = m_VerticesData[0].Count;
            int incriment = count - 1;

            for (int i = 0; i < count; i += incriment)
            {
                Vector3 dir = Vector3Extensions.GetDirectionToTarget(m_VerticesData[1][i], m_VerticesData[0][i]);
                startEnd.Add(m_VerticesData[0][i] + (dir * m_Extend));
            }

            List<Vector3> data = Vector3Extensions.LerpCollection(startEnd[0], startEnd[1], count).ToList();

            m_VerticesData.Insert(0, data);
        }
        if(widthEnd)
        {
            List<Vector3> startEnd = new(2);

            int count = m_VerticesData[0].Count;
            int incriment = count - 1;

            for (int i = 0; i < count; i += incriment)
            {
                Vector3 dir = Vector3Extensions.GetDirectionToTarget(m_VerticesData[^2][i], m_VerticesData[^1][i]);
                startEnd.Add(m_VerticesData[^1][i] + (dir * m_Extend));
            }

            List<Vector3> data = Vector3Extensions.LerpCollection(startEnd[0], startEnd[1], count).ToList();
            

            m_VerticesData.Add(data);
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

        // TODO: Instead of extrude & translate verts, extrude & scale.
 
        outside.Extrude(outside.faces, ExtrudeMethod.FaceNormal, 0.25f); // TODO: The .25 value should be editable in the inspector.

        float tileHeight = 0.2f; // magic // value needs calculating

        //outside.TranslateVertices(externalFaces[0].distinctIndexes, Vector3.up * tileHeight);
        // TODO: Sometimes we need to scale on the x & not the z. How do we work out when to do that?
        outside.ScaleVertices(externalFaces[0].distinctIndexes, TransformPoint.Middle, new Vector3(1, 1 + tileHeight, 1 + tileHeight));

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
