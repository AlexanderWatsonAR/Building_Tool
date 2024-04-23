using UnityEditor;
using UnityEngine.UIElements;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Window
{
    [CustomEditor(typeof(Pane))]
    public class PaneEditor : DataEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            return base.CreateInspectorGUI();
        }
    }
}
