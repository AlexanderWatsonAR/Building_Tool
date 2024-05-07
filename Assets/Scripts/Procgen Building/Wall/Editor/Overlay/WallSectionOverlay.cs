using UnityEditor;
using UnityEditor.Overlays;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Wall
{
    [Overlay(typeof(SceneView), nameof(WallSection), true)]
    public class WallSectionOverlay : DataOverlay
    {
    }
}
