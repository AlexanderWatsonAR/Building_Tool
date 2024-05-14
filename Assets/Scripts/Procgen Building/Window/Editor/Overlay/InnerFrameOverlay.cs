using UnityEditor;
using UnityEditor.Overlays;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Window
{
    [Overlay(typeof(SceneView), nameof(InnerFrame), true)]
    public class InnerFrameOverlay : DataOverlay
    {
    }
}
