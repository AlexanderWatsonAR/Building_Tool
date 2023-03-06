using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder.Shapes;
using AutoLayout3D;
using Unity.VisualScripting;


public class Testing : MonoBehaviour
{
    public GameObject Window;
    public GameObject WoodenPillar;
    public GameObject WallOutline;
    public GameObject WallInteriorOutline;
    public GameObject Door;
    public GameObject DoorInterior;
    public GameObject Bricksx3;
    public GameObject Bricksx2;

    public GameObject Terrain;

    private List<Transform> m_buildingComponents;
    private List<Transform> m_pillars;

    // Start is called before the first frame update
    void Start()
    {
        //BuildDistrict();

        //LoadDustbrook();

        //TriangulateTerrain();

        //PolyShape[] polyShapes = FindObjectsOfType<PolyShape>();

        //foreach(PolyShape polyShape in polyShapes)
        //{
        //    if(polyShape.controlPoints.Count == 6)
        //    {
        //        polyShape.GetComponent<Renderer>().material.color = Color.red;
        //    }
        //}    

    }

    //private void BuildDistrict()
    //{
    //    for(int i = 0; i < transform.childCount; i++)
    //    {
    //        if(transform.GetChild(i).GetComponent<PolyShape>() != null)
    //        {
    //            Building house;
    //            if (transform.GetChild(i).GetComponent<Building>() == null)
    //            {
    //                house = transform.GetChild(i).AddComponent<Building>();
    //                house.SetBuildingMaterials(WoodenPillar, WallOutline);
    //                house.Construct();

    //            }
    //            else
    //            {
    //                house = transform.GetChild(i).GetComponent<Building>();
    //                house.SetBuildingMaterials(WoodenPillar, WallOutline); // To be Removed
    //                house.Construct();
    //            }
    //        }
    //    }
    //}

    //private void TriangulateTerrain()
    //{
    //    Terrain.GetComponent<Terrain>().terrainData.

    //    new MeshImporter(Terrain).Import();

    //    ProBuilderMesh proBuilderMesh = Terrain.GetComponent<ProBuilderMesh>();

    //    proBuilderMesh.ToTriangles(proBuilderMesh.faces);

    //    proBuilderMesh.ToMesh();
    //    proBuilderMesh.Refresh();
    //}

    private void LoadDustbrook()
    {
        Transform duskbrook = GameObject.Find("Duskbrook").transform;

        int childCount = duskbrook.childCount;

        float pillarHeight = WoodenPillar.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;

        m_buildingComponents = new List<Transform>();
        m_pillars = new List<Transform>();

        for (int i = 0; i < childCount; i++) // Loops through each polyshape.
        {
            PolyShape polyShape = duskbrook.GetChild(i).GetComponent<PolyShape>();

            polyShape.gameObject.GetComponent<Renderer>().enabled = false;

            Vector3[] controlPoints = polyShape.controlPoints.ToList().ToArray();

            CreateGenericBuilding(polyShape, IsPolygonClockwise(polyShape), WoodenPillar, WallOutline, duskbrook);

            //int look = 1;
            //for (int j = 0; j < controlPoints.Length; j++)
            //{
            //    Vector3 currentPoint = controlPoints[j];

            //    if (j == controlPoints.Length - 1)
            //        look = (controlPoints.Length - 1) * -1;

            //    GameObject tudorPillar = Instantiate(WoodenPillar);
            //    tudorPillar.transform.SetParent(duskbrook, true);
            //    tudorPillar.transform.localPosition = currentPoint + polyShape.transform.localPosition + new Vector3(0, pillarHeight, 0);

            //    m_buildingComponents.Add(tudorPillar.transform);
            //    m_pillars.Add(tudorPillar.transform);

            //    if (!isClockwise)
            //    {
            //        tudorPillar.transform.Rotate(Vector3.up * 180);
            //    }

            //    GameObject nextPoint = new GameObject();
            //    nextPoint.transform.SetParent(duskbrook, true);
            //    nextPoint.transform.localPosition = controlPoints[j + look] + polyShape.transform.localPosition;

            //    tudorPillar.transform.LookAt(nextPoint.transform.position);
            //    tudorPillar.transform.localEulerAngles = new Vector3(0, tudorPillar.transform.localEulerAngles.y, tudorPillar.transform.localEulerAngles.z);

            //    GameObject tudorWall = Instantiate(WallOutline);
            //    tudorWall.transform.SetParent(tudorPillar.transform, false);

            //    if (!isClockwise)
            //    {
            //        tudorWall.transform.Rotate(Vector3.up * 180);
            //    }

            //    float distance = Vector3.Distance(tudorWall.transform.position, nextPoint.transform.position);


            //    for (int k = 0; k < tudorWall.transform.childCount; k++)
            //    {
            //        GameObject wall = tudorWall.transform.GetChild(k).gameObject;

            //        new MeshImporter(wall).Import();

            //        ProBuilderMesh proBuilderMesh = wall.GetComponent<ProBuilderMesh>();

            //        proBuilderMesh.Extrude(proBuilderMesh.faces, ExtrudeMethod.FaceNormal, distance);

            //        foreach (Face f in proBuilderMesh.faces)
            //        {
            //            f.uv.Reset();
            //            f.manualUV = true;
            //        }

            //        proBuilderMesh.ToMesh();
            //        proBuilderMesh.Refresh();
            //    }

            //    if (!isClockwise)
            //    {
            //        tudorWall.transform.localPosition = new Vector3(0, 0, distance);
            //    }

            //    GameObject wallSkirting = tudorWall.transform.GetChild(0).gameObject;

            //    ZAxisLayoutGroup3D layoutGroup3D = wallSkirting.GetComponent<ZAxisLayoutGroup3D>();

            //    layoutGroup3D.center = new Vector3(layoutGroup3D.center.x, layoutGroup3D.center.y, distance / 2);
            //    layoutGroup3D.size = new Vector3(1, 1, distance);

            //    int numberOfBricks = Mathf.FloorToInt(distance);

            //    while (numberOfBricks != 0)
            //    {
            //        if (numberOfBricks % 2 == 0)
            //        {
            //            GameObject bricks2 = Instantiate(Bricksx2);
            //            bricks2.transform.SetParent(wallSkirting.transform, false);
            //        }
            //        else
            //        {
            //            GameObject bricks3 = Instantiate(Bricksx3);
            //            bricks3.transform.SetParent(wallSkirting.transform, false);
            //        }
            //        numberOfBricks--;
            //    }


            //    // Insert Windows

            //    GameObject aWall = tudorWall.transform.GetChild(1).gameObject;
            //    Vector3 center = aWall.GetComponent<Renderer>().bounds.center;

            //    //GameObject point = new GameObject("Center Point");
            //    //point.transform.position = center;

            //    aWall.GetComponent<ProBuilderMesh>().SetPivot(center);

            //    //Bounds localBounds = aWall.GetComponent<Renderer>().localBounds;
            //    //localBounds.center = Vector3.zero;

            //    //aWall.GetComponent<Renderer>().localBounds = localBounds;

            //    if (controlPoints.Length == 4 && j % 2 == 1)
            //    {
            //        Destroy(nextPoint);
            //        continue;
            //    }


            //    int numberOfWindows = Mathf.FloorToInt(distance / 2);

            //    GridLayoutGroup3D gridLayout = aWall.GetComponent<GridLayoutGroup3D>();

            //    gridLayout.size = aWall.GetComponent<Renderer>().localBounds.size;
            //    gridLayout.cellSize = new Vector3(gridLayout.size.x, 1, 1);
            //    gridLayout.center = Vector3.zero;
            //    gridLayout.spacing = Vector3.forward;

            //    if (numberOfWindows >= 10)
            //    {
            //        numberOfWindows /= 2;
            //        gridLayout.spacing = Vector3.forward * 2;
            //    }

            //    gridLayout.constraintZ.constraintCount = numberOfWindows;

            //    List<GameObject> windowsOutlineList = new List<GameObject>();

            //    if (j == 0)
            //    {
            //        numberOfWindows--;
            //        GameObject anotherDoor = Instantiate(Door);
            //        if (isClockwise)
            //        {
            //            anotherDoor.transform.eulerAngles = tudorPillar.transform.eulerAngles + (Vector3.up * 90);
            //        }
            //        else
            //        {
            //            anotherDoor.transform.eulerAngles = tudorPillar.transform.eulerAngles - (Vector3.up * 90);
            //        }
            //        anotherDoor.transform.SetParent(aWall.transform);
            //        anotherDoor.GetComponent<LayoutElement3D>().center = new Vector3(0.4f, 1.15f, 0);

            //        windowsOutlineList.Add(anotherDoor.transform.GetChild(2).gameObject);
            //    }

            //    while (numberOfWindows != 0)
            //    {
            //        GameObject anotherWindow = Instantiate(Window);

            //        if (isClockwise)
            //        {
            //            anotherWindow.transform.eulerAngles = tudorPillar.transform.eulerAngles + (Vector3.up * 90);
            //        }
            //        else
            //        {
            //            anotherWindow.transform.eulerAngles = tudorPillar.transform.eulerAngles - (Vector3.up * 90);
            //        }

            //        anotherWindow.transform.SetParent(aWall.transform);
            //        windowsOutlineList.Add(anotherWindow.transform.GetChild(3).gameObject);
            //        numberOfWindows--;
            //    }

            //    gridLayout.UpdateLayout();

            //    float randomBevel = Random.Range(0.01f, 0.05f);

            //    aWall.GetComponent<ControlPointMesh>().InsertOutlines(windowsOutlineList, randomBevel);

            //    aWall.AddComponent<MeshCollider>();

            //    gridLayout.center = Vector3.right * randomBevel * 2;
            //    gridLayout.UpdateLayout();

            //    Destroy(nextPoint);
            //}

            //CreateFloor();
            //CreateGenericInterior(polyShape);

            //foreach (Transform t in m_buildingComponents)
            //{
            //    t.SetParent(polyShape.transform, true);
            //}

            //m_buildingComponents.Clear();
            //m_pillars.Clear();

        }
    }

    // Create pillars at polyshape control points.
    // Create walls between pillars.
    private void CreateGenericBuilding(PolyShape buildingOutline, bool isClockwise, GameObject pillarPrefab, GameObject wallOutlinePrefab, Transform region)
    {
        List<GameObject> outerWalls = new List<GameObject>();

        float pillarHeight = pillarPrefab.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;

        buildingOutline.gameObject.GetComponent<Renderer>().enabled = false;

        Vector3[] controlPoints = buildingOutline.controlPoints.ToList().ToArray();

        int look = 1;

        for (int i = 0; i < buildingOutline.controlPoints.Count; i++)
        {
            Vector3 currentPoint = controlPoints[i];

            if (i == controlPoints.Length - 1)
                look = (controlPoints.Length - 1) * -1;

            GameObject pillar = Instantiate(pillarPrefab);
            pillar.transform.SetParent(region, true);
            pillar.transform.localPosition = currentPoint + buildingOutline.transform.localPosition + new Vector3(0, pillarHeight, 0);

            m_buildingComponents.Add(pillar.transform);
            m_pillars.Add(pillar.transform);

            if (!isClockwise)
            {
                pillar.transform.Rotate(Vector3.up * 180);
            }

            GameObject nextPoint = new GameObject();
            nextPoint.transform.SetParent(region, true);
            nextPoint.transform.localPosition = controlPoints[i + look] + buildingOutline.transform.localPosition;

            pillar.transform.LookAt(nextPoint.transform.position);
            pillar.transform.localEulerAngles = new Vector3(0, pillar.transform.localEulerAngles.y, pillar.transform.localEulerAngles.z);

            GameObject wall = Instantiate(WallOutline);
            wall.transform.SetParent(pillar.transform, false);

            if (!isClockwise)
            {
                wall.transform.Rotate(Vector3.up * 180);
            }

            float distance = Vector3.Distance(wall.transform.position, nextPoint.transform.position);

            for (int j = 0; j < wall.transform.childCount; j++)
            {
                GameObject subWall = wall.transform.GetChild(j).gameObject;

                new MeshImporter(subWall).Import();

                ProBuilderMesh proBuilderMesh = subWall.GetComponent<ProBuilderMesh>();

                proBuilderMesh.Extrude(proBuilderMesh.faces, ExtrudeMethod.FaceNormal, distance);

                foreach (Face f in proBuilderMesh.faces)
                {
                    f.uv.Reset();
                    f.manualUV = true;
                }

                proBuilderMesh.ToMesh();
                proBuilderMesh.Refresh();
            }

            if (!isClockwise)
            {
                wall.transform.localPosition = new Vector3(0, 0, distance);
            }
            outerWalls.Add(wall);
            Destroy(nextPoint);
        }

        //CreateGenericExterior(buildingOutline, outerWalls.ToArray());
    }

    //Add external elements to wall
    //E.g.Windows, door,
    //Make it fit with internal walls
    //private void CreateGenericExterior(PolyShape buildingOutline, GameObject[] outerWalls)
    //{
    //    Vector3[] controlPoints = buildingOutline.controlPoints.ToArray();

    //    for (int i = 0; i < outerWalls.Length; i++)
    //    {
    //        //Insert Windows
    //        GameObject wall = outerWalls[i].transform.GetChild(1).gameObject;
    //        Vector3 center = wall.GetComponent<Renderer>().bounds.center;

    //        wall.GetComponent<ProBuilderMesh>().SetPivot(center);

    //        if (controlPoints.Length == 4 && j % 2 == 1)
    //        {
    //            continue;
    //        }

    //        int numberOfWindows = Mathf.FloorToInt(distance / 2);

    //        GridLayoutGroup3D gridLayout = aWall.GetComponent<GridLayoutGroup3D>();

    //        gridLayout.size = aWall.GetComponent<Renderer>().localBounds.size;
    //        gridLayout.cellSize = new Vector3(gridLayout.size.x, 1, 1);
    //        gridLayout.center = Vector3.zero;
    //        gridLayout.spacing = Vector3.forward;

    //        if (numberOfWindows >= 10)
    //        {
    //            numberOfWindows /= 2;
    //            gridLayout.spacing = Vector3.forward * 2;
    //        }

    //        gridLayout.constraintZ.constraintCount = numberOfWindows;

    //        List<GameObject> windowsOutlineList = new List<GameObject>();

    //        if (j == 0)
    //        {
    //            numberOfWindows--;
    //            GameObject anotherDoor = Instantiate(Door);
    //            if (isClockwise)
    //            {
    //                anotherDoor.transform.eulerAngles = tudorPillar.transform.eulerAngles + (Vector3.up * 90);
    //            }
    //            else
    //            {
    //                anotherDoor.transform.eulerAngles = tudorPillar.transform.eulerAngles - (Vector3.up * 90);
    //            }
    //            anotherDoor.transform.SetParent(aWall.transform);
    //            anotherDoor.GetComponent<LayoutElement3D>().center = new Vector3(0.4f, 1.15f, 0);

    //            windowsOutlineList.Add(anotherDoor.transform.GetChild(2).gameObject);
    //        }

    //        while (numberOfWindows != 0)
    //        {
    //            GameObject anotherWindow = Instantiate(Window);

    //            if (isClockwise)
    //            {
    //                anotherWindow.transform.eulerAngles = tudorPillar.transform.eulerAngles + (Vector3.up * 90);
    //            }
    //            else
    //            {
    //                anotherWindow.transform.eulerAngles = tudorPillar.transform.eulerAngles - (Vector3.up * 90);
    //            }

    //            anotherWindow.transform.SetParent(aWall.transform);
    //            windowsOutlineList.Add(anotherWindow.transform.GetChild(3).gameObject);
    //            numberOfWindows--;
    //        }

    //        gridLayout.UpdateLayout();

    //        float randomBevel = Random.Range(0.01f, 0.05f);

    //        aWall.GetComponent<ControlPointMesh>().InsertOutlines(windowsOutlineList, randomBevel);

    //        aWall.AddComponent<MeshCollider>();

    //        gridLayout.center = Vector3.right * randomBevel * 2;
    //        gridLayout.UpdateLayout();

    //    }
    //}

    private void CreateGenericInterior(PolyShape buildingOutline)
    {
        // Create Inside Walls
        Vector3[] controlPoints = buildingOutline.controlPoints.ToArray();
        bool isClockwise = IsPolygonClockwise(buildingOutline);

        switch (controlPoints.Length)
        {
            case 4:
                Vector3 startWallPoint = Vector3.Lerp(m_pillars[0].transform.position, m_pillars[3].transform.position, 0.5f);
                Vector3 endWallPoint = Vector3.Lerp(m_pillars[1].transform.position, m_pillars[2].transform.position, 0.5f);

                Vector3 middlePoint = Vector3.Lerp(startWallPoint, endWallPoint, 0.5f);

                GameObject interiorWall = Instantiate(WallInteriorOutline);
                interiorWall.transform.position = startWallPoint;
                interiorWall.transform.LookAt(endWallPoint);

                float wallLength = Vector3.Distance(startWallPoint, endWallPoint);

                for (int k = 0; k < interiorWall.transform.childCount; k++)
                {
                    GameObject wallInside = interiorWall.transform.GetChild(k).gameObject;

                    new MeshImporter(wallInside).Import();

                    ProBuilderMesh wallInsidePro = wallInside.GetComponent<ProBuilderMesh>();

                    wallInsidePro.Extrude(wallInsidePro.faces, ExtrudeMethod.FaceNormal, wallLength);

                    foreach (Face f in wallInsidePro.faces)
                    {
                        f.uv.Reset();
                        f.manualUV = true;
                    }

                    wallInsidePro.ToMesh();
                    wallInsidePro.Refresh();

                    wallInsidePro.SetPivot(wallInside.GetComponent<Renderer>().bounds.center);

                    Bounds wallLocalBounds = wallInside.GetComponent<Renderer>().localBounds;
                    wallLocalBounds.center = Vector3.zero;

                    wallInside.GetComponent<Renderer>().localBounds = wallLocalBounds;

                    if (wallInside.GetComponent<ZAxisLayoutGroup3D>() != null)
                    {
                        ZAxisLayoutGroup3D zAxisLayoutGroup3D = wallInside.GetComponent<ZAxisLayoutGroup3D>();

                        zAxisLayoutGroup3D.size = wallInside.GetComponent<Renderer>().localBounds.size;

                        GameObject bedroomDoor = Instantiate(DoorInterior);
                        bedroomDoor.transform.SetParent(wallInside.transform, false);

                        GameObject pantryDoor = Instantiate(DoorInterior);
                        pantryDoor.transform.SetParent(wallInside.transform, false);

                        bedroomDoor.transform.localEulerAngles = Vector3.up * 90;
                        pantryDoor.transform.localEulerAngles = Vector3.up * 90;

                        if (!isClockwise)
                        {
                            bedroomDoor.transform.localEulerAngles = Vector3.up * -90;
                            pantryDoor.transform.localEulerAngles = Vector3.up * -90;

                            zAxisLayoutGroup3D.padding.lower =
                                new Vector3(zAxisLayoutGroup3D.padding.lower.x / 2,
                                zAxisLayoutGroup3D.padding.lower.y,
                                zAxisLayoutGroup3D.padding.lower.z);
                        }

                        zAxisLayoutGroup3D.spacing = wallLength / 2;
                        zAxisLayoutGroup3D.UpdateLayout();

                        List<GameObject> doorList = new List<GameObject>();
                        doorList.Add(bedroomDoor.transform.GetChild(2).gameObject);
                        doorList.Add(pantryDoor.transform.GetChild(2).gameObject);

                        wallInside.GetComponent<ControlPointMesh>().InsertOutlines(doorList, 0);
                        wallInside.AddComponent<MeshCollider>();
                    }

                }


                GameObject verticalWall = Instantiate(WallInteriorOutline);
                verticalWall.transform.position = middlePoint;
                if (!isClockwise)
                {
                    verticalWall.transform.eulerAngles = interiorWall.transform.eulerAngles + (Vector3.up * 90);
                }
                else
                {
                    verticalWall.transform.eulerAngles = interiorWall.transform.eulerAngles + (Vector3.up * -90);
                }
                endWallPoint = Vector3.Lerp(m_pillars[2].transform.position, m_pillars[3].transform.position, 0.5f);
                wallLength = Vector3.Distance(middlePoint, endWallPoint);

                for (int k = 0; k < verticalWall.transform.childCount; k++)
                {
                    GameObject wallInside = verticalWall.transform.GetChild(k).gameObject;

                    new MeshImporter(wallInside).Import();

                    ProBuilderMesh wallInsidePro = wallInside.GetComponent<ProBuilderMesh>();

                    wallInsidePro.Extrude(wallInsidePro.faces, ExtrudeMethod.FaceNormal, wallLength);

                    foreach (Face f in wallInsidePro.faces)
                    {
                        f.uv.Reset();
                        f.manualUV = true;
                    }

                    wallInsidePro.ToMesh();
                    wallInsidePro.Refresh();
                }

                //verticalWall.transform.SetParent(houseComponents[houseComponents.Count - 1], true);

                m_buildingComponents.Add(interiorWall.transform);
                m_buildingComponents.Add(verticalWall.transform);
                break;
        }

    }

    private void CreateFloor()
    {
        ProBuilderMesh floorMesh = ProBuilderMesh.Create();
        floorMesh.gameObject.name = "Floor";

        Vector3[] controlPoints = new Vector3[m_pillars.Count];

        // Loop through pillars // min x, max z
        for (int i = 0; i < m_pillars.Count; i++)
        {
            controlPoints[i] = m_pillars[i].transform.position;
        }

        floorMesh.CreateShapeFromPolygon(controlPoints, 0.0001f, false);

        floorMesh.gameObject.GetComponent<Renderer>().material.color = Color.gray;

        floorMesh.ToMesh();
        floorMesh.Refresh();

        floorMesh.transform.position = new Vector3(0, -0.35f, 0);

        m_buildingComponents.Add(floorMesh.transform);
    }

    bool IsPolygonClockwise(PolyShape shape)
    {
        float temp = 0;
        bool isClockwise = false;

        int i = 0;
        for (; i < shape.controlPoints.Count; i++)
        {
            if (i != shape.controlPoints.Count - 1)
            {
                float mulA = shape.controlPoints[i].x * shape.controlPoints[i + 1].z;
                float mulB = shape.controlPoints[i + 1].x * shape.controlPoints[i].z;
                temp = temp + (mulA - mulB);
            }
            else
            {
                float mulA = shape.controlPoints[i].x * shape.controlPoints[i].z;
                float mulB = shape.controlPoints[0].x * shape.controlPoints[i].z;
                temp = temp + (mulA - mulB);
            }
        }
        temp /= 2;

        isClockwise = temp < 0 ? false : true;

        return isClockwise;
    }
}
