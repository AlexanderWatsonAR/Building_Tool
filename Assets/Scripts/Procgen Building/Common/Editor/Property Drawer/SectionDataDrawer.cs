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
        }

        protected override void RegisterValueChangeCallbacks()
        {
            m_OpeningsList.RegisterValueChangeCallback(evt =>
            {
                OpeningDataList openingList = evt.changedProperty.GetUnderlyingValue() as OpeningDataList;

                //Debug.Log("Open list change");

                if (!openingList.IsDirty)
                    return;

                SectionData data = m_Data.GetUnderlyingValue() as SectionData;

                data.IsDirty = true;
                openingList.IsDirty = false;
            });
            
        }
    }
}


