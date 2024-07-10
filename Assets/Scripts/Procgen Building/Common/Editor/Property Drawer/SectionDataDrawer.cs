using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using OnlyInvalid.CustomVisualElements;
using Unity.VisualScripting;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [CustomPropertyDrawer(typeof(SectionData), false)]
    public class SectionDataDrawer : DataDrawer
    {
        SerializedProperty m_Openings;
        PropertyField m_OpeningsList;

        protected override void AddFieldsToRoot()
        {
            m_Root.Clear();
            m_Root.Add(m_OpeningsList);
        }

        protected override void BindFields()
        {
            m_OpeningsList.BindProperty(m_Openings);
        }

        protected override void DefineFields()
        {
            m_OpeningsList = new PropertyField();
        }

        protected override void Initialize(SerializedProperty data)
        {
            m_Openings = m_Data.FindPropertyRelative("m_Openings");

            AddListeners();
        }
        private void AddListeners()
        {
            //OpeningDataList list = m_Openings.GetUnderlyingValue() as OpeningDataList;

            //list.OnAdd.AddListener(opening => { MarkDirty(); AddFieldsToRoot(); });
            //list.OnRemove.AddListener(() => { MarkDirty(); AddFieldsToRoot(); });
            //list.OnShiftDown.AddListener(() => { MarkDirty(); AddFieldsToRoot(); });
            //list.OnShiftUp.AddListener(() => { MarkDirty(); AddFieldsToRoot(); });
        }
        protected override void RegisterValueChangeCallbacks()
        {
            m_OpeningsList.RegisterValueChangeCallback(evt =>
            {
                OpeningDataList openingList = evt.changedProperty.GetUnderlyingValue() as OpeningDataList;

                if (!openingList.IsDirty)
                    return;

                Debug.Log("Opening Data List Change");

                SectionData data = m_Data.GetUnderlyingValue() as SectionData;

                data.IsDirty = true;
                openingList.IsDirty = false;
            });

            //m_OpeningsList.RegisterCallback<OpeningAddEvent>(item => 
            //{
            //    MarkDirty();
            //    AddFieldsToRoot();
            //});
        }

        private void MarkDirty()
        {
            SectionData data = m_Data.GetUnderlyingValue() as SectionData;
            data.IsDirty = true;
        }
    }
}