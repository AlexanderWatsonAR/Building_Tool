using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Window;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OnlyInvalid.ProcGenBuilding.Window
{
    [CustomEditor(typeof(OuterFrame))]
    public class OuterFrameEditor : DataEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            return base.CreateInspectorGUI();
        }
    }
}