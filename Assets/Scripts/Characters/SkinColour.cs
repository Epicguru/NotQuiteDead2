using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinColour : MonoBehaviour
{

    public Color Colour
    {
        get
        {
            return _colour;
        }
        set
        {
            if (_colour == value)
                return;

            _colour = value;
            dirty = true;
        }
    }
    [SerializeField]
    private Color _colour = new Color(1f, 0.8030427f, 0.5424528f);
    public SpriteRenderer[] BodyParts;

    private bool dirty = true;

    public void Update()
    {
        if (BodyParts == null || BodyParts.Length == 0)
            return;

        if (!dirty)
            return;

        foreach (var part in BodyParts)
        {
            if (part == null)
                continue;

            part.color = Colour;
        }

        dirty = false;
    }
}
