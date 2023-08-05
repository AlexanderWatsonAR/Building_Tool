using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;
using ProMaths = UnityEngine.ProBuilder.Math;

public class etes : MonoBehaviour
{
    //[SerializeField, Range(4, 32)] private int m_Sides = 4;
    ////[SerializeField, Range(0, 1)] private float m_Scale = 0.5f;
    //[SerializeField, Range(0, 0.999f)] private float m_Width = 0.5f;
    //[SerializeField, Range(0, 0.999f)] private float m_Height = 0.5f;
    //[SerializeField, Range(0, 45)] private float m_Angle = 0;
    //[SerializeField, Range(1, 10)] private int m_Columns = 1, m_Rows = 1;

    private void OnDrawGizmosSelected()
    {

    }

    public void DoTest()
    {
        Vector3[] points = new Vector3[] { new Vector3(-0.5f, 0, -0.5f), new Vector3(-0.75f, 1, 0.5f), new Vector3(0.75f, 1, 0.5f), new Vector3(0.5f, 0, -0.5f) };

        Vector3 centre = ProMaths.Average(points);

        //for (int i = 0; i < points.Length; i++)
        //{
        //    // Rotate
        //    Vector3 euler = Vector3.up * 45;
        //    Vector3 v = Quaternion.Euler(euler) * (points[i] - centre) + centre;
        //    points[i] = v;
        //}

        List<IList<Vector3>> holePoints = new ();
        holePoints.Add(points.ToList());

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 point = holePoints[0][i] - centre;
            Vector3 v = Vector3.Scale(point, Vector3.one * 0.5f) + centre;
            holePoints[0][i] = v;
        }

        //float extrude = 0.2f;

        ////ProBuilderMesh mesh = MeshMaker.RoofTileMesh(points, holePoints, extrude);
        //mesh.GetComponent<Renderer>().material = BuiltinMaterials.defaultMaterial;
    }

}
