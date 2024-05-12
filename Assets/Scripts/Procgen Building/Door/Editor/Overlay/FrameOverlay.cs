using UnityEditor;
using UnityEditor.Overlays;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Door
{
    [Overlay(typeof(SceneView), nameof(Frame), true)]
    public class FrameOverlay : DataOverlay
    {
    }
}
