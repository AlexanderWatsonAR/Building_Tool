using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Color Extraction/Extractor")]
public class ColourExtractor : ScriptableObject
{
    public Texture2D texture;
    public List<ColourCoordinate> uniqueColors;

    [System.Serializable]
    public class ColourCoordinate
    {
        public Color color;
        public Vector2 uv;
        public string name;
    }

    public bool DoesColourExistInPalette(Color color)
    {
        if(uniqueColors.Count == 0)
            return false;

        if(uniqueColors.FirstOrDefault(x => x.color.r == color.r && x.color.g == color.g && x.color.b == color.b) != null)
            return true;

        return false;
    }

    public ColourCoordinate GetColourCoordinate(Color color)
    {
        return uniqueColors.FirstOrDefault(x => x.color.r == color.r && x.color.g == color.g && x.color.b == color.b);
    }

    public void ExtractColours()
    {
        uniqueColors = new List<ColourCoordinate>();
        var width = texture.width;
        var height = texture.height;

        // Get the pixels from the texture
        Color[] pixels = texture.GetPixels();

        // Create a color library
        Dictionary<Color, string> colorLibrary = new Dictionary<Color, string>()
        {
            {Color.red, "Red"},
            {Color.green, "Green"},
            {Color.blue, "Blue"},
            {Color.yellow, "Yellow"},
            {Color.cyan, "Cyan"},
            {Color.magenta, "Magenta"},
            {Color.white, "White"},
            {Color.black, "Black"},
            {Color.gray, "Gray"},
            {new Color(1, 0.5f, 0), "Orange"}
        };

        // Iterate through the pixels
        for (int i = 0; i < pixels.Length; i++)
        {
            var pixel = pixels[i];
            var uv = new Vector2((i % width) / (float)width, (i / width) / (float)height);
            var found = false;
            // check if the color is already in the list
            for (int j = 0; j < uniqueColors.Count; j++)
            {
                if (uniqueColors[j].color == pixel)
                {
                    found = true;
                    break;
                }
            }
            // If the color is not already in the list, add it
            if (!found)
            {
                var colorCoordinate = new ColourCoordinate { color = pixel, uv = uv };
                if (colorLibrary.TryGetValue(pixel, out string colorName))
                {
                    colorCoordinate.name = colorName;
                }
                else
                {
                    colorCoordinate.name = "Unknown";
                }
                uniqueColors.Add(colorCoordinate);
            }
        }
    }
}
