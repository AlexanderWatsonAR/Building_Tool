using UnityEditor;
using UnityEditor.Overlays;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Pillar
{
    [Overlay(typeof(SceneView), nameof(Pillars), true)]
    public class PillarsOverlay : DataOverlay
    {
    }
}
