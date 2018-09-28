using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class CharacterDirection : MonoBehaviour
{
    public Character Character
    {
        get
        {
            if (_character == null)
                _character = GetComponent<Character>();
            return _character;
        }
    }
    private Character _character;

    public bool Right
    {
        get
        {
            return _right;
        }
        set
        {
            if(value != _right)
            {
                _right = value;
                UpdateScale(_right);
            }
        }
    }
    private bool _right;

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
