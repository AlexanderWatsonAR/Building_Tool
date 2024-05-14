using UnityEditor;
using UnityEditor.Overlays;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Storey
{
    [Overlay(typeof(SceneView), nameof(Storey), true)]
    public class StoreyOverlay : DataOverlay
    {
    }
}
