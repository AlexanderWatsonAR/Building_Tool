using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [CustomPropertyDrawer(typeof(OpeningData), useForChildren: true)]
    public class OpeningDataDrawer : DataDrawer
    {
        const string k_UssAligned = "unity-base-field__aligned";
        protected PropertyField m_Shape;

        protected Slider m_Height, m_Width, m_Angle;

        Vector2Field m_Position;

        InspectorElement m_Polygon3D;

        Foldout m_Polygon3DFoldout;

        OpeningDataSerializedProperties m_Props;

        protected override void AddFieldsToRoot()
        {
            m_Root.Add(m_Shape);
            m_Root.Add(m_Height);
            m_Root.Add(m_Width);
            m_Root.Add(m_Angle);
            m_Root.Add(m_Position);

            if(m_Props.Polygon3D != null)
            {
                m_Root.Add(m_Polygon3DFoldout);
                m_Polygon3DFoldout.Add(m_Polygon3D);
            }
        }

        protected override void BindFields()
        {
            m_Shape.BindProperty(m_Props.Shape);
            m_Height.BindProperty(m_Props.Height);
            m_Width.BindProperty(m_Props.Width);
            m_Angle.BindProperty(m_Props.Angle);
            m_Position.BindProperty(m_Props.Position);
        }

        protected override void DefineFields()
        {
            m_Shape = new PropertyField();
            m_Height = new Slider()
            {
                label = m_Props.Height.displayName,
                lowValue = 0, highValue = 1,
                showInputField = true
            };
            m_Width = new Slider()
            {
                label = m_Props.Width.displayName,
                lowValue = 0,
                highValue = 1,
                showInputField = true
            };
            m_Angle = new Slider()
            {
                label = m_Props.Angle.displayName,
                lowValue = 0,
                highValue = 180,
                showInputField = true
            };
            m_Position = new Vector2Field()
            {
                label = m_Props.Position.displayName
            };

            if (m_Props.Polygon3D != null)
            {
                m_Polygon3DFoldout = new Foldout() { text = "Content" };
                m_Polygon3D = new InspectorElement(m_Props.Polygon3D);
            }
            
            AddToClassList(); 
        }

        private void AddToClassList()
        {
            m_Height.AddToClassList(k_UssAligned);
            m_Width.AddToClassList(k_UssAligned);
            m_Angle.AddToClassList(k_UssAligned);
            m_Position.AddToClassList(k_UssAligned);
        }

        protected override void Initialize(SerializedProperty data)
        {
            m_Props = new OpeningDataSerializedProperties(data);
            //m_CurrentData = data.GetUnderlyingValue() as OpeningData;
            //m_PreviousData = m_CurrentData.Clone() as OpeningData;
            //m_ShapeHashCode = m_CurrentData.Shape.GetHashCode();
        }

        protected override void RegisterValueChangeCallbacks()
        {

        }
    }
}