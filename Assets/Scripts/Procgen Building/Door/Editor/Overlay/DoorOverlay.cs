using UnityEditor;
using UnityEditor.Overlays;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Door
{
    [Overlay(typeof(SceneView), nameof(Door), true)]
    public class DoorOverlay : DataOverlay
    {
    }
}
