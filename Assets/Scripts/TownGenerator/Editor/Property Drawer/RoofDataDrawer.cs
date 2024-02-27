using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using System.Linq;
using System;

[CustomPropertyDrawer(typeof(RoofData))]
public class RoofDataDrawer : PropertyDrawer
{
    [SerializeField] private RoofDataSerializedProperties m_Props;

    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        VisualElement container = new VisualElement();

        m_Props = new RoofDataSerializedProperties(data);

        container.Add(FrameOptions());
        container.Add(Tile());

        return container;
    }

    private VisualElement FrameOptions()
    {
        VisualElement optionsContainer = new VisualElement();
        IBuildable buildable = m_Props.SerializedObject.targetObject as IBuildable;
        RoofData roofData = m_Props.Data.GetUnderlyingValue() as RoofData;

        int[] frames = roofData.AvailableFrames;

        if (frames.Length <= 0)
            return optionsContainer;

        SerializedProperty frameType = m_Props.Type;

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
        optionsContainer.Add(DisplayRoof((RoofType) index));

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
            m_Props.SerializedObject.ApplyModifiedProperties();

            optionsContainer.Add(DisplayRoof((RoofType)enumValueIndex));

            buildable.Build();
        });

        return optionsContainer;
    }
    private VisualElement DisplayRoof(RoofType roofType)
    {
        VisualElement container = new VisualElement();
        switch (roofType)
        {
            case RoofType.Gable:
                DisplayGable(container);
                break;
            case RoofType.Mansard:
                DisplayMansard(container);
                break;
            case RoofType.Dormer:
                DisplayMansard(container);
                DisplayGable(container);
                break;
            case RoofType.MShaped:
                DisplayMShaped(container);
                break;
            case RoofType.Pyramid:
                DisplayPyramid(container);
                break;
            case RoofType.PyramidHip:
                DisplayMansard(container);
                DisplayPyramid(container);
                break;
        }
        return container; 
    }
    private void DisplayGable(VisualElement container)
    {
        RoofData roofData = m_Props.Data.GetUnderlyingValue() as RoofData;
        IBuildable buildable = m_Props.SerializedObject.targetObject as IBuildable;

        Foldout foldout = new Foldout() { text = "Gable"};
        PropertyField height = new PropertyField(m_Props.GableHeight) { label = "Height"};
        height.BindProperty(m_Props.GableHeight);
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
        PropertyField scale = new PropertyField(m_Props.GableScale) { label = "Scale"};
        scale.BindProperty(m_Props.GableScale);
        scale.RegisterValueChangeCallback(evt =>
        {
            if (roofData.GableTiles == null)
                return;

            bool rebuild = false;

            for (int i = 0; i < roofData.GableTiles.Length; i++)
            {
                if(roofData.GableTiles[i].ControlPoints[1].T != evt.changedProperty.floatValue ||
                   roofData.GableTiles[i].ControlPoints[2].T != evt.changedProperty.floatValue)
                {
                    rebuild = true;
                }

                roofData.GableTiles[i].ControlPoints[1].T = evt.changedProperty.floatValue;
                roofData.GableTiles[i].ControlPoints[2].T = evt.changedProperty.floatValue;
            }

            // Thought: We only want to rebuild the roof here. Instead we are rebuilding the building.
            if(rebuild)
                buildable.Build();

        });
        Toggle isOpen = new Toggle() { label = "Is Open", value = m_Props.IsOpen.boolValue};
        isOpen.BindProperty(m_Props.IsOpen);
        isOpen.RegisterValueChangedCallback(evt =>
        {
            if (evt == null)
                return;

            if (evt.newValue == evt.previousValue)
                return;

            if (roofData.GableTiles == null)
                return;

            if(evt.newValue)
            {
                scale.SetEnabled(false);

                for (int i = 0; i < roofData.GableTiles.Length; i++)
                {
                    roofData.GableTiles[i].ControlPoints[1].T = 1;
                    roofData.GableTiles[i].ControlPoints[2].T = 1;

                    bool[] roofTileExtend = roofData.GableData.extend[roofData.GableTiles[i].ID];

                    roofData.GableTiles[i].ExtendWidthBeginning = roofTileExtend[2];
                    roofData.GableTiles[i].ExtendWidthEnd = roofTileExtend[3];
                }
            }
            else
            {
                scale.SetEnabled(true);
                for (int i = 0; i < roofData.GableTiles.Length; i++)
                {
                    roofData.GableTiles[i].ControlPoints[1].T = roofData.GableScale;
                    roofData.GableTiles[i].ControlPoints[2].T = roofData.GableScale;

                    bool[] roofTileExtend = roofData.GableData.extend[roofData.GableTiles[i].ID];

                    bool extendWidthBeginning = roofData.IsOpen && roofTileExtend[2];
                    bool extendWidthEnd = roofData.IsOpen && roofTileExtend[3];

                    roofData.GableTiles[i].ExtendWidthBeginning = extendWidthBeginning;
                    roofData.GableTiles[i].ExtendWidthEnd = extendWidthEnd;
                }
            }

            buildable.Build();
        });

        foldout.Add(height);
        foldout.Add(scale);
        foldout.Add(isOpen);
        container.Add(foldout);

    }
    private void DisplayMShaped(VisualElement container)
    {
        IBuildable buildable = m_Props.SerializedObject.targetObject as IBuildable;

        Foldout foldout = new Foldout() { text = "M Shaped" };

        PropertyField height = new PropertyField(m_Props.GableHeight) { label = "Height" };
        height.BindProperty(m_Props.GableHeight);
        height.RegisterValueChangeCallback(evt =>
        {
            if (evt.changedProperty != null)
                buildable.Build();
        });

        PropertyField isFlipped = new PropertyField(m_Props.IsFlipped) { label = "Is Flipped" };
        isFlipped.BindProperty(m_Props.IsFlipped);

        foldout.Add(height);
        foldout.Add(isFlipped);
        container.Add(foldout);
    }
    private void DisplayPyramid(VisualElement container)
    {
        RoofData roofData = m_Props.Data.GetUnderlyingValue() as RoofData;
        IBuildable buildable = m_Props.SerializedObject.targetObject as IBuildable;

        Foldout foldout = new Foldout() { text = "Pyramid" };
        PropertyField height = new PropertyField(m_Props.PyramidHeight) { label = "Height" };
        height.BindProperty(m_Props.PyramidHeight);
        height.RegisterValueChangeCallback(evt =>
        {
            if (roofData.PyramidTiles == null)
                return;

            bool rebuild = false;

            for (int i = 0; i < roofData.PyramidTiles.Length; i++)
            {
                if(roofData.PyramidTiles[i].Height != evt.changedProperty.floatValue)
                    rebuild = true;

                roofData.PyramidTiles[i].Height = evt.changedProperty.floatValue;
            }

            if(rebuild)
                buildable.Build();
        });

        foldout.Add(height);
        container.Add(foldout);
    }
    private void DisplayMansard(VisualElement container)
    {
        RoofData roofData = m_Props.Data.GetUnderlyingValue() as RoofData;
        IBuildable buildable = m_Props.SerializedObject.targetObject as IBuildable;

        Foldout foldout = new Foldout() { text = "Mansard" };
        PropertyField height = new PropertyField(m_Props.MansardHeight) { label = "Height" };
        height.BindProperty(m_Props.MansardHeight);
        height.RegisterValueChangeCallback(evt =>
        {
            if (roofData.MansardTiles == null)
                return;

            bool rebuild = false;

            for (int i = 0; i < roofData.MansardTiles.Length; i++)
            {
                if (roofData.MansardTiles[i].Height != evt.changedProperty.floatValue)
                    rebuild = true;

                roofData.MansardTiles[i].Height = evt.changedProperty.floatValue;
            }

            if(rebuild)
                buildable.Build();
        });

        PropertyField scale = new PropertyField(m_Props.MansardScale, "Scale") { label = "Scale" };
        scale.label = "Scale";
        //scale.
        //scale.BindProperty(m_Props.MansardScale);
        scale.RegisterValueChangeCallback(evt =>
        {
            if (roofData.MansardTiles == null)
                return;

            bool rebuild = false;


            for (int i = 0; i < roofData.MansardTiles.Length; i++)
            {
                if(roofData.MansardTiles[i].ControlPoints[1].T != evt.changedProperty.floatValue ||
                roofData.MansardTiles[i].ControlPoints[2].T != evt.changedProperty.floatValue)
                {
                    rebuild = true;
                }

                roofData.MansardTiles[i].ControlPoints[1].T = evt.changedProperty.floatValue;
                roofData.MansardTiles[i].ControlPoints[2].T = evt.changedProperty.floatValue;
            }

            if(rebuild)
                buildable.Build();
        });

        foldout.Add(height);
        foldout.Add(scale);
        container.Add(foldout);
    }
    private VisualElement Tile()
    {
        RoofData roofData = m_Props.Data.GetUnderlyingValue() as RoofData;
        IBuildable buildable = m_Props.SerializedObject.targetObject as IBuildable;

        Foldout foldout = new Foldout() { text = "Tile" };

        PropertyField thicknessField = new PropertyField(m_Props.RoofTile.Thickness);
        thicknessField.BindProperty(m_Props.RoofTile.Thickness);
        thicknessField.RegisterValueChangeCallback(evt => 
        {
            bool rebuildMansard = false;
            bool rebuildGable = false;
            bool rebuildPyramid = false;

            if (roofData.MansardTiles != null)
            {
                for (int i = 0; i < roofData.MansardTiles.Length; i++)
                {
                    if (roofData.MansardTiles[i].Thickness != evt.changedProperty.floatValue)
                        rebuildMansard = true;

                    roofData.MansardTiles[i].Thickness = evt.changedProperty.floatValue;
                }
            }


            if (roofData.GableTiles != null)
            {
                for (int i = 0; i < roofData.GableTiles.Length; i++)
                {
                    if (roofData.GableTiles[i].Thickness != evt.changedProperty.floatValue)
                        rebuildGable = true;

                    roofData.GableTiles[i].Thickness = evt.changedProperty.floatValue;
                }
            }

            if (roofData.PyramidTiles != null)
            {
                for (int i = 0; i < roofData.PyramidTiles.Length; i++)
                {
                    if (roofData.PyramidTiles[i].Thickness != evt.changedProperty.floatValue)
                        rebuildPyramid = true;

                    roofData.PyramidTiles[i].Thickness = evt.changedProperty.floatValue;
                }
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

        PropertyField extendField = new PropertyField(m_Props.RoofTile.Extend);
        extendField.BindProperty(m_Props.RoofTile.Extend);
        extendField.RegisterValueChangeCallback(evt =>
        {
            bool rebuildMansard = false;
            bool rebuildGable = false;
            bool rebuildPyramid = false;

            if (roofData.MansardTiles != null)
            {
                for (int i = 0; i < roofData.MansardTiles.Length; i++)
                {
                    if (roofData.MansardTiles[i].Extend != evt.changedProperty.floatValue)
                        rebuildMansard = true;

                    roofData.MansardTiles[i].Extend = evt.changedProperty.floatValue;
                }
            }

            if (roofData.GableTiles != null)
            {
                for (int i = 0; i < roofData.GableTiles.Length; i++)
                {
                    if (roofData.GableTiles[i].Extend != evt.changedProperty.floatValue)
                        rebuildGable = true;

                    roofData.GableTiles[i].Extend = evt.changedProperty.floatValue;
                }
            }

            if (roofData.PyramidTiles != null)
            {
                for (int i = 0; i < roofData.PyramidTiles.Length; i++)
                {
                    if (roofData.PyramidTiles[i].Extend != evt.changedProperty.floatValue)
                        rebuildPyramid = true;

                    roofData.PyramidTiles[i].Extend = evt.changedProperty.floatValue;
                }
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
