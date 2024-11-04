using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    [System.Serializable]
    public class GridFrameData : FrameData
    {
        #region Members
        [SerializeField] int m_Columns, m_Rows;
        #endregion

        #region Accessors
        public int Columns { get { return m_Columns; } set { m_Columns = value; } }
        public int Rows { get { return m_Rows; } set { m_Rows = value; } }
        #endregion

        #region Constructors
        public GridFrameData() : this(2, 2, 0.95f)
        {

        }
        public GridFrameData(int columns, int rows) : base()
        {
            m_Columns = columns;
            m_Rows = rows;
        }
        public GridFrameData(int columns, int rows, float insideScale) : this(columns, rows)
        {
            m_InsideScale = insideScale;
        }
        public GridFrameData(Shape exteriorShape, List<Polygon2DData> interiorShapes, int columns, int rows, float frameScale, float depth, Vector3 position, Vector3 eulerAngle, Vector3 scale) : base(exteriorShape, interiorShapes, frameScale, depth, position, eulerAngle, scale)
        {
            m_Columns = columns;
            m_Rows = rows;
        }
        public GridFrameData(GridFrameData data) : this(data.ExteriorShape, data.InteriorShapes, data.Columns, data.Rows, data.InsideScale, data.Depth, data.Position, data.EulerAngle, data.Scale)
        {
        }
        #endregion

        #region Overrides
        public override bool Equals(object obj)
        {
            GridFrameData other = obj as GridFrameData;

            if (other == null)
                return false;

            if (m_Columns == other.Columns &&
               m_Rows == other.Rows &&
               base.Equals(obj))
            {
                return true;
            }

            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

    }
}
