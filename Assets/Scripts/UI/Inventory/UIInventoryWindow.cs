using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryWindow : MonoBehaviour
{
    public InventoryTests Tests;
    public Inventory Inventory
    {
        get
        {
            return Tests.Inventory;
        }
    }

    public bool AutoExpand = false;

    public UIWindow Window
    {
        get
        {
            if (_window == null)
                _window = GetComponentInParent<UIWindow>();
            return _window;
        }
    }
    private UIWindow _window;

    public RawImage Image;
    public const int HIGHLIGHT_SIZE = 128;
    private List<float> greenCells = new List<float>(HIGHLIGHT_SIZE);

    public void Update()
    {
        if (Inventory == null)
            return;

        const int GRID_SIZE = 32;

        if (AutoExpand)
        {
            Window.ContentSize = Inventory.Size * GRID_SIZE;
        }
        Image.material.SetInt("_SizeX", Inventory.Size.x);
        Image.material.SetInt("_SizeY", Inventory.Size.y);

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
        Image.material.SetFloatArray("_GreenCells", greenCells);
    }

    private float GetIndex(int x, int y, int width)
    {
        return x + y * width + 1;
    }
}
