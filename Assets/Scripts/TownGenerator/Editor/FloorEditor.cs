using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.ProBuilder;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.ProBuilder.MeshOperations;

[CustomEditor(typeof(Floor))]
public class FloorEditor : Editor
{
    private Floor m_Floor;
    private FloorMode m_Mode;

    private HashSet<int> m_SelectedHandles;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Add in controls for generating walls.
        // Maybe a button? or drop down?

        if(m_Mode == FloorMode.Hide)
        {
            if (GUILayout.Button("Add a Wall"))
            {
                m_Mode = FloorMode.Show;
                SceneView.RepaintAll();
            }
        }
        if(m_Mode == FloorMode.Show)
        {
            bool isDisabled = m_SelectedHandles.Count != 3;

            EditorGUI.BeginDisabledGroup(isDisabled);

            if (GUILayout.Button("Build Wall"))
            {
                BuildWalls();
                m_SelectedHandles.Clear();
                m_Mode = FloorMode.Hide;
                SceneView.RepaintAll();
            }

            EditorGUI.EndDisabledGroup();
        }


    }

    private void BuildWalls()
    {
        int[] indices = m_SelectedHandles.ToArray();
        float width = 0.1f;

        // for 3 selected handles.
        //for(int i = 0; i < indices.Length-1; i++)
        {
            Vector3 wallAStart = m_Floor.Split[indices[0]];
            Vector3 wallAEnd = m_Floor.Split[indices[1]];

            Vector3 dirA = wallAStart.DirectionToTarget(wallAEnd);

            //Use cross product of the direction to find the bottom 4 points of the wall.
            // First Wall
            Vector3 wallADir = Vector3.Cross(Vector3.up, dirA);

            Vector3[] wallAPoints = new Vector3[] { wallAStart, wallAStart, wallAEnd, wallAEnd };
            wallAPoints[0] += wallADir * -width; // bl
            wallAPoints[1] += wallADir * width; // tl
            wallAPoints[2] += wallADir * width; // tr
            wallAPoints[3] += wallADir * -width; //br

            Vector3 wallBStart = wallAEnd;
            Vector3 wallBEnd = m_Floor.Split[indices[2]];

            Vector3 dirB = wallBStart.DirectionToTarget(wallBEnd);

            Vector3 wallBDir = Vector3.Cross(Vector3.up, dirB);

            Vector3[] wallBPoints = new Vector3[] { wallBStart, wallBStart, wallBEnd, wallBEnd };
            wallBPoints[0] += wallBDir * -width; // bl
            wallBPoints[1] += wallBDir * width; // tl
            wallBPoints[2] += wallBDir * width; // tr
            wallBPoints[3] += wallBDir * -width; // br

            // These intersections will define the corner points

            if(Extensions.DoLinesIntersect(wallAPoints[0], wallAPoints[3], wallBPoints[3], wallBPoints[0], out Vector3 first))
            {
                Debug.Log("First intersection found");
            }

            if (Extensions.DoLinesIntersect(wallAPoints[1], wallAPoints[2], wallBPoints[2], wallBPoints[1], out Vector3 second))
            {
                Debug.Log("Second intersection found");
            }

            if (Extensions.DoLinesIntersect(wallAPoints[1], wallAPoints[2], wallBPoints[3], wallBPoints[0], out Vector3 third))
            {
                Debug.Log("Third intersection found");
            }

            if (Extensions.DoLinesIntersect(wallAPoints[0], wallAPoints[3], wallBPoints[1], wallBPoints[2], out Vector3 fourth))
            {
                Debug.Log("Fourth intersection found");
            }

            Vector3[] cornerPoints = new Vector3[] { first, second, third, fourth };

            cornerPoints = cornerPoints.SortPointsClockwise().ToArray();

            ProBuilderMesh corner = ProBuilderMesh.Create();
            corner.GetComponent<Renderer>().sharedMaterial = BuiltinMaterials.defaultMaterial;
            corner.name = "Corner";
            corner.CreateShapeFromPolygon(cornerPoints, 3, false);
            corner.ToMesh();
            corner.Refresh();

            ProBuilderMesh wallA = ProBuilderMesh.Create();
            wallA.GetComponent<Renderer>().sharedMaterial = BuiltinMaterials.defaultMaterial;
            wallA.name = "Wall A";
            wallA.CreateShapeFromPolygon(wallAPoints, 3, false);
            wallA.ToMesh();
            wallA.Refresh();

            ProBuilderMesh wallB = ProBuilderMesh.Create();
            wallB.GetComponent<Renderer>().sharedMaterial = BuiltinMaterials.defaultMaterial;
            wallB.name = "Wall B";
            wallB.CreateShapeFromPolygon(wallBPoints, 3, false);
            wallB.ToMesh();
            wallB.Refresh();

        }
    }

    private void OnEnable()
    {
        m_Floor = (Floor)target;
        m_Mode = FloorMode.Hide;
        m_SelectedHandles = new HashSet<int>(2);
    }

    private void OnDisable()
    {
        m_Floor = null;
        m_Mode = FloorMode.Hide;
        m_SelectedHandles.Clear();
    }

    private void OnSceneGUI()
    {
        Draw();
    }

    private void Draw()
    {
        if (m_Floor == null)
            return;

        if (m_Floor.Split == null)
            return;

        if (m_Mode == FloorMode.Hide)
            return;

        m_SelectedHandles ??= new();

        Vector3 sceneCamPos = SceneView.lastActiveSceneView.camera.transform.position;

        int controlID = 1;

        foreach (Vector3 point in m_Floor.Split)
        {

            Color handleColour = Handles.color;

            if (m_SelectedHandles.Count > 0)
            {
                Handles.color = m_SelectedHandles.Contains(controlID) ? Color.yellow : handleColour;
            }

            float size = UnityEditor.HandleUtility.GetHandleSize(point) * 0.05f;

            //Quaternion handleRot = Quaternion.LookRotation(polygon[i].DirectionToTarget(sceneCamPos));
            //BuildingHandles.TestSolidCircleHandleCap(controlID, polygon[i], handleRot, size, Event.current.type);

            Vector3 handle = Handles.FreeMoveHandle(controlID, point, Quaternion.identity, size, Vector3.up, BuildingHandles.TestSolidCircleHandleCap);

            Handles.color = handleColour;
            controlID++;
        }

        Event current = Event.current;

        if (current.button == (int)MouseButton.Left && current.type == EventType.Used && GUIUtility.hotControl > 0 && GUIUtility.hotControl < controlID)
        {
            if (m_SelectedHandles.Contains(GUIUtility.hotControl))
            {
                m_SelectedHandles.Remove(GUIUtility.hotControl);
            }
            else
            {
                m_SelectedHandles.Add(GUIUtility.hotControl);
            }

            // Updates the inspector.
            Repaint();
        }

    }

}
