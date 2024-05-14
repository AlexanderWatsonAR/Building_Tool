using UnityEditor;
using UnityEditor.Overlays;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Floor
{
    [Overlay(typeof(SceneView), nameof(Floor), true)]
    public class FloorOverlay : DataOverlay
    {
    }
}
