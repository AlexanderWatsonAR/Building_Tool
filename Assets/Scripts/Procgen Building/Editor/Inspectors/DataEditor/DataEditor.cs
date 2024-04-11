using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [CustomEditor(typeof(IBuildable), true)]
    public class DataEditor : Editor
    {
        protected SerializedProperty m_Data;
        [SerializeField] protected IBuildable m_Buildable;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            serializedObject.Update();

            m_Data = serializedObject.FindProperty("m_Data");
            m_Buildable = target as IBuildable;

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