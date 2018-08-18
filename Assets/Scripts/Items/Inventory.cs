using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : ScriptableObject
{
    // An inventory holds InventoryItems which contain ItemData and some positioning and references.
    // Inventories have limited space, and are a grid-like structure. Inventories can be fully serialized.

    public List<InventoryItem> Items = new List<InventoryItem>();

    public Vector2Int Size = new Vector2Int(5, 5);

    [JsonIgnore]
    public ItemData OriginalData;
    [JsonIgnore]
    public Vector2Int TempPos = new Vector2Int(0, 0);
    [JsonIgnore]
    public bool TempRotation = false;

    [JsonIgnore]
    public InventoryChangeEvent UponChange = new InventoryChangeEvent();

    private List<InventoryItem> tempItems = new List<InventoryItem>();

    public InventoryItem GetItem(ushort id)
    {
        // Finds the first occurence of item of ID, or returns null if none could be found.
        foreach (var item in Items)
        {
            if (item.Data.ID == id)
                return item;
        }

        return null;
    }

    public List<InventoryItem> GetItems(ushort id)
    {
        // Returns a list of items of Id within the inventory.
        // The item list MUST be coppied if it will be in use beyond the immediate future.

        tempItems.Clear();
        foreach (var item in Items)
        {
            if(item.Data.ID == id)
            {
                tempItems.Add(item);
            }
        }

        return tempItems;
    }

    public void RemoveItem(InventoryItem item)
    {
        if (item == null)
            return;

        if (!Items.Contains(item))
            return;

        Items.Remove(item);
        item.CurrentInventory = null;

        UponChange.Invoke(this, InventoryChangeType.ITEM_REMOVED, item);
    }

    public bool MoveItem(InventoryItem item, Vector2Int newPos, bool rotated)
    {
        if (item == null)
            return false;

        if (!Items.Contains(item))
            return false;

        RectInt bounds = new RectInt(newPos, new Vector2Int(rotated ? item.Data.Dimensions.y : item.Data.Dimensions.x, rotated ? item.Data.Dimensions.x : item.Data.Dimensions.y));

        if (CanFit(bounds))
        {
            item.Position = newPos;
            item.Rotated = rotated;

            UponChange.Invoke(this, InventoryChangeType.ITEM_MOVED, item);

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool InsertItem(ItemData data, Vector2Int position, bool rotated)
    {
        // Make new InventoryItem...

        if(data == null)
        {
            Debug.LogError("Null itemdata, cannot add item!");
            return false;
        }

        if (!SpaceInBounds(position.x, position.y))
        {
            Debug.LogError("Inventory position {0} is out of this inventory's bounds! ({1})".Form(position, Size));
            return false;
        }

        var size = data.Dimensions;
        RectInt space = new RectInt(position, size);
        if (rotated)
            space = space.Rotated();

        if (CanFit(space))
        {
            // Place inside the inventory.
            InventoryItem item = new InventoryItem();
            item.CurrentInventory = this;
            item.Data = data;
            item.Rotated = rotated;
            item.Position = position;

            Items.Add(item);

            if(UponChange != null)
            {
                UponChange.Invoke(this, InventoryChangeType.ITEM_ADDED, item);
            }

            return true;
        }
        else
        {
            Debug.LogError("Space {0} inside this inventory is occupied or cannot fit the item. {1}".Form(space, rotated ? "(Item was rotated)" : ""));
            return false;
        }
    }

    public bool SpaceInBounds(int x, int y)
    {
        return x >= 0 && x < Size.x && y >= 0 && y < Size.y;
    }

    public bool SpaceOccupied(int x, int y)
    {
        if (!SpaceInBounds(x, y))
            return true;

        // Loop through the items, find the space they occupy, check the space.

        Vector2Int pos = new Vector2Int(x, y);
        foreach (var item in Items)
        {
            var space = item.Space;

            if (space.Contains(pos))
            {
                return true;
            }
        }

        return false;
    }

    public bool CanFit(RectInt bounds)
    {
        // Can bounds be placed in the inventory without intersecting something?

        if (!CanBasicFit(bounds.size))
            return false;

        if (!SpaceInBounds(bounds.min.x, bounds.min.y))
            return false;
        if (!SpaceInBounds(bounds.max.x - 1, bounds.max.y - 1))
            return false;

        foreach (var item in Items)
        {
            var space = item.Space;
            
            if (bounds.Intersects(space))
                return false;
        }

        return true;
    }

    public bool CanBasicFit(Vector2Int size)
    {
        // Check if on a basic level an item of 'size' can fit inside the inventory, just based on the total size
        // of the inventory.
        if(size.x <= Size.x)
        {
            if (size.y <= Size.y)
            {
                return true;
            }
        }

        if(size.y <= Size.x)
        {
            if(size.x <= Size.y)
            {
                return true;
            }
        }

        return false;
    }

    public string ToJson()
    {
        return GameIO.ObjectToJson(this, Formatting.Indented, true);
    }

    public void MergeJson(string json)
    {
        Items.Clear();
        JsonConvert.PopulateObject(json, this);

        if (UponChange != null)
        {
            UponChange.Invoke(this, InventoryChangeType.INVENTORY_REFRESH, null);
        }
    }

    public void Dispose()
    {
        UponChange.Invoke(this, InventoryChangeType.INVENTORY_DISPOSED, null);
        UponChange.RemoveAllListeners();
    }

    public static Inventory FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            Debug.LogError("Invalid input json string: null or empty!");
            return null;
        }

        return GameIO.JsonToObject<Inventory>(json);
    }
}

public enum InventoryChangeType : byte
{
    ITEM_ADDED,
    ITEM_REMOVED,
    ITEM_MOVED,
    INVENTORY_REFRESH,
    INVENTORY_DISPOSED
}

public class InventoryChangeEvent : UnityEvent<Inventory, InventoryChangeType, InventoryItem>
{

}
