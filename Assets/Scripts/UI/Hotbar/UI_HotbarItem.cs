using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PoolableObject))]
public class UI_HotbarItem : MonoBehaviour
{
    public PoolableObject PoolableObject
    {
        get
        {
            if (_po == null)
                _po = GetComponent<PoolableObject>();
            return _po;
        }
    }
    private PoolableObject _po;

    public Sprite Icon
    {
        get
        {
            return _icon;
        }
        set
        {
            if (value == _icon)
                return;
            _icon = value;

            if (Image.sprite != Icon)
            {
                // Work out width and height.
                // Keep the original width, with a cap.
                const float WIDTH_CAP = 250f;

                float w = Mathf.Min(Icon.textureRect.width, WIDTH_CAP);
                const float h = 50f;

                ImageRect.sizeDelta = new Vector2(w, h);
                Image.preserveAspect = true;
                Image.sprite = Icon;
            }
        }
    }
    [Header("Data")]
    [ReadOnly]
    [SerializeField]
    private Sprite _icon;

    public string Key
    {
        get
        {
            return _key;
        }
        set
        {
            if (value == _key)
                return;

            _key = value;
            if (KeyText.text != Key)
                KeyText.text = Key;
        }
    }
    private string _key;

    [Header("References")]
    public Text KeyText;
    public Image Image;
    public RectTransform ImageRect;
}
