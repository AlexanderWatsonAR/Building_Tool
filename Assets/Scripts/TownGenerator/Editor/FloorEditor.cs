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
        SerializedProperty data = serializedObject.FindProperty("m_Data");

        SerializedProperty columns = data.FindPropertyRelative("m_Columns");
        SerializedProperty rows = data.FindPropertyRelative("m_Rows");

        EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(columns);
        EditorGUILayout.PropertyField(rows);
        EditorGUI.indentLevel--;

        // Add in controls for generating walls.
        // Maybe a button? or drop down?

        if (m_Mode == FloorMode.Hide)
        {
            if (GUILayout.Button("Add a Wall"))
            {
                m_Mode = FloorMode.Show;
                SceneView.RepaintAll();
            }
        }
        if(m_Mode == FloorMode.Show)
        {
            bool isDisabled = m_SelectedHandles.Count < 2;

            EditorGUI.BeginDisabledGroup(isDisabled);

            if (GUILayout.Button("Build Wall"))
            {
                Build();
               // BuildWalls();
                m_SelectedHandles.Clear();
                m_Mode = FloorMode.Hide;
                SceneView.RepaintAll();
            }

            EditorGUI.EndDisabledGroup();
        }

        if(serializedObject.ApplyModifiedProperties())
        {
            m_Floor.Build();
        }

    }

    private ControlPoint[] CalculateControlPoints()
    {
        int[] indices = m_SelectedHandles.ToArray();

        ControlPoint[] controlPoints = new ControlPoint[indices.Length];

        for(int i = 0; i < controlPoints.Length; i++)
        {
            controlPoints[i] = new ControlPoint(m_Floor.Split[indices[i] - 1]);
        }

        controlPoints[^1].SetForward(Vector3.Cross(controlPoints[^2].DirectionToTarget(controlPoints[^1]), Vector3.up));

        for (int i = 0; i < controlPoints.Length -1; i++)
        {
            int next = i + 1;

            controlPoints[i].SetForward(Vector3.Cross(controlPoints[i].DirectionToTarget(controlPoints[next]), Vector3.up));
        }

        return controlPoints;
    }

    private void Build()
    {
        ControlPoint[] controlPoints = CalculateControlPoints();

        float depth = 0.1f;
        float height = 3;

        int bl = 0;
        int tl = 1;
        int tr = 2;
        int br = 3;

        List<Vector3> startEndPoints = new List<Vector3>();
        startEndPoints.Add(controlPoints[0].Position + (controlPoints[0].Forward * (depth*0.5f)));

        Vector3[][] corners = new Vector3[controlPoints.Length][];
        bool[] isRightCorner = new bool[controlPoints.Length];

        for(int i = 1; i < controlPoints.Length-1; i++)
        {
            int previous = i - 1;
            int next = i + 1;

            bool isRight = Vector3.Cross(controlPoints[previous].DirectionToTarget(controlPoints[i]), controlPoints[previous].DirectionToTarget(controlPoints[next])).z > 0;

            isRightCorner[i] = isRight;

            Vector3 dirA = Vector3.Cross(controlPoints[previous].DirectionToTarget(controlPoints[i]), Vector3.up);
            Vector3 dirB = Vector3.Cross(controlPoints[i].DirectionToTarget(controlPoints[next]), Vector3.up);
            Vector3 dirC = controlPoints[i].DirectionToTarget(controlPoints[next]);
            Vector3 dirD = controlPoints[i].DirectionToTarget(controlPoints[previous]);
            Vector3 dirE = Vector3.Lerp(dirC, dirD, 0.5f);

            Vector3[] cornerPoints = new Vector3[4];

            if (isRight)
            {
                cornerPoints[br] = controlPoints[i].Position;
                cornerPoints[bl] = cornerPoints[br] + (dirA * depth);
                cornerPoints[tr] = cornerPoints[br] + (dirB * depth);

                Extensions.DoLinesIntersect(cornerPoints[bl], cornerPoints[bl] + (dirD * -depth), cornerPoints[tr], cornerPoints[tr] + (dirC * -depth), out cornerPoints[tl]);

                Vector3 offset = cornerPoints[br] - Math.Average(cornerPoints);

                for(int j = 0; j < cornerPoints.Length; j++)
                {
                    cornerPoints[j] += offset;
                }

                startEndPoints.Add(cornerPoints[bl]); // 1st wall ends at bl
                startEndPoints.Add(cornerPoints[tr]); // 2nd wall starts at tr
            }
            else
            {
                cornerPoints[bl] = controlPoints[i].Position;
                cornerPoints[br] = cornerPoints[bl] + (dirA * -depth);
                cornerPoints[tl] = cornerPoints[bl] + (dirB * -depth);

                Extensions.DoLinesIntersect(cornerPoints[br], cornerPoints[br] + (dirD * -depth), cornerPoints[tl], cornerPoints[tl] + (dirC * -depth), out cornerPoints[tr]);

                Vector3 offset = cornerPoints[bl] - Math.Average(cornerPoints);

                for (int j = 0; j < cornerPoints.Length; j++)
                {
                    cornerPoints[j] += offset;
                }


                startEndPoints.Add(cornerPoints[bl]); // 1st wall ends at bl
                startEndPoints.Add(cornerPoints[bl]); // 2nd wall ends at bl
            }

            corners[i] = cornerPoints;
        }

        startEndPoints.Add(controlPoints[^1].Position + (controlPoints[^1].Forward * (depth*0.5f)));

        int count = 0;

        Wall[] walls = new Wall[controlPoints.Length];

        for(int i = 0; i < startEndPoints.Count-1; i += 2)
        {
            int next = i + 1;
            Vector3 wallStart = startEndPoints[i];
            Vector3 wallEnd = startEndPoints[next];

            //Vector3[] wallPoints = new Vector3[] { wallStart, wallStart + (Vector3.up * height), wallEnd + (Vector3.up * height), wallEnd };

            ProBuilderMesh wall = ProBuilderMesh.Create();
            wall.name = "Wall " + count.ToString();
            wall.AddComponent<Wall>();
            WallData data = new WallData()
            {
                Start = wallStart,
                End = wallEnd,
                Material = BuiltinMaterials.defaultMaterial,
                Depth = depth,
                Height = height
            };
            wall.GetComponent<Wall>().Initialize(data).Build();
            walls[count] = wall.GetComponent<Wall>();
            count++;
        }


        for( int i = 1; i < controlPoints.Length-1; i++)
        {
            Vector3[] corner = corners[i];

            if (!isRightCorner[i])
            {
                //corner[tl] = walls[i].WallData.BottomPoints()[tl];

                //GameObject marker = new GameObject("Wall tl");
                //marker.transform.position = walls[i].WallData.BottomPoints()[tl] + (Vector3.up * height);

                //GameObject marker1 = new GameObject("corner tl");
                //marker1.transform.position = corner[tl] + (Vector3.up * height);
            }

            ProBuilderMesh cornerMesh = ProBuilderMesh.Create();
            cornerMesh.GetComponent<Renderer>().sharedMaterial = BuiltinMaterials.defaultMaterial;
            cornerMesh.name = "Corner";
            cornerMesh.CreateShapeFromPolygon(corner, height, false);
            cornerMesh.ToMesh();
            cornerMesh.Refresh();
            cornerMesh.SetSelectedVertices(new int[] { br });

        }

    }

    private void BuildWalls()
    {
        // Indices are + 1 out
        int[] indices = m_SelectedHandles.ToArray();
        float depth = 0.1f;
        float height = 3;

        int bl = 0;
        int tl = 1;
        int tr = 2;
        int br = 3;

        for (int i = 0; i < indices.Length -2; i++)
        {
            // first wall
            Vector3 wallAStart = m_Floor.Split[indices[i] - 1];
            Vector3 wallAEnd = m_Floor.Split[indices[i + 1] - 1];
            Vector3 dirA = wallAStart.DirectionToTarget(wallAEnd);
            Vector3 wallAFaceNormal = Vector3.Cross(Vector3.up, dirA);

            wallAStart += wallAFaceNormal * (-depth * 0.5f);
            wallAEnd += wallAFaceNormal * (-depth * 0.5f);

            Vector3[] wallAPoints = new Vector3[] { wallAStart, wallAStart, wallAEnd, wallAEnd };
            wallAPoints[1] += wallAFaceNormal * depth; // tl
            wallAPoints[2] += wallAFaceNormal * depth; // tr

            // second wall
            Vector3 wallBStart = wallAEnd;
            Vector3 wallBEnd = m_Floor.Split[indices[i + 2] - 1];
            Vector3 dirB = wallBStart.DirectionToTarget(wallBEnd);
            Vector3 wallBFaceNormal = Vector3.Cross(Vector3.up, dirB);

            wallBStart += wallBFaceNormal * (-depth * 0.5f);
            wallBEnd += wallBFaceNormal * (-depth * 0.5f);

            Vector3[] wallBPoints = new Vector3[] { wallBStart, wallBStart, wallBEnd, wallBEnd };
            wallBPoints[1] += wallBFaceNormal * depth; // tl
            wallBPoints[2] += wallBFaceNormal * depth; // tr

            // WallAPoints[3] is equal to WallBPoints[0]

            Vector3 aStartToBEnd = wallAStart.DirectionToTarget(wallBEnd);
            Vector3 cross = Vector3.Cross(dirA, aStartToBEnd); // By looking a the z value we can determine if the angle is moving left or right

            Vector3[] cornerPoints = new Vector3[4];

            if (cross.z > 0)
            {
                Vector3 middle = m_Floor.Split[indices[i + 1] - 1];

                dirA = middle.DirectionToTarget(m_Floor.Split[indices[i] - 1]);
                dirB = middle.DirectionToTarget(m_Floor.Split[indices[i + 2] - 1]);

                Vector3 dirC = Vector3.Lerp(dirA, dirB, 0.5f);
                Vector3 dirD = Vector3.Cross(Vector3.up, dirC);

                cornerPoints[bl] = middle + (dirD * depth);
                cornerPoints[tl] = middle + (dirC * -depth);
                cornerPoints[tr] = middle + (dirD * -depth);
                cornerPoints[br] = middle + (dirC * depth);

                // Right

            }
            else
            {
                //// Left
                //cornerPoints[bl] = wallAPoints[tr];
                //Extensions.DoLinesIntersect(wallAPoints[tl], wallAPoints[tr], wallBPoints[tl], wallBPoints[tr], out cornerPoints[1]);
                //Extensions.DoLinesIntersect(wallAPoints[bl], wallAPoints[br], wallBPoints[bl], wallBPoints[br], out cornerPoints[2]);
                //cornerPoints[br] = wallAPoints[br];
            }

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


            //Debug.Log(dot);

            //float normalizedAngle = (angle - min) / (max - min);

            //wallPoints[2] += dir * (-depth * normalizedAngle);
            //wallPoints[3] += dir * (-depth * normalizedAngle);

            //if (i != 0 && indices.Length >= 3)
            //{
            //    // Reduce start
            //    Vector3 dirPrevious = wallStart.DirectionToTarget(m_Floor.Split[indices[i - 1] - 1]);

            //    float angle = Vector3.Angle(dir, dirPrevious);
            //    float normalizedAngle = (angle - min) / (max - min);

            //    wallPoints[0] += dir * (depth * normalizedAngle);
            //    wallPoints[1] += dir * (depth * normalizedAngle);
            //}


            //ProBuilderMesh wall = ProBuilderMesh.Create();
            //wall.name = "Wall " + i.ToString();
            //wall.AddComponent<Wall>();
            //WallData data = new WallData()
            //{
            //    ControlPoints = wallPoints,
            //    Material = BuiltinMaterials.defaultMaterial,
            //    Depth = depth,
            //    Height = height
            //};
            //wall.GetComponent<Wall>().Initialize(data).Build();
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
