using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;
using UnityEditor;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace OnlyInvalid.ProcGenBuilding.Corner
{
    [System.Serializable]
    public class CornerData : Polygon3DAData
    {
        #region Members
        [SerializeField, HideInInspector] int m_ID;
        [SerializeField, HideInInspector] Vector3[] m_CornerPoints;
        [SerializeField, HideInInspector] Vector3[] m_FlatPoints;
        [SerializeField, HideInInspector] float m_Height;
        [SerializeField, HideInInspector] bool m_IsInside;
        [SerializeField] CornerType m_Type;
        [SerializeField, Range(3, 18)] int m_Sides;
        [SerializeField] float m_ShearX, m_ShearRadians;
        [SerializeField] float m_Angle;
        #endregion

        #region Accessors
        public int ID { get { return m_ID; } set { m_ID = value; } }
        public float Height { get { return m_Height; } set { m_Height = value; } }
        public CornerType Type { get { return m_Type; } set { m_Type = value; } }
        public int Sides { get { return m_Sides; } set { m_Sides = value; } }
        public float Angle => m_Angle;
        public override Vector3 Normal()
        {
            return Vector3.up;
        }
        #endregion

        #region Constructors
        public CornerData() : this(CornerType.Point, 90, 4, Vector3.forward, Vector3.zero, Vector3.zero, Vector3.zero)
        {

        }
        public CornerData(CornerType type, float angle, float height, Vector3 forward, Vector3 position, Vector3 eulerAngle, Vector3 scale) : base(position, eulerAngle, scale, null, null, height)
        {
            m_Angle = angle;
            m_Type = type;

            Vector3[] cornerPoints = PolygonMaker.WallCorner(angle);

            switch(m_Type)
            {
                case CornerType.Point:
                    {
                        m_ExteriorShape = new PathShape(cornerPoints);
                    }
                    break;
                case CornerType.Round:
                    {
                        Vector3[] curveyPoints = Vector3Extensions.QuadraticLerpCollection(cornerPoints[^1], cornerPoints[0], cornerPoints[1], 8);

                        List<Vector3> points = new List<Vector3>();
                        points.Add(cornerPoints[2]);
                        points.AddRange(curveyPoints);

                        m_ExteriorShape = new PathShape(points);
                    }
                    
                    break;
                case CornerType.Flat:
                    {
                        m_ExteriorShape = new PathShape(cornerPoints[1], cornerPoints[2], cornerPoints[3]);
                    }
                    
                    break;
            }

            forward.Normalize();

            Quaternion upRotation = Quaternion.FromToRotation(Vector3.forward, Vector3.up);

            Matrix4x4 rotate = Matrix4x4.Rotate(upRotation);
            Vector3 from = Vector3.zero.DirectionToTarget(cornerPoints[2]);

            from = rotate.MultiplyPoint3x4(from);


            Quaternion rotation = Quaternion.FromToRotation(from, forward) * upRotation;
            m_EulerAngle = rotation.eulerAngles;

        }
        public CornerData(CornerData data) : this(data.Type, data.Angle, data.Height, Vector3.forward, data.Position, data.EulerAngle, data.Scale)
        {

        }
        #endregion

        public override bool Equals(object obj)
        {
            if (obj is not CornerData)
                return false;

            CornerData corner = obj as CornerData;

            if (m_Sides == corner.Sides &&
               m_Type == corner.Type)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
