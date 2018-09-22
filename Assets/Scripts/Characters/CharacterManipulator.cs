
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CharacterReference))]
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

    private CharacterReference CharacterReference
    {
        get
        {
            if (_cr == null)
                _cr = GetComponent<CharacterReference>();
            return _cr;
        }
    }
    private CharacterReference _cr;

    public Character Target
    {
        get
        {
            return CharacterReference.GetCharacter();
        }
        set
        {
            // This should only ever be called on the server, but it should break anything major if it is called on a client.
            // However it obviously won't work on a client.

            var current = Target;

            if (value == current)
                return;

            if (value == null)
            {
                current.AssignManipulator(null);
            }
            else
            {
                if (current != null)
                    current.AssignManipulator(null);
                value.AssignManipulator(this);
            }
            Debug.Log("Set to " + value.netId.Value);
            CharacterReference.SetCharacter(value);
        }
    }

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