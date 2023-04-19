using UnityEditor;
using UnityEngine;

public class MeshWithHole : MonoBehaviour
{
    Mesh mesh;

    void Start()
    {
        mesh = new Mesh();

        Vector3[] vertices = new Vector3[8];
        vertices[0] = new Vector3(-1f, -1f, 0f);
        vertices[1] = new Vector3(1f, -1f, 0f);
        vertices[2] = new Vector3(1f, 1f, 0f);
        vertices[3] = new Vector3(-1f, 1f, 0f);
        vertices[4] = new Vector3(-0.5f, -0.5f, 0f); // bl
        vertices[5] = new Vector3(0.5f, -0.5f, 0f); // tl
        vertices[6] = new Vector3(0.5f, 0.5f, 0f); // tr
        vertices[7] = new Vector3(-0.5f, 0.5f, 0f); // br

        int[] triangles = new int[24];
        triangles[0] = 0;
        triangles[1] = 4;
        triangles[2] = 7;
        triangles[3] = 7;
        triangles[4] = 3;
        triangles[5] = 0;
        triangles[6] = 1;
        triangles[7] = 5;
        triangles[8] = 4;
        triangles[9] = 4;
        triangles[10] = 0;
        triangles[11] = 1;
        triangles[12] = 2;
        triangles[13] = 6;
        triangles[14] = 5;
        triangles[15] = 5;
        triangles[16] = 1;
        triangles[17] = 2;
        triangles[18] = 6;
        triangles[19] = 2;
        triangles[20] = 3;
        triangles[21] = 3;
        triangles[22] = 7;
        triangles[23] = 6;

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void OnDrawGizmosSelected()
    {
        if(mesh != null)
        {
            for(int i = 0; i < mesh.vertices.Length; i++)
            {
                Handles.Label(mesh.vertices[i] + transform.localPosition, i.ToString());
            }
        }
    }
}