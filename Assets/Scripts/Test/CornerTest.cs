using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using OnlyInvalid.ProcGenBuilding.Pillar;
using Unity.VisualScripting;
using OnlyInvalid.ProcGenBuilding.Corner;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.InputSystem.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class CornerTest : MonoBehaviour
{
    Vector3[] m_Polygon = PolygonMaker.U();
    
    void Start()
    {
        Vector3[] positions = new Vector3[m_Polygon.Length];

        System.Array.Copy(m_Polygon, positions, m_Polygon.Length);

        Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.forward, Vector3.up));
        Matrix4x4 scale = Matrix4x4.Scale(Vector3.one * 3);

        Matrix4x4 rs = rotation * scale;

        for(int i = 0; i < positions.Length; i++)
        {
            positions[i] = rs.MultiplyPoint3x4(positions[i]);
        }

        int[] concavePoints = PolygonRecognition.GetConcaveIndexPoints(positions);

        List<Corner> corners = new List<Corner>();

        for (int i = 0; i < positions.Length; i++)
        {
            ProBuilderMesh cornerMesh = ProBuilderMesh.Create();
            cornerMesh.name = "Corner " + i.ToString();

            int next = positions.GetNextControlPoint(i);
            int previous = positions.GetPreviousControlPoint(i);

            Vector3 dirA = positions[i].DirectionToTarget(positions[previous]);
            Vector3 dirB = positions[i].DirectionToTarget(positions[next]);

            Vector3 forward = Vector3.Lerp(dirA, dirB, 0.5f).normalized;

            float theta = Vector3.Angle(dirA, dirB);

            bool isConvexPoint = concavePoints.Any(x => x == i);

            Vector3 to = concavePoints.Any(x => x == i) ? -forward : forward;

            CornerData cornerData = new CornerData(CornerType.Point, theta, 5, to, positions[i], Vector3.zero, Vector3.one * 0.05f);

            Corner corner = cornerMesh.AddComponent<Corner>();
            corner.Initialize(cornerData);
            corner.Polygon3DData.IsDirty = true;
            corner.Build();
            corners.Add(corner);
        }

        for(int i = 0; i < corners.Count; i++)
        {
            ProBuilderMesh wallMesh = ProBuilderMesh.Create();
            wallMesh.name = "Wall " + i.ToString();

            int next = positions.GetNextControlPoint(i);

            WallAData wallData = new WallAData(corners[i].CornerData, corners[next].CornerData);

            WallA wall = wallMesh.AddComponent<WallA>();
            wall.Initialize(wallData);
            wall.WallAData.IsDirty = true;
            wall.Build();
            
        }

    }


    private void OnDrawGizmos()
    {
        Vector3[] positions = new Vector3[m_Polygon.Length];

        System.Array.Copy(m_Polygon, positions, m_Polygon.Length);


        Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.forward, Vector3.up));

        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = rotation.MultiplyPoint3x4(positions[i]);


        }

        //for(int i = 0; i < positions.Length; i++)
        //{
        //    int next = positions.GetNextControlPoint(i);
        //    int previous = positions.GetPreviousControlPoint(i);

        //    Vector3 dirA = positions[i].DirectionToTarget(positions[previous]);
        //    Vector3 dirB = positions[i].DirectionToTarget(positions[next]);

        //    Vector3 forward = Vector3.Lerp(dirA, dirB, 0.5f);

        //    float theta = Vector3.Angle(dirA, dirB);

        //    Vector3 from = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (theta * 0.5f)), 0, Mathf.Sin(Mathf.Deg2Rad * (theta * 0.5f)));
        //    Handles.DrawLine(positions[i], positions[i] + forward);
        //}

        //for (int i = 0; i < positions.Length; i++)
        //{
        //    Handles.DrawSolidDisc(positions[i], Vector3.up, 0.01f);
        //}

        //Color colour = Color.blue;
        //Handles.color = colour;

        //foreach (Vector3 position in positions)
        //{
        //    Handles.DrawSolidDisc(position, Vector3.up, 0.01f);
        //    colour += new Color(0.1f, 0.1f, 0.1f);
        //    Handles.color = colour;
        //}


        Handles.DrawAAPolyLine(positions);
        Handles.DrawAAPolyLine(positions[0], positions[^1]);
        Handles.color = Color.white;




        return;

        float depth = 0.1f;

        int[] concaveIndices = new int[] { 3, 4 };

        for (int i = 0; i < positions.Length; i++)
        {
            int next = positions.GetNextControlPoint(i);
            int previous = positions.GetPreviousControlPoint(i);

            Vector3 dirA = positions[i].DirectionToTarget(positions[previous]);
            Vector3 dirB = positions[i].DirectionToTarget(positions[next]);

            float theta = Vector3.Angle(dirA, dirB);

            //float hTheta = theta * 0.5f;

            Vector3 line1Dir = Vector3.right;
            Vector3 line2Dir = new Vector3(Mathf.Cos(Mathf.Deg2Rad * theta), Mathf.Sin(Mathf.Deg2Rad * theta), 0);

            Vector3 line1Cross = Vector3.Cross(Vector3.forward, line1Dir);
            Vector3 line2Cross = Vector3.Cross(Vector3.back, line2Dir);

            Vector3 line1Start = line1Dir;
            Vector3 line1End = line1Dir + line1Cross;

            Vector3 line2Start = line2Dir;
            Vector3 line2End = line2Dir + line2Cross;

            Vector3 intersection = Vector3.zero;

            if(theta > 90) 
            {
                if(Extensions.DoLinesIntersectXY(line1End, line1End + -line1Dir, line2End, line2End + -line2Dir, out intersection))
                {
                    float dis = Vector3.Distance(line1End, intersection);

                    line1Start -= (line1Dir * dis);
                    line2Start -= (line2Dir * dis);
                }
            }
            else if(theta < 90)
            {
                if (Extensions.DoLinesIntersectXY(line1End, line1End + line1Dir, line2End, line2End + line2Dir, out intersection))
                {
                    float dis = Vector3.Distance(line1End, intersection);

                    line1Start += (line1Dir * dis);
                    line2Start += (line2Dir * dis);
                }
            }
            else
            {
                if (Extensions.DoLinesIntersectXY(line1Start, line1End, line2Start, line2End, out intersection))
                {
                }
            }

            Vector3[] controlPoints = new Vector3[]
            {
                Vector3.zero,
                line1Start,
                intersection,
                line2Start,
            };

            Vector3 from = positions[i].DirectionToTarget(positions[i] + intersection);

            bool isConvexPoint = concaveIndices.Any(x => x == i);

            Vector3 to = isConvexPoint ? -Vector3.Lerp(dirA, dirB, 0.5f) : Vector3.Lerp(dirA, dirB, 0.5f);

            Quaternion rotation1 = Quaternion.FromToRotation(from, to);

            Matrix4x4 trs = Matrix4x4.TRS(positions[i], rotation1, Vector3.one * 0.1f);

            for (int j = 0; j < controlPoints.Length; j++)
            {
                controlPoints[j] = trs.MultiplyPoint3x4(controlPoints[j]);
            }

            if (i == 0)
                Handles.color = Color.red;
            else if (i == 1)
                Handles.color = Color.blue;
            else
                Handles.color = Color.white;


            Handles.DrawAAConvexPolygon(controlPoints);

            //Handles.color = Color.yellow;

            //Handles.DrawLine(Vector3.zero, line1Dir);
            //Handles.DrawLine(Vector3.zero, line2Dir);

            //Handles.color = Color.blue;

            //Handles.DrawLine(line1Dir, line1Dir + line1Cross);
            //Handles.DrawLine(line2Dir, line2Dir + line2Cross);

            Vector3 a = Vector3.Lerp(line1End, line2End, 0.5f);

           // Handles.DrawSolidDisc(a, Vector3.forward, 0.01f);

            //Handles.color = Color.red;

            //Handles.DrawLine(Vector3.zero, intersection);

           // Debug.Log(Vector3.Distance(line1End, a));

           // Handles.DrawLine(new Vector3(line1End.x, line2End.y), a - Vector3.up);

            //Handles.DrawLine((line1Start * depth) + positions[i], (line1Start * depth) + (a * depth * 2) + positions[i], 0.2f);
            //Handles.DrawLine((line2Start * depth) + positions[i], (line2Start * depth) + (b * depth * 2) + positions[i], 0.2f);
        }

    }
}
