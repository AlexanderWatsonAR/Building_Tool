using UnityEditor;
using UnityEditor.Overlays;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Roof
{
    [Overlay(typeof(SceneView), nameof(RoofSection), true)]
    public class RoofSectionOverlay : DataOverlay
    {
    }
}
