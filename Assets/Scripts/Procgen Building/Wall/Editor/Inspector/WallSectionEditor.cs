using UnityEditor;
using UnityEngine.UIElements;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Wall
{
    [CustomEditor(typeof(WallSection), editorForChildClasses:false)]
    public class WallSectionEditor : BuildableEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            return base.CreateInspectorGUI();
        }
    }
}
