using UnityEditor;
using UnityEditor.Overlays;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Wall
{
    [Overlay(typeof(SceneView), nameof(Wall), true)]
    public class WallOverlay : DataOverlay
    {
    }
}