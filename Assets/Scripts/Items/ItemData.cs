
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "Item Data")]
public class ItemData : ScriptableObject
{
    private static Dictionary<ushort, ItemData> Loaded = new Dictionary<ushort, ItemData>();

    // This class stores data about the current state of an Item.
    public ushort ID;
    public string Name = "Item Name";
    public Vector2Int Dimensions = new Vector2Int(1, 1);

    public StaticItemData Static
    {
        get
        {
            if (!IsOriginal)
                return Loaded[ID].Static;
            else
                return _static;
        }
    }
    [SerializeField]
    [JsonIgnore]
    private StaticItemData _static = new StaticItemData();

    public bool IsOriginal { get; private set; }

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

    public static void LoadAll()
    {
        Loaded.Clear();

        var loaded = Resources.LoadAll<ItemData>("Items/Data");
        foreach(var item in loaded)
        {
            if (Loaded.ContainsKey(item.ID))
            {
                Debug.LogError("Duplicate item data ID: {0} for item '{1}', conflicts with already-loaded item data '{2}'\nItem data will not be added to the loaded list.".Form(item.ID, item.Name, Loaded[item.ID].Name));
            }
            else
            {
                item.IsOriginal = true;
                Loaded.Add(item.ID, item);
            }
        }
    }

    public static void UnloadAll()
    {
        Loaded.Clear();
    }

    public static bool IsLoaded(ushort id)
    {
        return Loaded.ContainsKey(id);
    }

    public static ItemData GetOriginal(ushort id)
    {
        // Gets the original loaded item data. The return value of this should not be modified in any way.
        if (IsLoaded(id))
        {
            return Loaded[id];
        }
        else
        {
            return null;
        }
    }

    public static T CreateNew<T>(ushort id) where T : ItemData
    {
        if (!IsLoaded(id))
        {
            Debug.LogError("Base Item Data for Id {0} was not found, cannot create new instance based off that!".Form(id));
            return default(T);
        }

        // Hackish way to create a copy: seralize and deserialize using json.
        return ItemData.FromJson<T>(Loaded[id].ToJson());
    }
}

[System.Serializable]
public class StaticItemData
{
    // Data about a particular item (item ID) that will never change from instance to instance.
    public Sprite Icon;
}
