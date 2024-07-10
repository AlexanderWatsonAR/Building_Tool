using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using OnlyInvalid.ProcGenBuilding.Common;
using Unity.VisualScripting.FullSerializer;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [CustomPropertyDrawer(typeof(OpeningDataList), false)]
    public class OpeningDataListDrawer : PropertyDrawer
    {
        SerializedProperty m_Data, m_InnerList;
        VisualElement m_Root;
        List<OpeningDataField> m_OpeningDataFields;
        OpeningDataList m_Openings;

        public override VisualElement CreatePropertyGUI(SerializedProperty data)
        {
            Initialize(data);
            Create();
            return m_Root;
        }

        private void Initialize(SerializedProperty data)
        {
            m_Data = data;
            m_InnerList = m_Data.FindPropertyRelative("m_Openings");
            m_Openings = m_Data.GetUnderlyingValue() as OpeningDataList;
            m_OpeningDataFields = new List<OpeningDataField>();

            m_Root = new VisualElement() { name = nameof(OpeningDataList) + "_Root" };

            m_Root.RegisterCallback<OpeningAddEvent>(evt => 
            {
                AddField(m_OpeningDataFields.Count);
            });

            m_Openings.OnAdd.RemoveListener(SendAddEvent);
            m_Openings.OnAdd.AddListener(SendAddEvent);
        }

        private void Create()
        {
            //CreateFields();
            //CreateContextMenu();
            //RegisterCallbacks();
            //AddToRoot();
            
            for(int i = 0; i < m_InnerList.arraySize; i++)
            {
                AddField(i);
            }
            m_Openings.IsDirty = true;
        }

        private void SendAddEvent()
        {
            using (OpeningAddEvent openingAddEvent = OpeningAddEvent.GetPooled())
            {
                openingAddEvent.target = m_Root;

                EditorApplication.delayCall += () => m_Root.SendEvent(openingAddEvent);

                //m_Root.SendEvent(openingAddEvent);
            }
        }

        private void CreateFields()
        {
            Object target = m_Data.serializedObject.targetObject;

            string names = Extensions.GetParentNames(target.GameObject().transform);

            for (int i = 0; i < m_InnerList.arraySize; i++)
            {
                OpeningDataField field = new(new OpeningDataSerializedProperties(m_InnerList.GetArrayElementAtIndex(i)));
                field.HeaderFoldout.viewDataKey = names + i.ToString() + field.Opening.Name;
                m_OpeningDataFields.Add(field);
            }
        }

        private void AddField(int index)
        {
            Object target = m_Data.serializedObject.targetObject;
            string names = Extensions.GetParentNames(target.GameObject().transform);
            //int index = m_OpeningDataFields.Count;

            OpeningDataField field = new OpeningDataField(new OpeningDataSerializedProperties(m_InnerList.GetArrayElementAtIndex(index)));
            field.HeaderFoldout.viewDataKey = names + index.ToString() + field.Opening.Name;

            #region Context Menu
            field.HeaderFoldout.contextMenu.AddItem("Rename", false, () =>
            {
                field.HeaderFoldout.textField.style.display = DisplayStyle.Flex;
                field.HeaderFoldout.label.style.display = DisplayStyle.None;
                field.HeaderFoldout.textField.Focus();
            });
            field.HeaderFoldout.contextMenu.AddItem("Move Up", false, () =>
            {
                m_Openings.ShiftDown();
            });
            field.HeaderFoldout.contextMenu.AddItem("Move Down", false, () =>
            {
                m_Openings.ShiftUp();
            });
            field.HeaderFoldout.contextMenu.AddSeparator("");
            field.HeaderFoldout.contextMenu.AddItem("Remove", false, () =>
            {
                using (ChangeEvent<int> openingCountEvent = ChangeEvent<int>.GetPooled(m_Openings.Count, m_Openings.Count - 1))
                {
                    openingCountEvent.target = m_Root;
                    m_Openings.IsDirty = true;
                    m_Openings.Remove(field.Opening);
                    m_OpeningDataFields.Remove(field);
                    m_Root.Remove(field);
                    m_Root.SendEvent(openingCountEvent);
                }

            });
            #endregion

            #region Register Callbacks
            field.HeaderFoldout.textField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                {
                    field.HeaderFoldout.text = field.HeaderFoldout.textField.value;
                    field.HeaderFoldout.textField.style.display = DisplayStyle.None;
                    field.HeaderFoldout.label.style.display = DisplayStyle.Flex;
                    field.Opening.Name = field.HeaderFoldout.text;
                }
            });
            field.HeaderFoldout.toggle.RegisterValueChangedCallback(evt => m_Openings.IsDirty = true);
            field.Shape.RegisterCallback<ChangeEvent<Shape>>(evt =>
            {
                field.Props.Shape.SetUnderlyingValue(evt.newValue);

                m_Openings.IsDirty = true;
            });
            field.Position.RegisterValueChangedCallback(evt =>
            {
                if (evt.previousValue == evt.newValue)
                    return;

                if (field.PreviousPosition == evt.newValue)
                    return;

                field.PreviousPosition = evt.newValue;

                m_Openings.IsDirty = true;
            });
            field.Angle.RegisterValueChangedCallback(evt =>
            {
                if (evt.previousValue == evt.newValue)
                    return;

                if (field.PreviousAngle == evt.newValue)
                    return;

                field.PreviousAngle = evt.newValue;

                m_Openings.IsDirty = true;
            });
            field.Scale.RegisterValueChangedCallback(evt =>
            {
                if (evt.previousValue == evt.newValue)
                    return;

                if (field.PreviousScale == evt.newValue)
                    return;

                field.PreviousScale = evt.newValue;

                m_Openings.IsDirty = true;
            });
            field.DropdownField.RegisterValueChangedCallback(evt =>
            {
                m_Openings.IsDirty = true;
            });
            #endregion

            m_OpeningDataFields.Add(field);
            m_Root.Add(field);

        }

        private void CreateContextMenu()
        {
            foreach(var field in m_OpeningDataFields)
            {
                field.HeaderFoldout.contextMenu.AddItem("Rename", false, () =>
                {
                    field.HeaderFoldout.textField.style.display = DisplayStyle.Flex;
                    field.HeaderFoldout.label.style.display = DisplayStyle.None;
                    field.HeaderFoldout.textField.Focus();
                });
                field.HeaderFoldout.contextMenu.AddItem("Move Up", false, () =>
                {
                    m_Openings.ShiftDown();
                });
                field.HeaderFoldout.contextMenu.AddItem("Move Down", false, () =>
                {
                    m_Openings.ShiftUp();
                });
                field.HeaderFoldout.contextMenu.AddSeparator("");
                field.HeaderFoldout.contextMenu.AddItem("Remove", false, () =>
                {
                    using (ChangeEvent<int> openingCountEvent = ChangeEvent<int>.GetPooled(m_Openings.Count, m_Openings.Count-1))
                    {
                        openingCountEvent.target = m_Root;
                        m_Openings.IsDirty = true;
                        m_Openings.Remove(field.Opening);
                        m_OpeningDataFields.Remove(field);
                        m_Root.Remove(field);
                        m_Root.SendEvent(openingCountEvent);
                    }

                });
            }
        }
        private void RegisterCallbacks()
        {
            foreach (var field in m_OpeningDataFields)
            {
                field.HeaderFoldout.textField.RegisterCallback<KeyDownEvent>(evt =>
                {
                    if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                    {
                        field.HeaderFoldout.text = field.HeaderFoldout.textField.value;
                        field.HeaderFoldout.textField.style.display = DisplayStyle.None;
                        field.HeaderFoldout.label.style.display = DisplayStyle.Flex;
                        field.Opening.Name = field.HeaderFoldout.text;
                    }
                });
                field.HeaderFoldout.toggle.RegisterValueChangedCallback(evt => m_Openings.IsDirty = true);
                field.Shape.RegisterCallback<ChangeEvent<Shape>>(evt =>
                {
                    field.Props.Shape.SetUnderlyingValue(evt.newValue);

                    m_Openings.IsDirty = true;
                });
                field.Position.RegisterValueChangedCallback(evt =>
                {
                    if (evt.previousValue == evt.newValue)
                        return;

                    if (field.PreviousPosition == evt.newValue)
                        return;

                    field.PreviousPosition = evt.newValue;

                    m_Openings.IsDirty = true;
                });
                field.Angle.RegisterValueChangedCallback(evt =>
                {
                    if (evt.previousValue == evt.newValue)
                        return;

                    if (field.PreviousAngle == evt.newValue)
                        return;

                    field.PreviousAngle = evt.newValue;

                    m_Openings.IsDirty = true;
                });
                field.Scale.RegisterValueChangedCallback(evt =>
                {
                    if (evt.previousValue == evt.newValue)
                        return;

                    if (field.PreviousScale == evt.newValue)
                        return;

                    field.PreviousScale = evt.newValue;

                    m_Openings.IsDirty = true;
                });
                field.DropdownField.RegisterValueChangedCallback(evt =>
                {
                    m_Openings.IsDirty = true;
                });
            }
        }
        private void AddToRoot()
        {
            foreach(var field in m_OpeningDataFields)
            {
                m_Root.Add(field);
            }
        }
    }
}


