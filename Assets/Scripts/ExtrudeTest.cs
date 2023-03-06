using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEditor.ProBuilder;
using UnityEngine.ProBuilder.Shapes;

public class ExtrudeTest : MonoBehaviour
{
    public List<GameObject> objects;
    public GameObject Prefab;
    public AnimationCurve Curve;

    // Start is called before the first frame update
    void Start()
    {
        GameObject prefabPrefab = Instantiate(Prefab);
        //new MeshImporter(prefabPrefab).Import();
        ProBuilderMesh a = prefabPrefab.GetComponent<ProBuilderMesh>();
        a.transform.position = new Vector3(200, 10, 0);
        a.transform.eulerAngles = new Vector3(90, 10, 120);

        //Extensions.nameTBD(a, TransformPoint.Middle, a.faces[0].distinctIndexes);
    }

}
