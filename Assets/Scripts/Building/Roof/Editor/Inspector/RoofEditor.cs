using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(Roof))]
public class RoofEditor : DataEditor
{
    public override VisualElement CreateInspectorGUI()
    {
        return base.CreateInspectorGUI();
    }
}
