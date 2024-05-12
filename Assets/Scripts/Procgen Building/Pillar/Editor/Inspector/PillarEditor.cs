using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Pillar
{
    [CustomEditor(typeof(Pillar))]
    public class PillarEditor : BuildableEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            return base.CreateInspectorGUI();
        }
    }
}
