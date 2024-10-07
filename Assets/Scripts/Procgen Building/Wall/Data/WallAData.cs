using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using OnlyInvalid.ProcGenBuilding.Wall;
using OnlyInvalid.ProcGenBuilding.Corner;

[System.Serializable]
public class WallAData : Polygon3DAData
{
    #region Members
    [SerializeField] float m_Height;
    #endregion

    #region Constructors
    public WallAData(Vector3 position, Vector3 eulerAngle, Vector3 scale, float height, float depth) : base(position, eulerAngle, scale, new Square(), null, depth)
    {
        m_Height = height;
    }
    public WallAData (CornerData cornerA, CornerData cornerB)
    {
        m_ExteriorShape = new Square();
        m_InteriorShapes = null;

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
        m_Depth = cornerA.Scale.x;

    }
    #endregion

}
