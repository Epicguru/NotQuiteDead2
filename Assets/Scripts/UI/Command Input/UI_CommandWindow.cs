
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CommandWindow : MonoBehaviour, IDragHandler
{
    public Image Image;

    private bool showing = false;

    public void Update()
    {
        if(InputManager.IsDown("Command Console"))
        {
            Show();
        }
    }

    public void Show()
    {
        if (showing)
            return;

        foreach (Transform go in transform)
        {
            go.gameObject.SetActive(true);
        }
        Image.enabled = true;
        showing = true;
    }

    public void OnRedX()
    {
        if (!showing)
            return;

        foreach (Transform go in transform)
        {
            go.gameObject.SetActive(false);
        }
        Image.enabled = false;
        showing = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!showing)
            return;

        var delta = eventData.delta;
        var rect = transform as RectTransform;

        rect.anchoredPosition += delta;
    }
}