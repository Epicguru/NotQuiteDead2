using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventoryWindow : MonoBehaviour
{
    public InventoryTests Tests;
    public Inventory Inventory
    {
        get
        {
            return _inventory;
        }
        set
        {
            if (_inventory == value)
                return;
            if(_inventory != null)
            {
                _inventory.UponChange.RemoveListener(UponInventoryChange);
            }
            _inventory = value;
            if(_inventory != null)
            {
                _inventory.UponChange.AddListener(UponInventoryChange);
            }
        }
    }
    private Inventory _inventory;

    public bool IsScrollWindow = true;
    public float TargetVerticalCells = 4;

    public UI_Window Window
    {
        get
        {
            if (_window == null)
                _window = GetComponentInParent<UI_Window>();
            return _window;
        }
    }
    private UI_Window _window;

    public RectTransform Content;
    public RawImage Image;
    public const int HIGHLIGHT_SIZE = 128;
    private List<float> greenCells = new List<float>(HIGHLIGHT_SIZE);
    public Vector2 ContentPadding = new Vector2(0, 0);

    private List<UI_InventoryItem> uiItems = new List<UI_InventoryItem>();

    public void Update()
    {
        if(Inventory == null && Tests != null)
        {
            Inventory = Tests.Inventory;
        }

        if (Inventory == null)
            return;

        if (IsScrollWindow)
        {
            // Set content size.
            Content.sizeDelta = Inventory.Size * UI_InventoryItem.CELL_SIZE + ContentPadding;

            float targetHeight = TargetVerticalCells * UI_InventoryItem.CELL_SIZE;

            // Set the width of the window to the target size.
            Window.ContentSize = new Vector2(Content.sizeDelta.x, targetHeight);
        }
        else
        {
            // Set content size.
            Content.sizeDelta = Inventory.Size * UI_InventoryItem.CELL_SIZE + ContentPadding;

            // Just make the window match the content size.
            Window.ContentSize = Inventory.Size * UI_InventoryItem.CELL_SIZE + ContentPadding;
        }

        var mat = Image.material;
        if (mat == null)
            return;
        mat.SetInt("_SizeX", Inventory.Size.x);
        mat.SetInt("_SizeY", Inventory.Size.y);

        greenCells.Clear();
        foreach(var item in Inventory.Items)
        {
            foreach(var point in item.Space.allPositionsWithin)
            {
                greenCells.Add(GetIndex(point.x, point.y, Inventory.Size.x));
            }
        }
        while (greenCells.Count < HIGHLIGHT_SIZE)
            greenCells.Add(0);
        mat.SetFloatArray("_GreenCells", greenCells);
    }

    public void UponInventoryChange(Inventory i, InventoryChangeType type, InventoryItem changed)
    {
        if (Inventory == null)
            return;
        if (Inventory != i)
            return;

        if(type == InventoryChangeType.ITEM_ADDED)
        {
            // Add the specific item.
            if(changed != null)
            {
                var item = changed;

                item.UIItem = Pool.Get(Spawnables.I.UIInventoryItem).GetComponent<UI_InventoryItem>();
                item.UIItem.transform.parent = Image.transform;
                item.UIItem.InventoryWindow = this;
                item.UIItem.Item = item;
                item.UIItem.Init();

                uiItems.Add(item.UIItem);
            }
        }
        else if(type == InventoryChangeType.ITEM_REMOVED)
        {
            // Remove the specific item.
            if (changed != null && changed.UIItem != null)
            {
                if (this.uiItems.Contains(changed.UIItem))
                {
                    changed.UIItem.InventoryWindow = null;
                    changed.UIItem.Item = null;
                    uiItems.Remove(changed.UIItem);
                    Pool.Return(changed.UIItem.PoolableObject);
                }
            }
        }
        else
        {
            // Soemthing else happened, but it definitely changed the inventory layout or contents:
            // Just to be safe, refresh all of the items by removing and replacing.

            foreach(var spawned in uiItems)
            {
                Pool.Return(spawned.PoolableObject);
            }
            uiItems.Clear();

            foreach (var item in Inventory.Items)
            {
                if (item.UIItem == null)
                {
                    item.UIItem = Pool.Get(Spawnables.I.UIInventoryItem).GetComponent<UI_InventoryItem>();
                    item.UIItem.transform.parent = Image.transform;
                    item.UIItem.InventoryWindow = this;
                    item.UIItem.Item = item;
                    item.UIItem.Init();

                    uiItems.Add(item.UIItem);
                }
            }
        }
    }

    private float GetIndex(int x, int y, int width)
    {
        return x + y * width + 1;
    }
}
