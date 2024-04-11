using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;

namespace OnlyInvalid.ProcGenBuilding.Door
{
    [CustomPropertyDrawer(typeof(DoorData))]
    public class DoorDataDrawer : PropertyDrawer, IFieldInitializer
    {
        [SerializeField] DoorData m_CurrentData;
        [SerializeField] DoorData m_PreviousData;
        DoorDataSerializedProperties m_Props;

        VisualElement m_Root;
        PropertyField m_Scale;
        Foldout m_HingeFoldout;
        PropertyField m_Hinge;

        public void AddFieldsToRoot()
        {
            m_Root.Add(m_Scale);
            m_Root.Add(m_HingeFoldout);
            m_HingeFoldout.Add(m_Hinge);
        }

        public void BindFields()
        {
            m_Scale.BindProperty(m_Props.Hinge.Scale);
            m_Hinge.BindProperty(m_Props.Hinge.Data);
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty data)
        {
            Initialize(data);
            DefineFields();
            BindFields();
            RegisterValueChangeCallbacks();
            AddFieldsToRoot();
            return m_Root;
        }

        public void DefineFields()
        {
            m_Root = new VisualElement() { name = nameof(DoorData) + "_Root" };
            m_Scale = new PropertyField(m_Props.Hinge.Scale); // this probably shouldn't be in hinge data.
            m_HingeFoldout = new Foldout() { text = "Hinge" };
            m_Hinge = new PropertyField(m_Props.Hinge.Data);
        }

        public void Initialize(SerializedProperty data)
        {
            m_Props = new DoorDataSerializedProperties(data);
            m_CurrentData = data.GetUnderlyingValue() as DoorData;
            m_PreviousData = m_CurrentData.Clone() as DoorData;
        }

        public void RegisterValueChangeCallbacks()
        {
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
