
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterManipulator : NetworkBehaviour
{
    // Essentially owns a character, which they can then control. Characters can actually be controlled and changed without
    // being a character manipulator, but this is the formal way of doing things.

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
            return _target;
        }
        set
        {
            if (value == _target)
                return;

            if (value == null)
            {
                _target.AssignManipulator(null);
            }
            else
            {
                if (_target != null)
                    _target.AssignManipulator(null);
                value.AssignManipulator(this);
            }
            _target = value;
        }
    }
    [SerializeField]
    private Character _target;

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