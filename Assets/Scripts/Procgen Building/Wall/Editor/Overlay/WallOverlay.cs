using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;

namespace OnlyInvalid.ProcGenBuilding.Wall
{
    [Overlay(typeof(SceneView), "Wall", true)]
    public class WallOverlay : DataOverlay /*Overlay, ITransientOverlay*/
    {
        //public override VisualElement CreatePanelContent()
        //{
            

        //    var root = new VisualElement() { name = "Wall Root" };
        //    root.Add(new Label() { text = "Grid" });
        //    root.Add(new Slider("Columns") { });
        //    root.Add(new Slider("Rows"));
        //    return root;
        //}

        //public bool visible
        //{
        //    get
        //    {
        //        if (Selection.activeGameObject != null)
        //            return Selection.activeGameObject.TryGetComponent(out Wall wall);
        //        return false;
        //    }
        //}
    }
}