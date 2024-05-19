using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [System.Serializable]
    public class Opening
    {
        // Serial Ref because it is polymorphic.
        [SerializeReference] Shape m_Shape;
        [SerializeField] ShapeType m_ShapeType;

        public Shape Shape { get { return m_Shape; } set { m_Shape = value; } }
        public ShapeType Type { get { return m_ShapeType; } set { m_ShapeType = value; } }

        public Opening()
        {

        }

        public Opening(Shape shape, ShapeType shapeType)
        {
            m_Shape = shape;
            m_ShapeType = shapeType;
        }
    }
}
