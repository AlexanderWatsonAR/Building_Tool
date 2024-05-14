using UnityEditor;
using UnityEditor.Overlays;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Corner
{
    [Overlay(typeof(SceneView), nameof(Corners), true)]
    public class CornersOverlay : DataOverlay
    {
    }
}
