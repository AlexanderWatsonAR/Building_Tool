using OnlyInvalid.CustomVisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FrameDisplaySettingsProvider : SettingsProvider
{
    public FrameDisplaySettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
    {
    }
    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        var display = DisplayDataSettings.Data;

        Label label = new Label("Frame")
        {
            style =
            {
                fontSize = 20,
                unityFontStyleAndWeight = FontStyle.Bold
            }
        };

        var frame = display.Frame;
        var depth = new SliderDisplayDataField(frame.Depth);
        var scale = new SliderDisplayDataField(frame.Scale);

        depth.RegisterCallback<ChangeEvent<SliderDisplayData<float>>>(evt => frame.Depth = evt.newValue);
        scale.RegisterCallback<ChangeEvent<SliderDisplayData<float>>>(evt => frame.Scale = evt.newValue);

        rootElement.Add(label);
        rootElement.Add(depth);
        rootElement.Add(scale);
    }

    [SettingsProvider]
    public static SettingsProvider CreateSettingsProvider()
    {
        return new FrameDisplaySettingsProvider("Procedural Building/Display/Frame", SettingsScope.User);
    }
}
