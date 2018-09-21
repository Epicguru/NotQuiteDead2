using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Character))]
public class CharacterDirection : NetworkBehaviour
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
            if (value != _right)
            {
                _right = value;
            }
        }
    }
    [SyncVar] private bool _right = true;

    // Local to every client.
    private bool localRight = false;

    public float ScaleMagnitude = 1f;

    public void Start()
    {
        UpdateScale(Right);
    }

    public void Update()
    {
        if(localRight != Right)
        {
            localRight = Right;
            UpdateScale(Right);
        }
    }

    public void UpdateScale(bool right)
    {
        var s = transform.localScale;
        s.x = Mathf.Abs(ScaleMagnitude) * (Right ? 1f : -1f);
        transform.localScale = s;
    }
}
