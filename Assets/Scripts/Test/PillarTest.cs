using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using OnlyInvalid.ProcGenBuilding.Pillar;
using Unity.VisualScripting;

public class PillarTest : MonoBehaviour
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
            ProBuilderMesh pillarMesh = ProBuilderMesh.Create();
            pillarMesh.name = "Pillar " + i.ToString();

            int next = positions.GetNextControlPoint(i);
            Vector3 dir = positions[i].DirectionToTarget(positions[next]) == Vector3.back ? Vector3.forward : positions[i].DirectionToTarget(positions[next]);

            Vector3 eulerAngle = Quaternion.FromToRotation(Vector3.forward, dir).eulerAngles;

            PillarData pillarData = new PillarData(positions[i], eulerAngle, Vector3.one * 0.1f, 4);

            Pillar pillar = pillarMesh.AddComponent<Pillar>();
            pillar.Initialize(pillarData);
            pillar.Polygon3DData.IsDirty = true;
            pillar.Build();

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
