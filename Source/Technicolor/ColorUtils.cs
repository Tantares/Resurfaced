using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Technicolor
{
  
  public static class ParsedMultiColor
  {
    private enum ColorType
    {
      RGB,
      RGB255,
      RGBA,
      RGBA255,
      HEX,
      WORD,
      None,
    }

    private static Dictionary<string, Color> validColors = new();


    public static Color Parse(string colorString)
    {
      if (colorString == string.Empty) throw new FormatException($"[ColorUtils] Could not parse {colorString} to a valid Color");

      return ParseColor(colorString);
    }


    static ColorType GetColorType(string colorString)
    {
      // Easy to check this one
      if (colorString[0] == '#') return ColorType.HEX;

      // Split by commas, remove whitespace
      char[] comma = { ',' };
      string[] elements = colorString.Split(comma, StringSplitOptions.RemoveEmptyEntries);
      for (int i = 0; i < elements.Length; i++)
      {
        elements[i] = elements[i].Trim();
      }
      // highly scientific way of detecting floats
      if (elements[0].Contains('.'))
      {
        if (float.TryParse(elements[0], out float testFloat))
        {
          if (testFloat >= 0 && testFloat <= 1.0)
          {
            if (elements.Length == 3) return ColorType.RGB;
            if (elements.Length == 4) return ColorType.RGBA;
          }
        }
      }

      if (int.TryParse(elements[0], out int testInt))
      {
        if (testInt >= 0 && testInt <= 255)
        {
          if (elements.Length == 3) return ColorType.RGB255;
          if (elements.Length == 4) return ColorType.RGBA255;
        }
      }

      if (elements.Length == 1) return ColorType.WORD;
      return ColorType.None;
    }

    private static Color ParseColor(string colorString)
    {
      
      ColorType colorType = GetColorType(colorString);
      // use the BAD COLOR
      Color outColor = Color.magenta;
      char[] comma = { ','};

      // Split by commas, remove whitespace
      string[] elements = colorString.Split(comma, StringSplitOptions.RemoveEmptyEntries);
      for (int i = 0; i < elements.Length; i++)
      {
        elements[i] = elements[i].Trim();
      }

      if (colorType == ColorType.WORD)
      {
        // Get the valid colors
        if (validColors.Count == 0) validColors = GetValidNamedColors();

        if (validColors.ContainsKey(colorString))
        {
          outColor = validColors[colorString];
          return outColor;
        }
      }

      // Parse the numerics
      if (colorType == ColorType.RGB)
      {
        for (int i = 0; i < elements.Length; i++)
        {
          if (float.TryParse(elements[i], out float result))
          {
            outColor[i] = result;
          }
        }
        outColor.a = 1f;
        return outColor;
      }
      if (colorType == ColorType.RGBA)
      {
        for (int i = 0; i < elements.Length; i++)
        {
          if (float.TryParse(elements[i], out float result))
          {
            outColor[i] = result;
          }
        }
        return outColor;
      }
      if (colorType == ColorType.RGB255)
      {
        for (int i = 0; i < elements.Length; i++)
        {
          if (int.TryParse(elements[i], out int result))
          {
            outColor[i] = result/255.0f;
          }
        }
        outColor.a = 1f;
        return outColor;
      }
      if (colorType == ColorType.RGBA255)
      {
        for (int i = 0; i < elements.Length; i++)
        {
          if (int.TryParse(elements[i], out int result))
          {
            outColor[i] = result / 255.0f;
          }
        }
        return outColor;
      }
      if (colorType == ColorType.HEX)
      {
        HexToColor(colorString);
      }
      return outColor;
    }
    private static Color HexToColor(string hexColor)
    {
      hexColor = hexColor.Substring(1);

      if (hexColor.Length != 6)
      {
        Debug.LogError("Invalid hex color format. Expected 6 characters.");
        return Color.magenta; // Error indicator
      }

      byte r = byte.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
      byte g = byte.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
      byte b = byte.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

      // Normalize to 0-1 and return as UnityEngine.Color
      return new Color(r / 255f, g / 255f, b / 255f, 1.0f);
    }
    private static Dictionary<string, Color> GetValidNamedColors()
    {
      Dictionary<string, Color> validColors = new();
      // valid Color.xyz properties
      foreach (PropertyInfo propertyInfo in typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public))
      {
        if (propertyInfo.PropertyType == typeof(Color) && !validColors.ContainsKey(propertyInfo.Name))
        {
          validColors.Add(propertyInfo.Name, (Color)propertyInfo.GetValue(null, null));
        }
      }
      // valid XKCDcolors.xyz properties
      foreach (PropertyInfo propertyInfo in typeof(XKCDColors).GetProperties(BindingFlags.Static | BindingFlags.Public))
      {
        if (propertyInfo.PropertyType == typeof(Color) && !validColors.ContainsKey(propertyInfo.Name))
        {
          validColors.Add(propertyInfo.Name, (Color)propertyInfo.GetValue(null, null));
        }
      }
      return validColors;
    }


  }
}

