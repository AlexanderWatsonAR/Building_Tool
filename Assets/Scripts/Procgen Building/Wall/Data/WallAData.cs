using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using OnlyInvalid.ProcGenBuilding.Wall;
using OnlyInvalid.ProcGenBuilding.Corner;
using Unity.VisualScripting;
using UnityEngine.ProBuilder;

[System.Serializable]
public class WallAData : Polygon3DAData
{
    #region Accessors
    public List<OpeningAData> Openings => m_InteriorShapes.ConvertAll(x => x as OpeningAData);

    public override Vector3 Normal()
    {
        return Vector3.up;
    }

    #endregion

    #region Constructors
    public WallAData() : this(Vector3.zero, Vector3.zero, Vector3.one)
    {
        m_InteriorShapes = new List<Polygon2DData>();

        OpeningAData opening = new OpeningAData(new Square(), Vector3.up * 0.5f, Vector3.zero, Vector3.one * 0.5f);

        m_InteriorShapes.Add(opening);

    }
    public WallAData(Vector3 position, Vector3 eulerAngle, Vector3 scale) : base(position, eulerAngle, scale, new Square(PolygonMaker.PivotPoint.BottomCentre), null, 1)
    {
    }
    public WallAData (CornerData cornerA, CornerData cornerB)
    {
        m_Shape = new Square();
        m_InteriorShapes = new List<Polygon2DData>();

        // OpeningAData opening = new OpeningAData(new Square(), Vector3.zero, Vector3.zero, Vector3.one * 0.5f);

        //m_InteriorShapes.Add(opening);

        //CornerData dataA = cornerA.CornerData;
        //CornerData dataB = cornerB.CornerData;

        Vector3[] cornerAPoints = cornerA.ControlPoints;
        Vector3[] cornerBPoints = cornerB.ControlPoints;

        float length = Vector3.Distance(cornerAPoints[0], cornerBPoints[0]);
        Vector3 direction = cornerA.Position.DirectionToTarget(cornerB.Position);

        int cornerAIndex = 0;
        int cornerBIndex = 0;

        for (int i = 0; i < cornerAPoints.Length; i++)
        {
            for (int j = 0; j < cornerBPoints.Length; j++)
            {
                float distance = Vector3.Distance(cornerAPoints[i], cornerBPoints[j]);

                // Here we are locating the points where the wall should start and end.

                Vector3 dirB = cornerAPoints[i].DirectionToTarget(cornerAPoints[0]);
                Vector3 dirC = cornerBPoints[j].DirectionToTarget(cornerBPoints[0]);

                if (dirB == -direction)
                    cornerAIndex = i;

                if (dirC == direction)
                    cornerBIndex = j;

                length = distance < length ? distance : length;
            }
        }

        Vector3 forward = Vector3.Cross(direction, Vector3.up);

        m_Position = Vector3.Lerp(cornerAPoints[cornerAIndex], cornerBPoints[cornerBIndex], 0.5f) + (Vector3.up * 0.5f);
        m_EulerAngle = Quaternion.FromToRotation(Vector3.forward, forward).eulerAngles;
        m_Scale = new Vector3(length, 1);
        m_Depth = cornerA.Scale.z;

        //ProBuilderMesh frameMesh = ProBuilderMesh.Create();
        //Frame frame = frameMesh.AddComponent<Frame>();

        //Vector3 pos = this.Position + opening.Position + Vector3.zero;
        //Vector3 eulerAngle = (this.Rotation * opening.Rotation * Quaternion.identity).eulerAngles;
        //Vector3 scale = Vector3.Scale(Vector3.Scale(this.Scale, opening.Scale), Vector3.one);

        //frame.transform.position = pos;
        //frame.transform.eulerAngles = eulerAngle;
        //frame.transform.localScale = scale;

        //FrameData data = new FrameData(new Square(), new List<Polygon2DData>(), 0.95f, 0.05f, pos, eulerAngle, scale);
        //data.IsDirty = true;
        //frame.Initialize(data);

        //opening.Content = frame;


    }
    #endregion

}
