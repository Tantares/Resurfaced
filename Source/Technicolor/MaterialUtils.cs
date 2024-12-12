using UnityEngine;

namespace Technicolor
{
  static class Extension
  {
    public static bool IsEqualTo(this Color me, Color other)
    {
      return me.r == other.r && me.g == other.g && me.b == other.b && me.a == other.a;
    }
  }
  public static class MaterialUtils
  {
    public static string[] ColorToStringArray(Color c)
    {
      string[] newArr = new string[4];
      newArr[0] = c.r.ToString();
      newArr[1] = c.g.ToString();
      newArr[2] = c.b.ToString();
      newArr[3] = c.a.ToString();
      return newArr;
    }
  }
}