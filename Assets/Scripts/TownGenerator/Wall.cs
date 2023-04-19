using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder;

[System.Serializable]
public class Wall : MonoBehaviour
{
    // Idea: Use this class to add in doors, windows and other objects to the wall. 

    [SerializeField] private GameObject m_WallOutlinePrefab;
    [SerializeField] private float m_WallLength;
    
    [SerializeField] private bool m_HasInitialized;
    [SerializeField] private GameObject m_OriginalWallOutlinePrefab;
    [SerializeField] private float m_OriginalWallLength;
    [SerializeField] private Vector3 m_OriginalLocalPosition;
    [SerializeField] private Vector3 m_OriginalLocalEuler;
    [SerializeField] private GameObject m_Wall;
    private TransformCurve m_Curve;

    public GameObject WallOutlinePrefab => m_WallOutlinePrefab;
    public float WallLength => m_WallLength;


    public Wall Initialize(Wall wall)
    {
        return Initialize(wall.WallOutlinePrefab, WallLength);
    }    

    public Wall Initialize(GameObject wallOutlinePrefab, float wallLength)
    {
        if(wallOutlinePrefab == null)
            return null;

        if (Application.isEditor)
        {
            DestroyImmediate(m_Wall);
        }

        m_Wall = Instantiate(wallOutlinePrefab);
        m_WallOutlinePrefab = wallOutlinePrefab;
        m_WallLength = wallLength;
        m_Curve = GetComponent<TransformCurve>();

        if(!m_HasInitialized)
        {
            m_OriginalWallOutlinePrefab = m_WallOutlinePrefab;
            m_OriginalWallLength = m_WallLength;
            m_HasInitialized = true;
        }

        return this;
    }

    public Wall Build()
    {
        if (m_WallLength <= 0)
            return this;

        if (m_Wall.TryGetComponent(out Extrudable extrudable))
            ExtrudeOutline(extrudable);

        m_Wall.transform.SetParent(transform, false);

        if(!m_HasInitialized)
        {
            m_OriginalLocalPosition = m_Wall.transform.localPosition;
            m_OriginalLocalEuler = m_Wall.transform.localEulerAngles;
        }

        for (int i = 0; i < m_Wall.transform.childCount; i++)
        {
            if (m_Wall.transform.GetChild(i).TryGetComponent(out Extrudable wallComponent))
                ExtrudeOutline(wallComponent);
        }
        return this;
    }

    private void ExtrudeOutline(Extrudable extrudable)
    {
        int steps = m_Curve != null ? 0 : 1;

        extrudable.Extrude(ExtrudeMethod.FaceNormal, m_WallLength, steps);

        if (steps == 0)
            extrudable.GetComponent<TransformCurve>().Reshape();

    }

    //private void Reset()
    //{
    //    if (!m_HasInitialized)
    //        return;

    //    m_Wall.transform.localEulerAngles = m_OriginalLocalEuler;
    //    m_Wall.transform.localPosition = m_OriginalLocalPosition;
    //    m_WallOutlinePrefab = m_OriginalWallOutlinePrefab;
    //    m_WallLength = m_OriginalWallLength;
    //    Build();
    //}

    private void InsertOutline()
    {

    }

    //private void ConstructWall()
    //{
    //    if (m_DoorPrefab != null)
    //    {
    //        GameObject aWall = m_Walls[0].transform.GetChild(0).gameObject;

    //        GridLayoutGroup3D gridLayout = aWall.GetComponent<GridLayoutGroup3D>();

    //        gridLayout.size = aWall.GetComponent<Renderer>().localBounds.size;
    //        gridLayout.cellSize = new Vector3(gridLayout.size.x, 1, 1);
    //        gridLayout.center = Vector3.zero;
    //        gridLayout.spacing = Vector3.forward;

    //        //gridLayout.enabled = false;

    //        GameObject aDoor = Instantiate(m_DoorPrefab);
    //        aDoor.transform.SetParent(aWall.transform);
    //        aDoor.transform.localEulerAngles = Vector3.up * 90;
    //        aDoor.GetComponent<LayoutElement3D>().center = new Vector3(0.4f, 1.5f, 0);

    //        gridLayout.UpdateLayout();

    //        List<GameObject> outlineList = new List<GameObject>();
    //        outlineList.Add(aDoor.transform.GetChild(2).gameObject);

    //        for (int i = 0; i < m_Walls[0].transform.childCount; i++)
    //        {
    //            if (m_Walls[0].transform.GetChild(i).GetComponent<ControlPointMesh>() != null)
    //            {
    //                m_Walls[0].transform.GetChild(i).GetComponent<ControlPointMesh>().InsertOutlines(outlineList, 0);
    //            }
    //        }
    //    }
    //}
}
