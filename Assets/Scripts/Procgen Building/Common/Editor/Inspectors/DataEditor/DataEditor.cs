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
    public class DataEditor : Editor
    {
        protected SerializedProperty m_Data;
        [SerializeField] protected Buildable m_Buildable;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            serializedObject.Update();

            m_Buildable = target as Buildable;
            Type buildableType = target.GetType();

            m_Data = serializedObject.FindProperty("m_" + buildableType.Name + "Data");

            PropertyField dataField = new PropertyField(m_Data);
            dataField.BindProperty(m_Data);

            root.Add(dataField);

            return root;
        }

        private void OnEnable()
        {
            EditorApplication.update = Build;
        }

        private void OnDisable()
        {
            EditorApplication.update = null;
        }

        private void Build()
        {
            m_Buildable.Build();
        }

    }
}
