using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem
{
    // An inventory item is the combination of ItemData and some data about position and rotation, a reference to the inventory that contains it.

    public Inventory CurrentInventory { get; set; }
    public ItemData Data { get; set; }
    public Vector2Int Position;
    public bool Rotated;

    public RectInt Space
    {
        get
        {
            return new RectInt(Position, Size);
        }
    }

    public Vector2Int Size
    {
        get
        {
            if (Data == null)
                return Vector2Int.zero;

            if (!Rotated)
            {
                return Data.Dimensions;
            }
            else
            {
                return new Vector2Int(Data.Dimensions.y, Data.Dimensions.x);
            }
        }
    }
}
