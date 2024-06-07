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
        SerializedProperty[] m_OpeningsArray;
        HeaderFoldout[] m_HeaderFoldouts;
        PropertyField[] m_OpeningFields;

        protected override void AddFieldsToRoot()
        {
            for(int i = 0; i < m_HeaderFoldouts.Length; i++)
            {
                m_HeaderFoldouts[i].AddItem(m_OpeningFields[i]);
                m_Root.Add(m_HeaderFoldouts[i]);
            }
        }

        protected override void BindFields()
        {
            for(int i = 0; i < m_OpeningFields.Length; i++)
            {
                m_OpeningFields[i].BindProperty(m_OpeningsArray[i]);
            }
        }

        protected override void DefineFields()
        {
            for(int i = 0; i < m_OpeningsArray.Length; i++)
            {
                m_HeaderFoldouts[i] = new HeaderFoldout("Opening " + i.ToString());
                m_OpeningFields[i] = new PropertyField();
            }
        }

        protected override void Initialize(SerializedProperty data)
        {
            m_Openings = m_Data.FindPropertyRelative("m_Openings");
            m_OpeningsArray = new SerializedProperty[m_Openings.arraySize];
            m_HeaderFoldouts = new HeaderFoldout[m_Openings.arraySize];
            m_OpeningFields = new PropertyField[m_Openings.arraySize];

            for(int i = 0; i < m_OpeningsArray.Length; i++)
            {
                m_OpeningsArray[i] = m_Openings.GetArrayElementAtIndex(i);
            }
        }

        protected override void RegisterValueChangeCallbacks()
        {
            for (int i = 0; i < m_OpeningFields.Length; i++)
            {
                m_OpeningFields[i].RegisterValueChangeCallback(evt =>
                {
                    SectionData data = m_Data.GetUnderlyingValue() as SectionData;
                    data.IsDirty = true;
                });
            }
        }
    }
}


