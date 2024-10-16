using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.Rendering;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Roof
{
    public class RoofTile : Buildable
    {
        [SerializeReference] RoofTileData m_RoofTileData;
        [SerializeField, HideInInspector] List<Vector3[]> m_SubPoints;

        private List<Vector3[]> SubPoints
        {
            get
            {
                if (m_RoofTileData.Columns <= 0 && m_RoofTileData.Rows <= 0) return null;

                m_SubPoints = MeshMaker.CreateGridFromControlPoints(m_RoofTileData.ExtendedPoints, m_RoofTileData.Columns, m_RoofTileData.Rows);

                return m_SubPoints;
            }
        }
        public override Buildable Initialize(DirtyData data)
        {
            base.Initialize(data);
            m_RoofTileData = data as RoofTileData;
            name = "Roof Tile";
            return this;
        }
        public override void Build()
        {
            List<Vector3[]> bottomPoints = SubPoints;

            transform.DeleteChildren();

            m_RoofTileData.Sections ??= new RoofSectionData[m_RoofTileData.Columns, m_RoofTileData.Rows];

            Vector3[] extendedPoints = m_RoofTileData.ExtendedPoints;

            Vector3[] projectedVerts = MeshMaker.ProjectedCubeVertices(extendedPoints, m_RoofTileData.Thickness);
            Vector3 midPointA = Vector3.Lerp(extendedPoints[0], extendedPoints[1], 0.5f);
            Vector3 midPointB = Vector3.Lerp(projectedVerts[0], projectedVerts[1], 0.5f);
            float distance = Vector3.Distance(midPointA, midPointB);

            List<Vector3[]> topPoints = MeshMaker.CreateGridFromControlPoints(projectedVerts, m_RoofTileData.Columns, m_RoofTileData.Rows);

            for (int x = 0; x < m_RoofTileData.Columns; x++)
            {
                for (int y = 0; y < m_RoofTileData.Rows; y++)
                {
                    Vector3 bl = bottomPoints[y][x];
                    Vector3 tl = bottomPoints[y + 1][x];
                    Vector3 tr = bottomPoints[y + 1][x + 1];
                    Vector3 br = bottomPoints[y][x + 1];

                    Vector3[] controlPoints = new Vector3[] { bl, tl, tr, br };

                    Vector3 topBL = topPoints[y][x];
                    Vector3 topTL = topPoints[y + 1][x];
                    Vector3 topTR = topPoints[y + 1][x + 1];
                    Vector3 topBR = topPoints[y][x + 1];

                    Vector3[] top = new Vector3[] { topBL, topTL, topTR, topBR };

                    ProBuilderMesh roofSectionMesh = ProBuilderMesh.Create();
                    roofSectionMesh.name = "Roof Section " + y.ToString() + " " + x.ToString();
                    roofSectionMesh.GetComponent<Renderer>().sharedMaterial = m_RoofTileData.Material;

                    roofSectionMesh.transform.SetParent(transform, false);

                    m_RoofTileData.Sections[x, y] ??= new RoofSectionData(m_RoofTileData.SectionData)
                    {
                        ID = new Vector2Int(x, y)
                    };

                    m_RoofTileData.Sections[x, y].ControlPoints = controlPoints;
                    m_RoofTileData.Sections[x, y].TopPoints = top;
                    m_RoofTileData.Sections[x, y].SectionHeight = distance;

                    RoofSection roofSection = roofSectionMesh.AddComponent<RoofSection>().Initialize(m_RoofTileData.Sections[x, y]) as RoofSection;
                    roofSection.Build();
                }
            }

        }

        private void OnDrawGizmosSelected()
        {
            //if (m_Data.ControlPoints == null)
            //    return;

            //if (m_Data.Columns == 0 || m_Data.Rows == 0)
            //    return;

            //List<Vector3[]> subPoints = SubPoints;

            //if (m_Data.Rows != 0 && m_Data.Columns != 0)
            //{
            //    for (int i = 0; i < m_Data.Columns; i++)
            //    {
            //        for (int j = 0; j < m_Data.Rows; j++)
            //        {
            //            Vector3 bl = subPoints[j + 0][i + 0];
            //            Vector3 tl = subPoints[j + 0][i + 1];
            //            Vector3 tr = subPoints[j + 1][i + 1];
            //            Vector3 br = subPoints[j + 1][i + 0];

            //            Vector3 dir = bl.DirectionToTarget(br);
            //            Vector3 cross = Vector3.Cross(bl.DirectionToTarget(tl), dir) * m_Data.Thickness;

            //            bl += cross;
            //            tl += cross;
            //            tr += cross;
            //            br += cross;

            //            bl += transform.position;
            //            tl += transform.position;
            //            tr += transform.position;
            //            br += transform.position;

            //            Handles.DrawAAPolyLine(bl, tl, tr, br);
            //            Handles.DrawAAPolyLine(bl, br);

            //        }
            //    }
            //}
        }
    }
}
