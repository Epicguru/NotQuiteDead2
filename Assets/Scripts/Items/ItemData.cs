
using Newtonsoft.Json;
using UnityEngine;

public class ItemData
{
    // This class stores data about the current state of an Item.
    public ushort ID;
    public string Name = "Item Name";
    public Vector2Int Dimensions = new Vector2Int(1, 1);

    public string ToJson()
    {
        return GameIO.ObjectToJson(this, Formatting.None, true);
    }

    public static T FromJson<T>(string json) where T : ItemData
    {
        if(string.IsNullOrWhiteSpace(json))
        {
            Debug.LogError("Invalid input json string: null or empty!");
            return null;
        }

        return GameIO.JsonToObject<T>(json);
    }
}
