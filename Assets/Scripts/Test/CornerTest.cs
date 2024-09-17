using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using OnlyInvalid.ProcGenBuilding.Pillar;
using Unity.VisualScripting;
using OnlyInvalid.ProcGenBuilding.Corner;

public class CornerTest : MonoBehaviour
{
    
    void Start()
    {
        Vector3[] positions = PolygonMaker.L();

        Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.forward, Vector3.up));

        for(int i = 0; i < positions.Length; i++)
        {
            positions[i] = rotation.MultiplyPoint3x4(positions[i]);
        }

        for(int i = 0; i < positions.Length; i++)
        {
            ProBuilderMesh cornerMesh = ProBuilderMesh.Create();
            cornerMesh.name = "Corner " + i.ToString();

            int next = positions.GetNextControlPoint(i);
            Vector3 dir = positions[i].DirectionToTarget(positions[next]) == Vector3.back ? Vector3.forward : positions[i].DirectionToTarget(positions[next]);


            CornerData cornerData = new CornerData(CornerType.Point, 4, positions[i], Vector3.zero, Vector3.one * 0.1f);

            Corner corner = cornerMesh.AddComponent<Corner>();
            corner.Initialize(cornerData);
            corner.Polygon3DData.IsDirty = true;
            corner.Build();

        }

    }


    private void OnDrawGizmos()
    {
        Vector3[] positions = PolygonMaker.L();

        Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.forward, Vector3.up));

        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = rotation.MultiplyPoint3x4(positions[i]);
        }

        foreach (Vector3 position in positions)
        {
            Handles.DrawSolidDisc(position, Vector3.up, 0.01f);
        }
    }
}
