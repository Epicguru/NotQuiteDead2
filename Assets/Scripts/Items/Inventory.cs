using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    // An inventory holds InventoryItems which contain ItemData and some positioning and references.
    // Inventories have limited space, and are a grid-like structure. Inventories can be fully serialized.

    public List<InventoryItem> Items = new List<InventoryItem>();

    public Vector2Int Size = new Vector2Int(10, 10);

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

        foreach (var item in Items)
        {
            var space = item.Space;

            if (space.Intersects(bounds))
                return false;
        }

        return true;
    }

    public bool CanBasicFit(Vector2Int size)
    {
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
