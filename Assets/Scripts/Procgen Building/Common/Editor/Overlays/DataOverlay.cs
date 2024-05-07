using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Overlays;
using OnlyInvalid.ProcGenBuilding.Common;
using System;

//[Overlay(typeof(SceneView), displayName : "Data Overlay", defaultDisplay: true)]
public abstract class DataOverlay : Overlay, ITransientOverlay
{
    protected VisualElement m_Root;
    protected SerializedObject m_SerializedObject;
    protected SerializedProperty m_Data;
    protected PropertyField m_DataField;
    protected Buildable m_Buildable;


    public override VisualElement CreatePanelContent()
    {
        if (m_Buildable == null)
            return new VisualElement();

        Type buildableType = m_Buildable.GetType();
        displayName = buildableType.Name;

        m_Root = new VisualElement();
        m_SerializedObject = new SerializedObject(m_Buildable);

        m_Data = m_SerializedObject.FindProperty("m_" + buildableType.Name + "Data");
        m_DataField = new PropertyField(m_Data);
        m_DataField.BindProperty(m_Data);
        m_Root.Add(m_DataField);

        return m_Root;
    }

    public bool visible
    {
        get
        {
            return IsBuildable(Selection.activeGameObject);
        }
    }

    bool IsBuildable(GameObject gameObject)
    {
        if (gameObject == null)
            return false;

        bool isBuildable = Selection.activeGameObject.TryGetComponent(out Buildable buildable);

        if (isBuildable)
        {
            m_Buildable = buildable;
        }
        return isBuildable;
    }
}
