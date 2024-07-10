using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Layout
{

    public class HorizontalLayoutGroup : LayoutGroup
    {
        public override void Layout()
        {
            throw new System.NotImplementedException();
        }
    }

    public class VerticalLayoutGroup : LayoutGroup
    {
        public override void Layout()
        {
            throw new System.NotImplementedException();
        }
    }

    public class GridLayoutGroup : LayoutGroup
    {
        public override void Layout()
        {
            throw new System.NotImplementedException();
        }
    }

    public abstract class LayoutGroup : Buildable
    {
        // layout group will control the size of the polygons
        // Consider using a vector 2 scale value to control the size of a cell. 
        // Could we use polygon3d data instead of member variables?

        // TODO: Use the Spilt Polygon function from mesh maker to create a grid.

        protected float m_Height, m_Width;
        protected Vector3 m_Normal;
        protected Vector3 m_Position;

        protected List<Polygon3D.Polygon3D> m_Polygons;

        public override void Build()
        {
            Layout();
        }

        public abstract void Layout();

    }

}

