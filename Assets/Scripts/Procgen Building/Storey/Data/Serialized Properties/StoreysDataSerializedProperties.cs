using OnlyInvalid.ProcGenBuilding.Storey;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Storey
{
    public class StoreysDataSerializedProperties : SerializedPropertyGroup
    {
        readonly List<StoreyDataSerializedProperties> m_Storeys;

        public List<StoreyDataSerializedProperties> Storeys => m_Storeys;

        public StoreysDataSerializedProperties(SerializedProperty data) : base(data)
        {
            m_Storeys = new List<StoreyDataSerializedProperties>(data.arraySize);

            for (int i = 0; i < data.arraySize; i++)
            {
                m_Storeys.Add(new StoreyDataSerializedProperties(data.GetArrayElementAtIndex(i)));
            }

        }
    }
}


