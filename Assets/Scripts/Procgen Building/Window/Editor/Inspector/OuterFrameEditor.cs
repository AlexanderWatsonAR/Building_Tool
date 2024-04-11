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

        private void OnValidate() 
        {
            // TODO: this should start a chain of validate events going up to the building

            OuterFrame outerFrame = target as OuterFrame;
            Window window = outerFrame.transform.parent.GetComponent<Window>();

            window.Data.OuterFrame = outerFrame.Data;
        }
    }
}