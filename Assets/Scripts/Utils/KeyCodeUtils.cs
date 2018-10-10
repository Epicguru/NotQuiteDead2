
using UnityEngine;

public static class KeyCodeUtils
{
    public static string GetNiceName(this KeyCode key)
    {
        string baseName = key.ToString();
        const string ALPHA = "Alpha";
        if (baseName.StartsWith(ALPHA))
        {
            return baseName.Substring(ALPHA.Length);
        }

        return baseName;
    }
}
