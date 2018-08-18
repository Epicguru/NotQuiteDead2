using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : ScriptableObject
{
    // An inventory holds InventoryItems which contain ItemData and some positioning and references.
    // Inventories have limited space, and are a grid-like structure. Inventories can be fully serialized.

    public List<InventoryItem> Items = new List<InventoryItem>();

    public Vector2Int Size = new Vector2Int(10, 10);
    public Vector2Int TempItem = new Vector2Int(1, 1);
    public Vector2Int TempPos = new Vector2Int(0, 0);
    public bool TempRotation = false;

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
        return GameIO.ObjectToJson(this, Formatting.None, true);
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
