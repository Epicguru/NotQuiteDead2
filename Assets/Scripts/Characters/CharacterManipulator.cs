
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetRef))]
public class CharacterManipulator : NetworkBehaviour
{
    // Essentially owns a character, which they can then control. Characters can actually be controlled and changed without
    // being a character manipulator, but this is the formal way of doing things.
    // Normally attached to a player object or next to an AI script.

    public static List<CharacterManipulator> All = new List<CharacterManipulator>();

    public bool IsBot { get; set; }
    public bool IsPlayer
    {
        get
        {
            return Player != null;
        }
    }

    public Character Target
    {
        get
        {
            var value = CharacterRef.Value;
            if (value == null)
                return null;
            return value.GetComponent<Character>();
        }
        set
        {
            // Should only be called on the server!
            CharacterRef.SetReferenceObj(value);
        }
    }

    public NetRef CharacterRef
    {
        get
        {
            if (_ref == null)
                _ref = GetComponent<NetRef>();
            return _ref;
        }
    }
    private NetRef _ref;

    public Player Player
    {
        get
        {
            if (_player == null)
                _player = GetComponentInParent<Player>();
            return _player;
        }
    }
    private Player _player;

    public void Awake()
    {
        if (!All.Contains(this))
            All.Add(this);
    }

    public void OnDestroy()
    {
        if(All.Contains(this))
            All.Remove(this);
    }

    public Vector2 MovementDirection
    {
        get
        {
            return Target.Movement.NormalizedInputDirection;
        }
        set
        {
            Target.Movement.NormalizedInputDirection = value;
        }
    }

    public float MovementSpeed
    {
        get
        {
            return Target.Movement.CurrentSpeed;
        }
        set
        {
            Target.Movement.CurrentSpeed = value;
        }
    }

    public static CharacterManipulator GetOwnerOf(Character c)
    {
        // Warning - this works, but is very expensive.
        foreach (var item in All)
        {
            if (item.Target == c)
                return item;
        }

        return null;
    }
}