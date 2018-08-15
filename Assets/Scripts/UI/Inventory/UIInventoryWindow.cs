using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryWindow : MonoBehaviour
{
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

    public RectTransform Image;


    public void Update()
    {
        const int GRID_SIZE = 32;
        Vector2 contentSize = Window.ContentSize;

        int width = Mathf.FloorToInt(contentSize.ToInt().x / GRID_SIZE);
        int height = Mathf.FloorToInt(contentSize.ToInt().y / GRID_SIZE);

        Image.sizeDelta = new Vector2(width * GRID_SIZE, height * GRID_SIZE);
    }
}
