using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;

namespace OnlyInvalid.ProcGenBuilding.Door
{
    [CustomPropertyDrawer(typeof(DoorData))]
    public class DoorDataDrawer : Polygon3D.Polygon3DDataDrawer
    {
        DoorData m_PreviousData, m_CurrentData;
        DoorDataSerializedProperties m_Props;

        PropertyField m_Scale;
        Foldout m_HingeFoldout;
        PropertyField m_Hinge;

        protected override void AddFieldsToRoot()
        {
            base.AddFieldsToRoot();
            m_Root.Add(m_Scale);
            m_Root.Add(m_HingeFoldout);
            m_HingeFoldout.Add(m_Hinge);
        }

        protected override void BindFields()
        {
            base.BindFields();
            m_Scale.BindProperty(m_Props.Hinge.Scale);
            m_Hinge.BindProperty(m_Props.Hinge.Data);
        }
        protected override void DefineFields()
        {
            base.DefineFields();
            m_Scale = new PropertyField(m_Props.Hinge.Scale); // this probably shouldn't be in hinge data.
            m_HingeFoldout = new Foldout() { text = "Hinge" };
            m_Hinge = new PropertyField(m_Props.Hinge.Data);
        }

        protected override void Initialize(SerializedProperty data)
        {
            base.Initialize(data);
            m_Props = new DoorDataSerializedProperties(data);
            m_CurrentData = data.GetUnderlyingValue() as DoorData;
            m_PreviousData = m_CurrentData.Clone() as DoorData;
        }

        protected override void RegisterValueChangeCallbacks()
        {
            base.RegisterValueChangeCallbacks();
            m_Scale.RegisterValueChangeCallback(evt =>
            {
                Vector3 scale = evt.changedProperty.vector3Value;

                if (scale == m_PreviousData.Hinge.Scale)
                    return;

                m_PreviousData.Hinge.Scale = scale;
                m_CurrentData.IsDirty = true;
            });
            m_Hinge.RegisterValueChangeCallback(evt =>
            {
                TransformData data = evt.changedProperty.GetUnderlyingValue() as TransformData;

                if (m_PreviousData.Hinge.Equals(data))
                    return;

                m_PreviousData.Hinge = new TransformData(data);
                m_CurrentData.IsDirty = true;
            });
        }
    }
}
