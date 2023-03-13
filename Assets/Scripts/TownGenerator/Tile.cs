using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder;
using ProMaths = UnityEngine.ProBuilder.Math;
using System.Linq;
using Unity.VisualScripting;
using System.ComponentModel;
using UnityEngine.UI;
using UnityEditor;

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

    public float ExtendDistance => m_Extend;
    public float Height => m_Height;
    public bool ExtendHeightBeginning => m_ExtendHeightBeginning;
    public bool ExtendHeightEnd => m_ExtendHeightEnd;
    public bool ExtendWidthBeginning => m_ExtendWidthBeginning;
    public bool ExtendWidthEnd => m_ExtendWidthEnd;


    public Tile Initialize(Extrudable[] extrudables, bool flipFace = false)
    {
        m_Extend = 1;
        m_Height = 0.25f;
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

        ProBuilderMesh inside = ProBuilderMesh.Create(vertices, new Face[] { new Face(triangles.Reverse()) });
        
        inside.GetComponent<MeshRenderer>().sharedMaterial = m_Material;

        inside.ToMesh();
        inside.Refresh();

        ProBuilderMesh outside = ProBuilderMesh.Create(vertices, externalFaces);

        outside.transform.SetParent(transform, true);
        outside.GetComponent<MeshRenderer>().sharedMaterial = m_Material;
        //Vector3 faceNormal = ProBuilderExtensions.FaceNormal(outside, outside.faces[0]);

        outside.Extrude(outside.faces, ExtrudeMethod.FaceNormal, m_Height);
        //outside.TranslateVertices(outside.faces[0].distinctIndexes, faceNormal * m_Height);

        Vector3 scale = Vector3.zero;
        Vector3 centre = ProBuilderExtensions.FaceCentre(outside, externalFaces[0]);

        for (int i = 0; i < externalFaces[0].distinctIndexes.Count; i++)
        {
            if (!m_FlipFace)
            {
                Vector3 normal = outside.normals[externalFaces[0].distinctIndexes[i]];
                normal = Quaternion.Euler(0, 180, 0) * normal;
                scale = Vector3.one + (normal * m_Height);
            }
            else
            {
                scale = Vector3.one + (outside.normals[i] * m_Height);
            }

            outside.ScaleVertices(new int[] { externalFaces[0].distinctIndexes[i] }, centre, scale);
        }

        outside.ToMesh();
        outside.Refresh();

        inside.transform.SetParent(outside.transform, true);

        m_External = outside;
        m_Internal = inside;
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

    private void OnDrawGizmos()
    {
        if(m_External == null)
        {
            return;
        }

        Vector3[] externalWorldVerts = m_External.VerticesInWorldSpace();

        List<Vector3> points = new List<Vector3>();
        int count = m_External.faces[0].distinctIndexes.Count;

        for (int i = 0; i < count; i++)
        {
            points.Add(externalWorldVerts[m_External.faces[0].distinctIndexes[i]]);
        }

        if (points == null)
            return;

        if (m_Internal.normals == null)
            return;

        Vector3 centre = ProMaths.Average(points);
        Vector3 normal = ProMaths.Average(m_Internal.normals);

        Handles.DoPositionHandle(centre, Quaternion.LookRotation(-normal));
        //Handles.DrawSolidDisc(centre, normal, .5f);


    }
}
