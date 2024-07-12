using OnlyInvalid.ProcGenBuilding.Layout;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LayoutScriptableObject : ContentScriptableObject
{
    [SerializeField, HideInInspector] protected LayoutGroupData m_LayoutGroupData;
    [SerializeField] protected List<ContentScriptableObject> m_Contents;


}
