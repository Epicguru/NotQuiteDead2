using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterDirection : NetworkBehaviour
{
    public bool Right
    {
        get
        {
            return _right;
        }
        set
        {
            if (value != _right)
            {
                _right = value;
                UpdateScale(_right);
            }
        }
    }
    private bool _right = true;

    public float ScaleMagnitude = 1f;

    public void Start()
    {
        UpdateScale(Right);
    }

    public void UpdateScale(bool right)
    {
        var s = transform.localScale;
        s.x = Mathf.Abs(ScaleMagnitude) * (Right ? 1f : -1f);
        transform.localScale = s;
    }
}
