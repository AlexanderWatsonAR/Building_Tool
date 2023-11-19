using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using Unity.VisualScripting;
using UnityEditor;

public class RoofTile : MonoBehaviour, IBuildable
{
    [SerializeField] private RoofTileData m_Data;
    [SerializeField, HideInInspector] private List<Vector3[]> m_SubPoints;
    
    public event Action<IData> OnDataChange;

    public RoofTileData Data => m_Data;

    private List<Vector3[]> SubPoints
    {
        get
        {
            if (m_Data.Columns <= 0 && m_Data.Rows <= 0) return null;

            m_SubPoints = MeshMaker.CreateGridFromControlPoints(m_Data.ExtendedPoints, m_Data.Columns, m_Data.Rows);

            return m_SubPoints;
        }
    }

    public IBuildable Initialize(IData data)
    {
        m_Data = data as RoofTileData;
        name = "Roof Tile";
        return this;
    }

    public void Build()
    {
        List<Vector3[]> bottomPoints = SubPoints;

        Vector3[] extendedPoints = m_Data.ExtendedPoints;

        Vector3[] projectedVerts = MeshMaker.ProjectedCubeVertices(extendedPoints, m_Data.Height);
        Vector3 midPointA = Vector3.Lerp(extendedPoints[0], extendedPoints[1], 0.5f);
        Vector3 midPointB = Vector3.Lerp(projectedVerts[0], projectedVerts[1], 0.5f);
        float distance = Vector3.Distance(midPointA, midPointB);

        List<Vector3[]> topPoints = MeshMaker.CreateGridFromControlPoints(projectedVerts, m_Data.Columns, m_Data.Rows);

        transform.DeleteChildren();

        for (int i = 0; i < m_Data.Columns; i++)
        {
            for (int j = 0; j < m_Data.Rows; j++)
            {
                Vector3 bl = bottomPoints[j][i];
                Vector3 tl = bottomPoints[j + 1][i];
                Vector3 tr = bottomPoints[j + 1][i + 1];
                Vector3 br = bottomPoints[j][i + 1];

                Vector3[] controlPoints = new Vector3[] { bl, tl, tr, br };

                Vector3 topBL = topPoints[j][i];
                Vector3 topTL = topPoints[j + 1][i];
                Vector3 topTR = topPoints[j + 1][i + 1];
                Vector3 topBR = topPoints[j][i + 1];

                Vector3[] points = new Vector3[] { topBL, topTL, topTR, topBR };

                ProBuilderMesh roofSection = ProBuilderMesh.Create();
                roofSection.name = "Roof Section " + j.ToString() + " " + i.ToString();
                roofSection.GetComponent<Renderer>().sharedMaterial = m_Data.Material;

                roofSection.transform.SetParent(transform, false);

                RoofSectionData data = new RoofSectionData()
                {
                    ControlPoints = controlPoints,
                    TopPoints = points,
                    SectionHeight = distance
                };

                roofSection.AddComponent<RoofSection>().Initialize(data).Build();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (m_Data.ControlPoints == null)
            return;

        if (m_Data.Columns == 0 || m_Data.Rows == 0)
            return;

        List<Vector3[]> subPoints = SubPoints;

        if (m_Data.Rows != 0 && m_Data.Columns != 0)
        {
            for (int i = 0; i < m_Data.Columns; i++)
            {
                for (int j = 0; j < m_Data.Rows; j++)
                {
                    Vector3 bl = subPoints[j + 0][i + 0];
                    Vector3 tl = subPoints[j + 0][i + 1];
                    Vector3 tr = subPoints[j + 1][i + 1];
                    Vector3 br = subPoints[j + 1][i + 0];

                    Vector3 dir = bl.DirectionToTarget(br);
                    Vector3 cross = Vector3.Cross(bl.DirectionToTarget(tl), dir) * m_Data.Height;

                    bl += cross;
                    tl += cross;
                    tr += cross;
                    br += cross;

                    bl += transform.position;
                    tl += transform.position;
                    tr += transform.position;
                    br += transform.position;

                    Handles.DrawAAPolyLine(bl, tl, tr, br);
                    Handles.DrawAAPolyLine(bl, br);

                }
            }
        }
    }

}
