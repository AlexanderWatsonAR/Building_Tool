using OnlyInvalid.CustomVisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GridFrameDisplaySettingsProvider : SettingsProvider
{
    public GridFrameDisplaySettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
    {
    }
    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        var display = DisplayDataSettings.Data;

        Label label = new Label("Grid Frame")
        {
            style =
            {
                fontSize = 20,
                unityFontStyleAndWeight = FontStyle.Bold
            }
        };

        var frame = display.GridFrame;
        var columns = new SliderIntDisplayDataField(frame.Columns);
        var rows = new SliderIntDisplayDataField(frame.Rows);
        var depth = new SliderDisplayDataField(frame.Depth);
        var scale = new SliderDisplayDataField(frame.Scale);

        columns.RegisterCallback<ChangeEvent<SliderDisplayData<int>>>(evt => frame.Columns = evt.newValue);
        rows.RegisterCallback<ChangeEvent<SliderDisplayData<int>>>(evt => frame.Rows = evt.newValue);
        depth.RegisterCallback<ChangeEvent<SliderDisplayData<float>>>(evt => frame.Depth = evt.newValue);
        scale.RegisterCallback<ChangeEvent<SliderDisplayData<float>>>(evt => frame.Scale = evt.newValue);

        rootElement.Add(label);
        rootElement.Add(columns);
        rootElement.Add(rows);
        rootElement.Add(depth);
        rootElement.Add(scale);

    }

    [SettingsProvider]
    public static SettingsProvider CreateSettingsProvider()
    {
        return new GridFrameDisplaySettingsProvider("Procedural Building/Display/Grid Frame", SettingsScope.User);
    }

}