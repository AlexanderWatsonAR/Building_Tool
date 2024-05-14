using UnityEditor;
using UnityEditor.Overlays;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Floor
{
    [Overlay(typeof(SceneView), nameof(FloorSection), true)]
    public class FloorSectionOverlay : DataOverlay
    {
    }
}
