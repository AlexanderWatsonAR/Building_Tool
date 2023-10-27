using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;


// I want this linked to the wall class.
// when the inspector is updated this should be updated and vice versa.
// imgui may be a better solution.
// it may be a better idea to abstract the custom editor code, so that the overlay script & this can call the same functions.

[Overlay(typeof(SceneView), "Wall", true)]
public class WallOverlay : Overlay
{
    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement() { name = "Wall Root" };
        root.Add(new Label() { text = "Grid" });
        root.Add(new Slider("Columns"));
        root.Add(new Slider("Rows"));
        return root;

    }
}