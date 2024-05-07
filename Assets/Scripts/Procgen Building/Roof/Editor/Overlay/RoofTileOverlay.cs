using UnityEditor;
using UnityEditor.Overlays;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Roof
{
    [Overlay(typeof(SceneView), nameof(RoofTile), true)]
    public class RoofTileOverlay : DataOverlay
    {
    }
}
