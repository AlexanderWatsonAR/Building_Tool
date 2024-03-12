using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TransformDataSerializedProperties : SerializedPropertyGroup
{
    #region Constants 
    const string k_RelativePosition = "m_RelativePosition";
    const string k_AbsolutePosition = "m_AbsolutePosition";
    const string k_PositionOffset = "m_PositionOffset";
    const string k_EulerAngle = "m_EulerAngle";
    const string k_Scale = "m_Scale";
    #endregion

    public SerializedProperty RelativePosition => m_Data.FindPropertyRelative(k_RelativePosition);
    public SerializedProperty AbsolutePosition => m_Data.FindPropertyRelative(k_AbsolutePosition);
    public SerializedProperty PositionOffset => m_Data.FindPropertyRelative(k_PositionOffset);
    public SerializedProperty EulerAngle => m_Data.FindPropertyRelative(k_EulerAngle);
    public SerializedProperty Scale => m_Data.FindPropertyRelative(k_Scale);


    public TransformDataSerializedProperties(SerializedProperty data) : base(data)
    {

    }

}
