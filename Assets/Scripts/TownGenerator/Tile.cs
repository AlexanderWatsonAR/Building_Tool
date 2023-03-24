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

    private Vector3[] corners;
    private int[] cornerIndices;
    public float Scale => m_Scale;
    public float ExtendDistance => m_Extend;
    public float Height => m_Height;
    public bool ExtendHeightBeginning => m_ExtendHeightBeginning;
    public bool ExtendHeightEnd => m_ExtendHeightEnd;
    public bool ExtendWidthBeginning => m_ExtendWidthBeginning;
    public bool ExtendWidthEnd => m_ExtendWidthEnd;

    private Vector3[] CornerPointsExtrudables
    {
        get
        {
            Vector3[] cornerPoints = new Vector3[4];
            cornerPoints[0] = m_Extrudables[0].ExtrusionPositions[0]; // Top Left
            cornerPoints[1] = m_Extrudables[^1].ExtrusionPositions[0]; // Top Right
            cornerPoints[2] = m_Extrudables[^1].ExtrusionPositions[^1]; // Bottom Right
            cornerPoints[3] = m_Extrudables[0].ExtrusionPositions[^1]; // Bottom Left
            return cornerPoints; 
        }
    }

    private Vector3[] CornerPointsVerticesData
    {
        get
        {
            Vector3[] cornerPoints = new Vector3[4];
            cornerPoints[0] = m_VerticesData[0][0]; // Top Left
            cornerPoints[1] = m_VerticesData[^1][0]; // Top Right
            cornerPoints[2] = m_VerticesData[^1][^1]; // Bottom Right
            cornerPoints[3] = m_VerticesData[0][^1]; // Bottom Left
            return cornerPoints;
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
        //m_Extend = 1;
        //m_Height = 0.25f;
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
        return this;
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

    private void DoStuff()
    {
        // Extend Top Points



        Vector3[,] heightVertsData = new Vector3[m_VerticesData.Count, m_VerticesData[0].Count];

        //for(int i = 0; i < corners.Length; i++)
        //{
        //    heightenedVertsData[i][0]
        //}



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

        //Vector3 scale = Vector3.one + (faceNormal * m_Height);
        //Vector3 facePosition = outside.FaceCentre(outside.faces[0]);

        //outside.ScaleVerticesAlt(outside.faces[0].distinctIndexes, facePosition, scale);

        int[] distinctIndices = outside.faces[0].distinctIndexes.ToArray();
        //Array.Sort(distinctIndices);
        //if (!m_FlipFace)
        //{
        //    Array.Sort(distinctIndices);
        //    Array.Reverse(distinctIndices);
        //}
        
        List<List<int>> verticesData = new List<List<int>>(); // store distinct index data.

        for(int i = 0; i < width; i++)
        {
            verticesData.Add(new List<int>(new int[height]));
        }

        count--;
        for (int i = width-1; i > -1; i--)
        {
            for (int j = height-1; j > -1; j--)
            {
                verticesData[i][j] = distinctIndices[count];
                count--;
            }
        }

        Vertex[] points = outside.GetVertices();

        corners = new Vector3[4]; // if ff = false
        corners[0] = points[verticesData[0][0]].position; // tl
        corners[1] = points[verticesData[0][^1]].position; // bl
        corners[2] = points[verticesData[^1][0]].position; // tr
        corners[3] = points[verticesData[^1][^1]].position; //br

        cornerIndices = new int[4];
        cornerIndices[0] = verticesData[0][0]; // bottom left
        cornerIndices[1] = verticesData[0][^1]; // top left
        cornerIndices[2] = verticesData[^1][0]; // bottom right
        cornerIndices[3] = verticesData[^1][^1]; // top right

        for (int i = 0; i < width; i++) // number of columns
        {
            float index = 0;
            float counter = (float)height / (height - 1);

            // first & second should be on the same column but different rows
            int first = verticesData[i][^1];
            int second = verticesData[i][0];

            Vector3 firstPoint = points[first].position;
            Vector3 secondPoint = points[second].position;
            
            Vector3 dir = Vector3Extensions.GetDirectionToTarget(firstPoint, secondPoint);

            for (int j = 0; j < height; j++) // number of rows
            {
                float y = points[verticesData[i][j]].position.y;

                float t = (float) index / (float) height;

                float a = Mathf.Lerp(0, m_Height, t);
                float b = Mathf.Lerp(m_Height, 0, t);

                Vector3 a2 = Vector3.up * a;
                Vector3 b2 = dir * b;

                points[verticesData[i][j]].position += a2;
                points[verticesData[i][j]].position += b2;

                if(a == 0)
                {
                    Vector3 point = points[verticesData[i][j]].position;
                    point = new Vector3(point.x, y, point.z);
                    points[verticesData[i][j]].position = point;
                }

                List<int> shared = outside.GetCoincidentVertices(new int[] { verticesData[i][j] });

                for (int k = 0; k < shared.Count; k++)
                {
                    points[shared[k]].position = points[verticesData[i][j]].position;
                }

                //Debug.Log(points[verticesData[i][j]].position);
                index += counter;
            }
        }

        outside.SetVertices(points);
        outside.ToMesh();
        outside.Refresh();

        {
            //outside.TranslateVertices(new Face[] { outside.faces[0] }, faceNormal * m_Height);

            //Vector3 scale = Vector3.zero;
            //Vector3 facePosition = outside.FaceCentre(outside.faces[0]);

            //Vector3[] corners = CornerPoints;

            //int topLeft, topRight, bottomLeft, bottomRight;

            //Vector3 middle = ProMaths.Average(corners);

            //gizmoPos = middle;
            //gizmoForward = faceNormal;

            //for(int i = 0; i < corners.Length; i++)
            //{
            //    Vector3 vertex = corners[i];
            //    Vector3 v = vertex - middle;
            //    Vector3 projection = Vector3.Dot(v, faceNormal) * faceNormal;

            //    // Project the vertex onto a plane defined by the face normal
            //    Vector3 projectedVertex = v - projection;

            //    // Determine which quadrant the projected vertex is in
            //    if (projectedVertex.x <= middle.x && projectedVertex.z <= middle.z)
            //    {
            //        bottomLeft = i;
            //    }
            //    else if (projectedVertex.x <= middle.x && projectedVertex.z > middle.z)
            //    {
            //        topLeft = i;
            //    }
            //    else if (projectedVertex.x > middle.x && projectedVertex.z <= middle.z)
            //    {
            //        bottomRight = i;
            //    }
            //    else
            //    {
            //        topRight = i;
            //    }
            //}

            //Vector3 direction = Vector3Extensions.GetDirectionToTarget(corners[i], middle);

            //float dot = Vector3.Dot(direction, faceNormal);

            //if (dot > Vector3.Dot(corners[0] - middle, faceNormal))
            //{
            //    topLeft = i;
            //}

            //if (dot > Vector3.Dot(corners[1] - middle, faceNormal))
            //{
            //    topRight = i;
            //}

            //if (dot < Vector3.Dot(corners[2] - middle, faceNormal))
            //{
            //    bottomLeft = i;
            //}

            //if (dot < Vector3.Dot(corners[3] - middle, faceNormal))
            //{
            //    bottomRight = i;
            //}

            //Vector3[] orderedCorners = new Vector3[corners.Length];
            //orderedCorners[0] = corners[bottomLeft];
            //orderedCorners[1] = corners[topLeft];
            //orderedCorners[2] = corners[topRight];
            //orderedCorners[3] = corners[bottomRight];
        }

        //Vector3[] corners = CornerPointsVerticesData;
        //int topLeft = 0;
        //int bottomLeft = 3;
        //int bottomRight = 2;
        //int topRight = 1;

        //Vector3[] extension = new Vector3[4];

        //Vector3 dirA = Vector3Extensions.GetDirectionToTarget(corners[topLeft], corners[bottomLeft]);
        //Vector3 dirB = Vector3Extensions.GetDirectionToTarget(corners[topRight], corners[bottomRight]);

        //extension[bottomLeft] = corners[bottomLeft] + (dirA * m_Height);
        //extension[bottomRight] = corners[bottomRight] + (dirB * m_Height);

        //extension[topLeft] = corners[topLeft] + (Vector3.up * m_Height);
        //extension[topRight] = corners[topRight] + (Vector3.up * m_Height);

        //extension[bottomLeft] = new Vector3(extension[bottomLeft].x, corners[bottomLeft].y, extension[bottomLeft].z);
        //extension[bottomRight] = new Vector3(extension[bottomRight].x, corners[bottomRight].y, extension[bottomRight].z);


        //ProBuilderMesh test = ProBuilderMesh.Create();
        //test.CreateShapeFromPolygon(extension, 0.01f, false);
        //test.GetComponent<Renderer>().sharedMaterial = m_Material;
        //test.transform.SetParent(transform, true);

        //GameObject point = new GameObject();
        //point.transform.SetParent(transform, true);
        //point.transform.localPosition = facePosition;
        //point.transform.forward = faceNormal;

        //Vector3 absUp = new Vector3(MathF.Abs(point.transform.up.x), MathF.Abs(point.transform.up.y), MathF.Abs(point.transform.up.z));

        ////Vector3 worldNormal = transform.TransformVector(faceNormal);
        //Vector3 scale = (Vector3.one + (absUp * m_Height));

        //float scaleFactor = m_Height + 1.0f;

        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    Vector3 vertex = vertices[i];
        //    float distanceToFace = Vector3.Dot(vertex - facePosition, faceNormal);
        //    float scale = scaleFactor + distanceToFace / m_Scale;
        //    vertices[i] = facePosition + (vertex - facePosition) * scale;
        //}

        //outside.ScaleFace(outside.faces[0], scaleFactor);

        //outside.ScaleVertices(new Face[] { externalFaces[0] }, facePosition, scale);

        //outside.SetPivot(originalPivot);
        outside.ToMesh();
        outside.Refresh();

        inside.transform.SetParent(outside.transform, true);

        m_External = outside;
        m_Internal = inside;
    }

    private void OnDrawGizmos()
    {
        //if (gizmoPos == Vector3.zero && gizmoForward == Vector3.zero)
        //    return;

        //Handles.DoPositionHandle(gizmoPos, Quaternion.LookRotation(gizmoForward));

        if (corners == null)
            return;

        for(int i = 0; i < corners.Length; i++)
        {
            Handles.Label(corners[i], cornerIndices[i].ToString());
        }

        return;

        if(m_External == null || m_Internal == null)
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

        Vector3 centreOutside = ProBuilderExtensions.FaceCentre(m_External, m_External.faces[0]);
        Vector3 centreInside = ProBuilderExtensions.FaceCentre(m_Internal, m_Internal.faces[0]);

        Vector3 dir = Vector3Extensions.GetDirectionToTarget(centreInside, centreOutside);
        //m_External.ClearSelection();
        //m_External.SetSelectedFaces(new Face[] { m_External.faces[0] });
        //Vertex[] verts = m_External.GetVertices(m_External.selectedVertices);

        //Vector3 vertsCentre = ProMaths.Average(verts.Positions());
        //Vector3 vertsNormal = ProMaths.Average(verts.Normals());

        Vector3 a = Handles.DoPositionHandle(centreOutside, Quaternion.LookRotation(dir));
        //Handles.DoPositionHandle(vertsCentre, Quaternion.LookRotation(vertsNormal));
        //Handles.DrawSolidDisc(centre, normal, .5f);


    }
}
