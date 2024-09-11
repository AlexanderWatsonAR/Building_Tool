using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(AGridData))]
public class AGridDataPropertyDrawer : DataDrawer
{
    Vector3Field m_Position, m_EulerAngle, m_Scale;

    PropertyField m_Exterior, m_Interior, m_Rotation;

    Vector2IntField m_Dimensions;
    Vector2Field m_GridScale;

    FloatField m_Depth;

    

    protected override void AddFieldsToRoot()
    {
        m_Root.Add(m_Position);
       // m_Root.Add(m_Rotation);
        m_Root.Add(m_EulerAngle);
        m_Root.Add(m_Scale);
        m_Root.Add(m_Exterior);
        m_Root.Add(m_Interior);
        m_Root.Add(m_Depth);

        m_Root.Add(m_Dimensions);
        m_Root.Add(m_GridScale);
    }

    protected override void BindFields()
    {
        SerializedProperty position = m_Data.FindPropertyRelative("m_Position");
        //SerializedProperty rotation = m_Data.FindPropertyRelative("m_Rotation");
        SerializedProperty eulerAngle = m_Data.FindPropertyRelative("m_EulerAngle");
        SerializedProperty scale = m_Data.FindPropertyRelative("m_Scale");

        SerializedProperty exterior = m_Data.FindPropertyRelative("m_ExteriorShape");
        SerializedProperty interior = m_Data.FindPropertyRelative("m_InteriorShapes");

        SerializedProperty depth = m_Data.FindPropertyRelative("m_Depth");

        SerializedProperty dimensions = m_Data.FindPropertyRelative("m_Dimensions");
        SerializedProperty gridScale = m_Data.FindPropertyRelative("m_GridScale");

        m_Position.BindProperty(position);
        m_EulerAngle.BindProperty(eulerAngle);
        //m_Rotation.BindProperty(rotation);
        m_Scale.BindProperty(scale);
        m_Exterior.BindProperty(exterior);
        m_Interior.BindProperty(interior);
        m_Depth.BindProperty(depth);
        m_Dimensions.BindProperty(dimensions);
        m_GridScale.BindProperty(gridScale);



    }

    protected override void DefineFields()
    {
        m_Position = new Vector3Field();
        //m_Rotation = new PropertyField();
        m_EulerAngle = new Vector3Field();
        m_Scale = new Vector3Field();
        m_Exterior = new PropertyField();
        m_Interior = new PropertyField();
        m_Depth = new FloatField();

        m_Dimensions = new Vector2IntField();
        m_GridScale = new Vector2Field();


    }

    protected override void Initialize(SerializedProperty data)
    {
    }

    protected override void RegisterValueChangeCallbacks()
    {

    }
}
