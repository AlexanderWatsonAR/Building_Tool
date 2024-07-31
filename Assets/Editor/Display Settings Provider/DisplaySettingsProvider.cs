using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class DisplaySettingsProvider : SettingsProvider
{
    public DisplaySettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
    {
    }
    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        Label label = new Label("Display")
        {
            style =
            {
                fontSize = 20,
                unityFontStyleAndWeight = FontStyle.Bold
            }
        };

        rootElement.Add(label);

    }

    [SettingsProvider]
    public static SettingsProvider CreateSettingsProvider()
    {
        return new DisplaySettingsProvider("Procedural Building/Display", SettingsScope.User);
    }

}
