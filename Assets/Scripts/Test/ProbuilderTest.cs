using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEditor.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class ProbuilderTest : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {

        Vector3[] controlPoints = new Vector3[]
        {
            new Vector3 (-9.5f, -9.5f),
            new Vector3 (-9.5f, 9.5f),
            new Vector3 (9.5f, 9.5f),
            new Vector3 (9.5f, -9.5f)
        };

        IList<IList<Vector3>> holePoints = new List<IList<Vector3>>();

        holePoints.Add(new List<Vector3>() 
        {
            new Vector3 (-2.5f, -2.5f),
            new Vector3 (-2.5f, 2.5f),
            new Vector3 (2.5f, 2.5f),
            new Vector3 (2.5f, -2.5f)
        });

        //holePoints.Add(new List<Vector3>()
        //{
        //    new Vector3 (-1.5f, -1.5f),
        //    new Vector3 (-1.5f, 2.5f),
        //    new Vector3 (4.5f, 2.5f),
        //    new Vector3 (4.5f, -1.5f)
        //});


        ProBuilderMesh mesh = ProBuilderMesh.Create();
        mesh.CreateShapeFromPolygon(controlPoints, 0.1f, false, holePoints);
        mesh.ToMesh();
        mesh.Refresh();

    }
}
