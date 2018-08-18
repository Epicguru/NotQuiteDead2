using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(PoolableObject))]
public class UIWindow : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField]
    private Text titleText;
    [SerializeField]
    private RectTransform content;

    public PoolableObject PoolableObject
    {
        get
        {
            if (_poolable == null)
                _poolable = GetComponent<PoolableObject>();
            return _poolable;
        }
    }
    private PoolableObject _poolable;

    public Vector2 ContentSize
    {
        get
        {
            return content.rect.size;
        }
        set
        {
            if (ContentSize == value)
                return;

            Vector2 difference = Size - ContentSize;
            Size = value + difference;
        }
    }

    public Vector2 Size
    {
        get
        {
            return ((RectTransform)transform).sizeDelta;
        }
        set
        {
            ((RectTransform)transform).sizeDelta = value;
        }
    }

    [Header("Controls")]
    public Vector2 MinSize = new Vector2(120, 150);
    public Vector2 MaxSize = new Vector2(600, 800);

    public Vector2 TargetSize
    {
        get
        {
            return _targetSize;
        }
        set
        {
            if (value == _targetSize)
                return;

            if (value.x < MinSize.x)
                value.x = MinSize.x;
            if (value.y < MinSize.y)
                value.y = MinSize.y;

            if (value.x > MaxSize.x)
                value.x = MaxSize.x;
            if (value.y > MaxSize.y)
                value.y = MaxSize.y;

            _targetSize = value;
            UpdateSize();
        }
    }
    private Vector2 _targetSize = new Vector2(500, 500);

    public string Title
    {
        get
        {
            return titleText.text;
        }
        set
        {
            if (titleText.text != value)
                titleText.text = value;
        }
    }

    private Vector2 lastPointerPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastPointerPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var rtf = (RectTransform)transform;

        Vector2 change = eventData.position - lastPointerPos;
        rtf.anchoredPosition += change;
        lastPointerPos = eventData.position;

        KeepOnScreen();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        KeepOnScreen();
    }

    public Rect Bounds
    {
        get
        {
            var rtf = (RectTransform)transform;

            var point = rtf.anchoredPosition;
            var size = rtf.rect.size;

            Rect bounds = new Rect(point, size);

            return bounds;
        }
        private set
        {
            var rtf = (RectTransform)transform;
            rtf.anchoredPosition = value.position;
            rtf.sizeDelta = value.size;
        }
    }

    public void KeepOnScreen()
    {
        var bounds = Bounds;

        int minX = 5;
        int minY = 5;
        int maxX = Screen.width - 5;
        int maxY = Screen.height - 5;

        if(bounds.x < minX)
        {
            bounds.x = minX;
        }
        if (bounds.y < minY)
        {
            bounds.y = minY;
        }
        if(bounds.xMax > maxX)
        {
            bounds.x = maxX - bounds.width;
        }
        if (bounds.yMax > maxY)
        {
            bounds.y = maxY - bounds.height;
        }

        Bounds = bounds;
    }    

    public void UpdateSize()
    {
        ((RectTransform)transform).sizeDelta = TargetSize;
        KeepOnScreen();
    }
}
