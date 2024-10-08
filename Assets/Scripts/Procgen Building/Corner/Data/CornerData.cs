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
        [SerializeField] CornerType m_Type;
        [SerializeField] int m_Sides;
        [SerializeField] float m_Angle;
        [SerializeField] Vector3 m_Forward;
        #endregion

        #region Accessors
        public int ID { get => m_ID; set => m_ID = value; }
        public CornerType Type { get => m_Type;  set => m_Type = value; }
        public int Sides { get => m_Sides;  set => m_Sides = value; }
        public float Angle { get => m_Angle; set => m_Angle = value; }
        public Vector3 Forward { get => m_Forward; set => m_Forward = value; }
        public override Vector3 Normal()
        {
            return Vector3.up;
        }
        #endregion

        #region Constructors
        public CornerData() : this(CornerType.Point, 90, 5, Vector3.forward, Vector3.zero, Vector3.zero, Vector3.zero)
        {

        }
        public CornerData(CornerType type, float angle, int sides, Vector3 forward, Vector3 position, Vector3 eulerAngle, Vector3 scale) : base(position, eulerAngle, scale, null, null, 1)
        {
            m_Angle = angle;
            m_Forward = forward;
            m_Type = type;
            m_Sides = sides;
        }
        public CornerData(CornerData data) : this(data.Type, data.Angle, data.Sides, data.Forward, data.Position, data.EulerAngle, data.Scale)
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
