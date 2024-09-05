using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace Assets.Scripts.Test
{
    public class Test : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            NPolygon polygon = new NPolygon(5);

            //Bounds bounds = new Bounds(new Vector3(0, 0), new Vector3(1, 1));
            Bounds bounds = new Bounds(new Vector3(1f/3f, 0), new Vector3(1f / 3f, 1));

            Vector3[] snap = polygon.Snapshot(bounds);

            foreach(Vector3 snapePoint in snap)
            {
                ProBuilderMesh mesh = ShapeGenerator.GenerateIcosahedron(PivotLocation.Center, 0.1f, 1);
                mesh.transform.position = snapePoint;
            }

            

        }

        private void OnDrawGizmos()
        {
            Bounds boundsA = new Bounds(new Vector3(1f / 3f, 0), new Vector3(1f / 3f, 1));
            Bounds boundsB = new Bounds(new Vector3(0, 0), new Vector3(1, 1));
            Gizmos.DrawWireCube(boundsA.center, boundsA.size);

            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(boundsB.center, boundsB.size);
        }
    }
}