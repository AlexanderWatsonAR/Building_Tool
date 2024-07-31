using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;

namespace OnlyInvalid.ProcGenBuilding.Door
{
    [CustomPropertyDrawer(typeof(DoorData))]
    public class DoorDataDrawer : DataDrawer
    {
        DoorDataSerializedProperties m_Props;

        PropertyField m_Depth, m_Scale;
        Foldout m_HingeFoldout;
        PropertyField m_Hinge;

        protected override void AddFieldsToRoot()
        {
            m_Root.Add(m_Depth);
            m_Root.Add(m_Scale);
            m_Root.Add(m_HingeFoldout);
            m_HingeFoldout.Add(m_Hinge);
        }
        protected override void BindFields()
        {
            m_Depth.BindProperty(m_Props.Depth);
            m_Scale.BindProperty(m_Props.Hinge.Scale);
            m_Hinge.BindProperty(m_Props.Hinge.Data);
        }
        protected override void DefineFields()
        {
            m_Depth = new PropertyField(m_Props.Depth);
            m_Scale = new PropertyField(m_Props.Hinge.Scale);
            m_HingeFoldout = new Foldout() { text = "Hinge" };
            m_Hinge = new PropertyField(m_Props.Hinge.Data);
        }
        protected override void Initialize(SerializedProperty data)
        {
            m_Props = new DoorDataSerializedProperties(data);

            ScriptableObject defaults = Resources.Load("Path") as ScriptableObject;
        }
        protected override void RegisterValueChangeCallbacks()
        {
        }
    }
}
