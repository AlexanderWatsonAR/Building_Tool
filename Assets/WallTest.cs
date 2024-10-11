using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEditor;
using UnityEngine.UIElements;

public class WallTest : MonoBehaviour
{
    Vector3[] m_Polygon = PolygonMaker.U();

    // Start is called before the first frame update
    void Start()
    {
        Vector3[] positions = new Vector3[m_Polygon.Length];

        System.Array.Copy(m_Polygon, positions, m_Polygon.Length);

        Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.forward, Vector3.up));

        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = rotation.MultiplyPoint3x4(positions[i]);
        }

        float height = 1;

        for (int i = 0; i < positions.Length; i++)
        {
            ProBuilderMesh wallMesh = ProBuilderMesh.Create();
            wallMesh.name = "Wall " + i.ToString();

            int next = positions.GetNextControlPoint(i);

            float distance = positions[i].DistanceToTarget(positions[next]);
            distance -= 0.05f;
            Vector3 dir = positions[i].DirectionToTarget(positions[next]);
            Vector3 forward = Vector3.Cross(dir, Vector3.up);

            Vector3 position = positions[i];
            position += dir * (distance * 0.5f);
            position += Vector3.up * (height * 0.5f);

            Quaternion rotation1 = Quaternion.FromToRotation(Vector3.forward, forward);

            Vector3 scale = new Vector3(distance, height);

            WallAData wallData = new WallAData(position, rotation1.eulerAngles, scale);

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

        float height = 1;
        float depth = 1;

        for (int i = 0; i< m_Polygon.Length; i++)
        {
            Square square = new Square();

            Vector3[] controlPoints = square.ControlPoints();


            int next = positions.GetNextControlPoint(i);

            float distance = positions[i].DistanceToTarget(positions[next]);
            Vector3 dir = positions[i].DirectionToTarget(positions[next]);
            Vector3 forward = Vector3.Cross(dir, Vector3.up);

            Vector3 position = positions[i];
            position += dir * (distance * 0.5f);
            position += Vector3.up * (height * 0.5f);

            Quaternion rotation1 = Quaternion.FromToRotation(Vector3.forward, forward);

            Vector3 scale = new Vector3(distance, height);

            Matrix4x4 trs = Matrix4x4.TRS(position, rotation1, scale);

            for(int j = 0; j < controlPoints.Length; j++)
            {
                controlPoints[j] = trs.MultiplyPoint3x4(controlPoints[j]);
            }


            Handles.DrawPolyLine(controlPoints);
            Handles.DrawPolyLine(controlPoints[0], controlPoints[^1]);
        }
    }
}
