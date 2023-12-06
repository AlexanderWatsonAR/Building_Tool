using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using System.Linq;
using UnityEditor.Build.Reporting;
using Codice.Client.Common;
using System.Runtime.InteropServices.WindowsRuntime;

[CustomPropertyDrawer(typeof(RoofData))]
public class RoofDataDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        VisualElement container = new VisualElement();

        SerializedProperty roofActive = data.FindPropertyRelative("m_IsActive");

        container.Add(FrameOptions(data));
        container.Add(Tile(data));

        return container;
    }

    private VisualElement FrameOptions(SerializedProperty data)
    {
        VisualElement optionsContainer = new VisualElement();
        IBuildable buildable = data.serializedObject.targetObject as IBuildable;
        RoofData roofData = data.GetUnderlyingValue() as RoofData;

        int[] frames = roofData.AvailableFrames;

        if (frames.Length <= 0)
            return optionsContainer;

        SerializedProperty frameType = data.FindPropertyRelative("m_RoofType");

        int index = 0;
        int value = (int)frameType.GetEnumValue<RoofType>();

        string[] allOptions = frameType.enumNames;
        string[] allOptionsDisplay = frameType.enumDisplayNames; // "Gable", "Mansard", "Flat", "Dormer", "M Shaped", "Pyramid", "Pyramid Hip"
        string[] optionsDisplay = new string[frames.Length];

        for (int i = 0; i < frames.Length; i++)
        {
            optionsDisplay[i] = allOptionsDisplay[frames[i]];
            //options[i] = allOptions[frames[i]];

            if (frames[i] == value)
            {
                index = i;
            }
        }

        PopupField<string> frameTypeField = new PopupField<string>("Frame Type", optionsDisplay.ToList(), index);
        //frameTypeField.BindProperty(frameType); // Issue: Once we bind the field to the property, we no longer have a custom choice selection. 

        optionsContainer.Add(frameTypeField);
        optionsContainer.Add(DisplayRoof(data, (RoofType) index));

        frameTypeField.RegisterValueChangedCallback(evt =>
        {
            optionsContainer.Clear();
            optionsContainer.Add(frameTypeField);

            int enumValueIndex = -1;

            for (int i = 0; i < allOptionsDisplay.Length; i++)
            {
                if (evt.newValue != null && evt.newValue == allOptionsDisplay[i])
                {
                    enumValueIndex = i;
                    break;
                }
            }

            frameType.SetEnumValue((RoofType)enumValueIndex);
            data.serializedObject.ApplyModifiedProperties();

            optionsContainer.Add(DisplayRoof(data, (RoofType)enumValueIndex));

            buildable.Build();
        });

        return optionsContainer;
    }
    private VisualElement DisplayRoof(SerializedProperty data, RoofType roofType)
    {
        VisualElement container = new VisualElement();
        switch (roofType)
        {
            case RoofType.Gable:
                DisplayGable(data, container);
                break;
            case RoofType.Mansard:
                DisplayMansard(data, container);
                break;
            case RoofType.Dormer:
                DisplayMansard(data, container);
                DisplayGable(data, container);
                break;
            case RoofType.MShaped:
                DisplayMShaped(data, container);
                break;
            case RoofType.Pyramid:
                DisplayPyramid(data, container);
                break;
            case RoofType.PyramidHip:
                DisplayMansard(data, container);
                DisplayPyramid(data, container);
                break;
        }
        return container; 
    }
    private void DisplayGable(SerializedProperty data, VisualElement container)
    {
        RoofData roofData = data.GetUnderlyingValue() as RoofData;
        IBuildable buildable = data.serializedObject.targetObject as IBuildable;

        SerializedProperty gableHeight = data.FindPropertyRelative("m_GableHeight");
        SerializedProperty gableScale = data.FindPropertyRelative("m_GableScale");
        SerializedProperty isGableOpen = data.FindPropertyRelative("m_IsOpen");

        Foldout foldout = new Foldout() { text = "Gable"};
        PropertyField height = new PropertyField(gableHeight) { label = "Height"};
        height.BindProperty(gableHeight);
        height.RegisterValueChangeCallback(evt =>
        {
            if (roofData.GableTiles == null)
                return;

            bool rebuild = false;

            for (int i = 0; i < roofData.GableTiles.Length; i++)
            {
                if(roofData.GableTiles[i].Height != evt.changedProperty.floatValue)
                    rebuild = true;

                roofData.GableTiles[i].Height = evt.changedProperty.floatValue;
            }

            for (int i = 0; i < roofData.Walls.Length; i++)
            {
                if (roofData.Walls[i].Height != evt.changedProperty.floatValue)
                    rebuild = true;

                roofData.Walls[i].Height = evt.changedProperty.floatValue;
            }

            if (rebuild)
                buildable.Build();
        });
        PropertyField scale = new PropertyField(gableScale) { label = "Scale"};
        scale.BindProperty(gableScale);
        scale.RegisterValueChangeCallback(evt =>
        {
            if (roofData.GableTiles == null)
                return;

            for (int i = 0; i < roofData.GableTiles.Length; i++)
            {
                roofData.GableTiles[i].ControlPoints[1].T = evt.changedProperty.floatValue;
                roofData.GableTiles[i].ControlPoints[2].T = evt.changedProperty.floatValue;
            }

            buildable.Build();
        });
        PropertyField isOpen = new PropertyField(isGableOpen) { label = "Is Open" };
        isOpen.BindProperty(isGableOpen);
        isOpen.RegisterValueChangeCallback(evt =>
        {
            if (roofData.GableTiles == null)
                return;

            if(evt.changedProperty.boolValue)
            {
                scale.SetEnabled(false);

                for(int i = 0; i < roofData.GableTiles.Length; i++)
                {
                    roofData.GableTiles[i].ControlPoints[1].T = 1;
                    roofData.GableTiles[i].ControlPoints[2].T = 1;
                }
            }
            else
            {
                scale.SetEnabled(true);
                for (int i = 0; i < roofData.GableTiles.Length; i++)
                {
                    roofData.GableTiles[i].ControlPoints[1].T = roofData.GableScale;
                    roofData.GableTiles[i].ControlPoints[2].T = roofData.GableScale;
                }
            }

            buildable.Build();
        });

        foldout.Add(height);
        foldout.Add(scale);
        foldout.Add(isOpen);
        container.Add(foldout);

    }
    private void DisplayMShaped(SerializedProperty data, VisualElement container)
    {
        IBuildable builadable = data.GetUnderlyingValue() as IBuildable;
        SerializedProperty gableHeight = data.FindPropertyRelative("m_GableHeight");
        SerializedProperty isMFlipped = data.FindPropertyRelative("m_IsFlipped");

        Foldout foldout = new Foldout() { text = "M Shaped" };

        PropertyField height = new PropertyField(gableHeight) { label = "Height" };
        height.BindProperty(gableHeight);
        height.RegisterValueChangeCallback(evt =>
        {
            if (evt.changedProperty != null)
                builadable.Build();
        });

        PropertyField isFlipped = new PropertyField(isMFlipped) { label = "Is Flipped" };
        isFlipped.BindProperty(isMFlipped);

        foldout.Add(height);
        foldout.Add(isFlipped);
        container.Add(foldout);
    }
    private void DisplayPyramid(SerializedProperty data, VisualElement container)
    {
        RoofData roofData = data.GetUnderlyingValue() as RoofData;
        IBuildable buildable = data.serializedObject.targetObject as IBuildable;

        SerializedProperty pyramidHeight = data.FindPropertyRelative("m_PyramidHeight");

        Foldout foldout = new Foldout() { text = "Pyramid" };
        PropertyField height = new PropertyField(pyramidHeight) { label = "Height" };
        height.BindProperty(pyramidHeight);
        height.RegisterValueChangeCallback(evt =>
        {
            if (roofData.PyramidTiles == null)
                return;

            for (int i = 0; i < roofData.PyramidTiles.Length; i++)
            {
                roofData.PyramidTiles[i].Height = evt.changedProperty.floatValue;
            }

            buildable.Build();
        });

        foldout.Add(height);
        container.Add(foldout);
    }
    private void DisplayMansard(SerializedProperty data, VisualElement container)
    {
        RoofData roofData = data.GetUnderlyingValue() as RoofData;
        IBuildable buildable = data.serializedObject.targetObject as IBuildable;

        SerializedProperty mansardHeight = data.FindPropertyRelative("m_MansardHeight");
        SerializedProperty mansardScale = data.FindPropertyRelative("m_MansardScale");

        Foldout foldout = new Foldout() { text = "Mansard" };
        PropertyField height = new PropertyField(mansardHeight) { label = "Height" };
        height.BindProperty(mansardHeight);
        height.RegisterValueChangeCallback(evt =>
        {
            if (roofData.MansardTiles == null)
                return;

            for (int i = 0; i < roofData.MansardTiles.Length; i++)
            {
                roofData.MansardTiles[i].Height = evt.changedProperty.floatValue;
            }

            buildable.Build();
        });

        PropertyField scale = new PropertyField(mansardScale) { label = "Scale" };
        scale.BindProperty(mansardScale);
        scale.RegisterValueChangeCallback(evt =>
        {
            if (roofData.MansardTiles == null)
                return;

            for (int i = 0; i < roofData.MansardTiles.Length; i++)
            {
                roofData.MansardTiles[i].ControlPoints[1].T = evt.changedProperty.floatValue;
                roofData.MansardTiles[i].ControlPoints[2].T = evt.changedProperty.floatValue;
            }

            buildable.Build();
        });

        foldout.Add(height);
        foldout.Add(scale);
        container.Add(foldout);
    }
    private VisualElement Tile(SerializedProperty data)
    {
        SerializedProperty roofTileData = data.FindPropertyRelative("m_RoofTileData");
        SerializedProperty thickness = roofTileData.FindPropertyRelative("m_Thickness");
        SerializedProperty extend = roofTileData.FindPropertyRelative("m_Extend");

        RoofData roofData = data.GetUnderlyingValue() as RoofData;
        IBuildable buildable = data.serializedObject.targetObject as IBuildable;

        Foldout foldout = new Foldout() { text = "Tile" };

        PropertyField thicknessField = new PropertyField(thickness);
        thicknessField.BindProperty(thickness);
        thicknessField.RegisterValueChangeCallback(evt => 
        {
            bool rebuildMansard = false;
            bool rebuildGable = false;
            bool rebuildPyramid = false;

            for (int i = 0; i < roofData.MansardTiles.Length; i++)
            {
                if (roofData.MansardTiles[i].Thickness != evt.changedProperty.floatValue)
                    rebuildMansard = true;

                roofData.MansardTiles[i].Thickness = evt.changedProperty.floatValue;
            }

            for (int i = 0; i < roofData.GableTiles.Length; i++)
            {
                if (roofData.GableTiles[i].Thickness != evt.changedProperty.floatValue)
                    rebuildGable = true;

                roofData.GableTiles[i].Thickness = evt.changedProperty.floatValue;
            }

            for (int i = 0; i < roofData.PyramidTiles.Length; i++)
            {
                if (roofData.PyramidTiles[i].Thickness != evt.changedProperty.floatValue)
                    rebuildPyramid = true;

                roofData.PyramidTiles[i].Thickness = evt.changedProperty.floatValue;
            }

            switch (roofData.RoofType)
            {
                case RoofType.Gable:
                    if (rebuildGable)
                        buildable.Build();
                    break;
                case RoofType.Mansard:
                    if (rebuildMansard)
                        buildable.Build();
                    break;
                case RoofType.Dormer:
                    if (rebuildMansard || rebuildGable)
                        buildable.Build();
                    break;
                case RoofType.MShaped:
                    if (rebuildGable)
                        buildable.Build();
                    break;
                case RoofType.Pyramid:
                    if (rebuildPyramid)
                        buildable.Build();
                    break;
                case RoofType.PyramidHip:
                    if (rebuildMansard || rebuildPyramid)
                        buildable.Build();
                    break;
            }

        });

        PropertyField extendField = new PropertyField(extend);
        extendField.BindProperty(extend);
        extendField.RegisterValueChangeCallback(evt =>
        {
            bool rebuildMansard = false;
            bool rebuildGable = false;
            bool rebuildPyramid = false;

            for (int i = 0; i < roofData.MansardTiles.Length; i++)
            {
                if (roofData.MansardTiles[i].Extend != evt.changedProperty.floatValue)
                    rebuildMansard = true;

                roofData.MansardTiles[i].Extend = evt.changedProperty.floatValue;
            }

            for (int i = 0; i < roofData.GableTiles.Length; i++)
            {
                if (roofData.GableTiles[i].Extend != evt.changedProperty.floatValue)
                    rebuildGable = true;

                roofData.GableTiles[i].Extend = evt.changedProperty.floatValue;
            }

            for (int i = 0; i < roofData.PyramidTiles.Length; i++)
            {
                if (roofData.PyramidTiles[i].Extend != evt.changedProperty.floatValue)
                    rebuildPyramid = true;

                roofData.PyramidTiles[i].Extend = evt.changedProperty.floatValue;
            }

            switch (roofData.RoofType)
            {
                case RoofType.Gable:
                    if (rebuildGable)
                        buildable.Build();
                    break;
                case RoofType.Mansard:
                    if (rebuildMansard)
                        buildable.Build();
                    break;
                case RoofType.Dormer:
                    if (rebuildMansard || rebuildGable)
                        buildable.Build();
                    break;
                case RoofType.MShaped:
                    if (rebuildGable)
                        buildable.Build();
                    break;
                case RoofType.Pyramid:
                    if (rebuildPyramid)
                        buildable.Build();
                    break;
                case RoofType.PyramidHip:
                    if (rebuildMansard || rebuildPyramid)
                        buildable.Build();
                    break;
            }

        });

        foldout.Add(thicknessField);
        foldout.Add(extendField);

        return foldout;
    }

}
