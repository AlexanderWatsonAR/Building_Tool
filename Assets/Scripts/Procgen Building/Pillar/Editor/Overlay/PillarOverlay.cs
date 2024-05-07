using UnityEditor;
using UnityEditor.Overlays;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Pillar
{
    [Overlay(typeof(SceneView), nameof(Pillar), true)]
    public class PillarOverlay : DataOverlay
    {
    }
}
