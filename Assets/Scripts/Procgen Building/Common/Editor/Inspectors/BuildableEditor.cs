using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using OnlyInvalid.ProcGenBuilding.Wall;
using System;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [CustomEditor(typeof(Buildable), editorForChildClasses : true)]
    public class BuildableEditor : Editor
    {
        protected SerializedProperty m_Data;
        protected VisualElement m_Root;

        public override VisualElement CreateInspectorGUI()
        {
            m_Root = new VisualElement();
            serializedObject.Update();

            m_Data = serializedObject.FindProperty("m_Data");

            PropertyField dataField = new PropertyField(m_Data);
            dataField.BindProperty(m_Data);

            dataField.RegisterValueChangeCallback(evt =>
            {
                DirtyData dirtyData = evt.changedProperty.GetUnderlyingValue() as DirtyData;

                dirtyData.IsDirty = true;
            });

            m_Root.Add(dataField);

            return m_Root;
        }

        private void OnEnable()
        {
           // Debug.Log("On Enable");
            EditorApplication.update += Build;
        }

        private void OnDisable()
        {
          //  Debug.Log("On Disable");
            EditorApplication.update -= Build;
        }

        private void Build()
        {
            Buildable buildable = target as Buildable;
            buildable.Build();
        }

    }
}
