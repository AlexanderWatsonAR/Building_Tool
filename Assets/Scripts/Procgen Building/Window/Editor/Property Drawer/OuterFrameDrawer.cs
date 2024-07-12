using OnlyInvalid.ProcGenBuilding.Polygon3D;
using OnlyInvalid.ProcGenBuilding.Window;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;
using Unity.VisualScripting;

[CustomPropertyDrawer(typeof(OuterFrameData))]
public class OuterFrameDrawer : FrameDataDrawer
{
    InspectorElement m_InnerPolygon;

    protected override void AddFieldsToRoot()
    {
        base.AddFieldsToRoot();

        m_Root.Add(m_InnerPolygon);
    }

    protected override void DefineFields()
    {
        base.DefineFields();

        OuterFrameData outerFrameData = m_Data.GetUnderlyingValue() as OuterFrameData;
        m_InnerPolygon = new InspectorElement(outerFrameData.InnerPolygon3D);
    }

    protected override void Initialize(SerializedProperty data)
    {
        base.Initialize(data);

    }
}
