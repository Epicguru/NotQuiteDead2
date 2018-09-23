
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetRef))]
public class CharacterManipulator : NetworkBehaviour
{
    // Essentially owns a character, which they can then control. Characters can actually be controlled and changed without
    // being a character manipulator, but this is the formal way of doing things.
    // Normally attached to a player object or next to an AI script.

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
}