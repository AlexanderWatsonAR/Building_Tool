using OnlyInvalid.ProcGenBuilding.Polygon3D;

namespace OnlyInvalid.ProcGenBuilding.Roof
{
    /// <summary>
    /// Thoughts on this class
    /// Roof tile should behave in a similar way to the wall class.
    /// Main difference being, the roof tile projects outwards(or inwards depeneding on how you create it.
    /// projection should have a 90d fov & a 1:1 aspect ratio.
    /// May need to have multiple types of roof class.
    /// what about having seperate corner objects like with the walls?
    /// </summary>

    public class RoofTile : Polygon3D.Polygon3D
    {
        public RoofTileData RoofTileData => m_Data as RoofTileData;

        public override void Build()
        {
            base.Build();
        }

        private void OnDrawGizmosSelected()
        {
        }
    }
}
