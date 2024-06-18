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
        [SerializeField, HideInInspector] protected Buildable m_Buildable;

        public override VisualElement CreateInspectorGUI()
        {
            m_Root = new VisualElement();
            serializedObject.Update();

            m_Buildable = target as Buildable;
            //Type buildableType = target.GetType();

            m_Data = serializedObject.FindProperty("m_Data");

            PropertyField dataField = new PropertyField(m_Data);
            dataField.BindProperty(m_Data);

            m_Root.Add(dataField);

            return m_Root;
        }

        private void OnEnable()
        {
            EditorApplication.update += Build;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Build;
        }

        private void Build()
        {
            m_Buildable.Build();
        }

    }
}
