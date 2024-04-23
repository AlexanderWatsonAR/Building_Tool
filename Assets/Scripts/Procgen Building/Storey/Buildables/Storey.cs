using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Wall;
using OnlyInvalid.ProcGenBuilding.Corner;
using OnlyInvalid.ProcGenBuilding.Pillar;
using OnlyInvalid.ProcGenBuilding.Floor;

namespace OnlyInvalid.ProcGenBuilding.Storey
{
    public class Storey : Buildable
    {
        [SerializeReference] StoreyData m_StoreyData;

        [SerializeField] List<Corner.Corner> m_Corners;
        [SerializeField] List<Wall.Wall> m_Walls;
        [SerializeField] List<Pillar.Pillar> m_Pillars;
        [SerializeField] Floor.Floor m_Floor;

        public override Buildable Initialize(DirtyData data)
        {
            m_StoreyData = data as StoreyData;
            return base.Initialize(data);
        }

        public override void Build()
        {
            Demolish();

            if (m_Data.IsDirty)
            {
                CreateExternalWalls();
                CreateCorners();
                CreatePillars();
                CreateFloor();
            }

            BuildWalls();
            BuildCorners();
            BuildPillars();
            BuildFloor();
        }
        private void CreatePillars()
        {
            if (!m_StoreyData.ActiveElements.IsElementActive(StoreyElement.Pillars))
                return;

            m_Pillars ??= new List<Pillar.Pillar>();

            if (m_StoreyData.Pillars == null || m_StoreyData.Pillars.Length != m_StoreyData.ControlPoints.Length)
            {
                m_StoreyData.Pillars = new PillarData[m_StoreyData.ControlPoints.Length];
            }

            m_Pillars = new List<Pillar.Pillar>();

            GameObject pillars = new GameObject("Pillars");
            pillars.transform.SetParent(transform, false);

            for (int i = 0; i < m_StoreyData.ControlPoints.Length; i++)
            {
                m_StoreyData.Pillars[i] ??= CalculatePillar(i);

                Pillar.Pillar pillar = CreatePillar(m_StoreyData.Pillars[i]);
                pillar.transform.SetParent(pillars.transform, false);
                pillar.transform.localPosition = m_StoreyData.ControlPoints[i].Position;
                int index = m_StoreyData.ControlPoints.GetNext(i);
                pillar.transform.forward = pillar.transform.localPosition.DirectionToTarget(m_StoreyData.ControlPoints[index].Position);

                m_Pillars.Add(pillar);
            }
        }
        private void CreateCorners()
        {
            if (!m_StoreyData.ActiveElements.IsElementActive(StoreyElement.Walls))
                return;

            m_Corners ??= new List<Corner.Corner>();

            GameObject corners = new GameObject("Corners");
            corners.transform.SetParent(transform, false);

            if (m_StoreyData.Corners == null || m_StoreyData.Corners.Length != m_StoreyData.ControlPoints.Length)
            {
                m_StoreyData.Corners = new CornerData[m_StoreyData.ControlPoints.Length];
            }

            for (int i = 0; i < m_StoreyData.ControlPoints.Length; i++)
            {
                m_StoreyData.Corners[i] = CalculateCorner(i);
                Corner.Corner corner = CreateCorner(m_StoreyData.Corners[i]);
                corner.transform.SetParent(corners.transform, false);
                m_Corners.Add(corner);
            }
        }
        private void CreateExternalWalls()
        {
            if (!m_StoreyData.ActiveElements.IsElementActive(StoreyElement.Walls))
                return;

            m_Walls ??= new List<Wall.Wall>();

            if (m_StoreyData.Walls == null || m_StoreyData.Walls.Length != m_StoreyData.ControlPoints.Length)
            {
                m_StoreyData.Walls = new WallData[m_StoreyData.ControlPoints.Length];
            }

            GameObject walls = new GameObject("Walls");
            walls.transform.SetParent(transform, false);

            for (int i = 0; i < m_StoreyData.ControlPoints.Length; i++)
            {
                m_StoreyData.Walls[i] ??= CalculateWall(i);

                Wall.Wall wall = CreateWall(m_StoreyData.Walls[i]);
                wall.transform.SetParent(walls.transform, true);

                m_Walls.Add(wall);
            }
        }
        private Wall.Wall CreateWall(WallData data)
        {
            ProBuilderMesh wallMesh = ProBuilderMesh.Create();
            wallMesh.name = "Wall " + data.ID.ToString();
            Wall.Wall wall = wallMesh.AddComponent<Wall.Wall>();
            wall.Initialize(data);
            wall.Data.IsDirty = true;
            wall.AddDataListener(dirtyData => 
            {
                WallData wallData = dirtyData as WallData;
                m_StoreyData.Walls[wallData.ID] = wallData;
                m_OnDataChanged.Invoke(m_StoreyData);

            });
            return wall;
        }
        private Corner.Corner CreateCorner(CornerData data)
        {
            ProBuilderMesh cornerMesh = ProBuilderMesh.Create();
            cornerMesh.name = "Corner " + data.ID.ToString();
            cornerMesh.GetComponent<Renderer>().sharedMaterial = BuiltinMaterials.defaultMaterial;
            Corner.Corner corner = cornerMesh.AddComponent<Corner.Corner>();
            corner.Initialize(data);
            return corner;
        }
        private Pillar.Pillar CreatePillar(PillarData data)
        {
            int i = data.ID;
            ProBuilderMesh pillarMesh = ProBuilderMesh.Create();
            pillarMesh.name = "Pillar " + data.ID.ToString();
            Pillar.Pillar pillar = pillarMesh.AddComponent<Pillar.Pillar>();
            pillar.Initialize(m_StoreyData.Pillars[i]);
            return pillar;
        }
        private void CreateFloor()
        {
            if (!m_StoreyData.ActiveElements.IsElementActive(StoreyElement.Floor))
                return;

            ProBuilderMesh floorMesh = ProBuilderMesh.Create();

            ControlPoint[] points = new ControlPoint[m_StoreyData.ControlPoints.Length];
            Array.Copy(m_StoreyData.ControlPoints, points, points.Length);

            m_Floor = floorMesh.AddComponent<Floor.Floor>().Initialize(new FloorData() { ControlPoints = points }) as Floor.Floor;

            floorMesh.transform.SetParent(transform, false);
            floorMesh.transform.localPosition = Vector3.zero;

        }

        #region Build
        public void BuildCorners()
        {
            m_Corners.BuildCollection();
        }
        public void BuildPillars()
        {
            m_Pillars.BuildCollection();
        }
        public void BuildWalls()
        {
            m_Walls.BuildCollection();
        }
        public void BuildFloor()
        {
            m_Floor.Build();
        }
        #endregion

        #region Calculate
        /// <summary>
        /// Can we migrate all of this to the Corners class?
        /// </summary>
        /// <param name="cornerIndex"></param>
        /// <returns></returns>
        private CornerData CalculateCorner(int cornerIndex)
        {
            bool isConcave = m_StoreyData.ControlPoints.IsConcave(out int[] concavePoints);
            WallData wallData = m_StoreyData.WallData;

            int current = cornerIndex;
            int previous = m_StoreyData.ControlPoints.GetPrevious(current);
            int next = m_StoreyData.ControlPoints.GetNext(current);


            Vector3 dirA = m_StoreyData.Walls[current].StartPosition.DirectionToTarget(m_StoreyData.Walls[current].EndPosition);
            Vector3 crossA = Vector3.Cross(dirA, Vector3.up) * wallData.Depth;

            Vector3 dirB = m_StoreyData.Walls[previous].StartPosition.DirectionToTarget(m_StoreyData.Walls[previous].EndPosition);
            Vector3 crossB = Vector3.Cross(dirB, Vector3.up) * wallData.Depth;

            Vector3 intersection;

            Extensions.DoLinesIntersect(m_StoreyData.Walls[current].StartPosition + crossA, m_StoreyData.Walls[current].EndPosition + crossA, m_StoreyData.Walls[previous].StartPosition + crossB, m_StoreyData.Walls[previous].EndPosition + crossB, out intersection);

            int numberOfSamples = m_StoreyData.CornerData.Sides + 1;

            Vector3[] cornerPoints = new Vector3[] { m_StoreyData.Walls[current].StartPosition, m_StoreyData.Walls[current].StartPosition + crossA, m_StoreyData.Walls[current].StartPosition + crossB, intersection };
            Vector3[] flatPoints = new Vector3[] { cornerPoints[0], cornerPoints[1], cornerPoints[2] };

            bool isInside = isConcave && concavePoints.Any(b => b == current);

            if (isInside)
            {
                Extensions.DoLinesIntersect(m_StoreyData.Walls[current].StartPosition, m_StoreyData.Walls[current].EndPosition, m_StoreyData.Walls[previous].StartPosition, m_StoreyData.Walls[previous].EndPosition, out intersection);
                cornerPoints = new Vector3[] { m_StoreyData.Walls[current].StartPosition, m_StoreyData.Walls[current].StartPosition + crossA, m_StoreyData.Walls[previous].EndPosition, intersection };
                flatPoints = new Vector3[] { m_StoreyData.Walls[current].StartPosition, m_StoreyData.Walls[current].StartPosition + crossA, m_StoreyData.Walls[previous].EndPosition };
            }

            CornerData cornerData = new CornerData(m_StoreyData.CornerData)
            {
                CornerPoints = cornerPoints,
                FlatPoints = flatPoints.SortPointsClockwise().ToArray(),
                ID = current,
                IsInside = isInside,
                Height = m_StoreyData.WallData.Height
            };

            return cornerData;
        }
        private WallData CalculateWall(int wallIndex)
        {
            WallData wallData = m_StoreyData.WallData;
            wallData.Sections = new WallSectionData[wallData.Columns * wallData.Rows];

            m_StoreyData.WallPoints = new WallPoints[m_StoreyData.ControlPoints.Length];

            int current = wallIndex;
            int next = m_StoreyData.ControlPoints.GetNext(current);

            Vector3 nextDir = m_StoreyData.ControlPoints[current].DirectionToTarget(m_StoreyData.ControlPoints[next]);
            Vector3 wallForward = Vector3.Cross(nextDir, Vector3.up) * wallData.Depth;

            ControlPoint start = new ControlPoint(m_StoreyData.ControlPoints[current]);
            ControlPoint end = new ControlPoint(m_StoreyData.ControlPoints[next]);

            bool isConcave = m_StoreyData.ControlPoints.IsConcave(out int[] concavePoints);

            if (isConcave)
            {
                bool conditionA = concavePoints.Any(a => a == next);
                bool conditionB = concavePoints.Any(b => b == current);

                if (conditionA)
                {
                    end.SetForward(-wallForward);
                }
                if (conditionB)
                {
                    start.SetForward(-wallForward);
                }
            }

            WallData data = new WallData(wallData)
            {
                ID = wallIndex,
                Start = start,
                End = end,
                Normal = wallForward.normalized
            };

            return data;
        }
        private PillarData CalculatePillar(int pillarIndex)
        {
            PillarData data = new PillarData(m_StoreyData.Pillar)
            {
                ID = pillarIndex
            };

            return data;
        }
        #endregion

        public override void Demolish()
        {
            if (!m_Data.IsDirty)
                return;

            transform.DeleteChildren();

            m_Walls?.Clear();
            m_Corners?.Clear();
            m_Pillars?.Clear();
        }
    }
}
