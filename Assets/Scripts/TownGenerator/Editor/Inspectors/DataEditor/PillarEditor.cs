using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(Pillar))]
public class PillarEditor : DataEditor
{
    public override VisualElement CreateInspectorGUI()
    {
        return base.CreateInspectorGUI();
    }
}
