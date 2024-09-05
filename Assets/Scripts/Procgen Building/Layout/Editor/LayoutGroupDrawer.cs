using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OnlyInvalid.ProcGenBuilding.Layout;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(LayoutGroupData), useForChildren:true)]
public class LayoutGroupDrawer : DataDrawer
{
    //PropertyField m_Polygons;
    SerializedProperty m_PolygonsProperty;
    FloatField[] m_SizeFields;

    protected override void AddFieldsToRoot()
    {
        foreach(var field in m_SizeFields)
        {
            m_Root.Add(field);
        }
    }

    protected override void BindFields()
    {
    }

    protected override void DefineFields()
    {
        m_SizeFields = new FloatField[m_PolygonsProperty.arraySize];

        for(int i = 0; i < m_SizeFields.Length; i++)
        {
            m_SizeFields[i] = new FloatField(i.ToString())
            {
                value = 100 / (float)m_SizeFields.Length
            };
        }
    }

    protected override void Initialize(SerializedProperty data)
    {
        m_PolygonsProperty = data.FindPropertyRelative("m_Polygons");
    }

    protected override void RegisterValueChangeCallbacks()
    {
        foreach(FloatField field in m_SizeFields)
        {
            field.RegisterValueChangedCallback(evt =>
            {
                float difference = Mathf.Abs(evt.previousValue - evt.newValue);
                float adjustment = difference / (float)(m_SizeFields.Length - 1);
                adjustment = evt.newValue > evt.previousValue ? -adjustment : adjustment;

                for(int i = 0; i < m_SizeFields.Length; i++)
                {
                    if (m_SizeFields[i] == field)
                        continue;

                    m_SizeFields[i].SetValueWithoutNotify(m_SizeFields[i].value + adjustment);
                }

            });
        }



    }
}
