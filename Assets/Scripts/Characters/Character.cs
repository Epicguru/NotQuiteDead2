﻿
using UnityEngine;

[RequireComponent(typeof(CharacterDirection))]
[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(Health))]
public class Character : MonoBehaviour
{
    // A character is a humanoid actor in the world.
    // A character can do everything the player can do - because the player object is just another character.
    // Characters are controlled and owned by CharacterManipulators.

    // The character manipulator and character relationship only exists in it's real state on the server: clients may not have
    // access to this data. Furthermore all actions performed by the CharacterManipulator should be on the server, and
    // then synchronized to the clients by individual components.

    public CharacterDirection Direction
    {
        get
        {
            if (_direction == null)
                _direction = GetComponent<CharacterDirection>();
            return _direction;
        }
    }
    private CharacterDirection _direction;

    public CharacterMovement Movement
    {
        get
        {
            if (_movement == null)
                _movement = GetComponent<CharacterMovement>();
            return _movement;
        }
    }
    private CharacterMovement _movement;

    public CharacterHandManager Hands
    {
        get
        {
            if (_hands == null)
                _hands = GetComponent<CharacterHandManager>();
            return _hands;
        }
    }
    private CharacterHandManager _hands;

    public Health Health
    {
        get
        {
            if (_health == null)
                _health = GetComponent<Health>();
            return _health;
        }
    }
    private Health _health;

    public Item ToEquip;

    public bool HasHandManager
    {
        get
        {
            return Hands != null;
        }
    }

    public CharacterManipulator Manipulator
    {
        get
        {
            if(_manipulator == null)
            {
                _manipulator = CharacterManipulator.GetOwnerOf(this);
            }
            else
            {
                if(_manipulator.Target != this)
                {
                    _manipulator = CharacterManipulator.GetOwnerOf(this);
                }
            }

            return _manipulator;
        }
    }
    private CharacterManipulator _manipulator;
    public bool IsControlled
    {
        get
        {
            return Manipulator != null;
        }
    }

    /// <summary>
    /// The same as Character.Health.IsDead
    /// </summary>
    public bool IsDead
    {
        get
        {
            return Health.IsDead;
        }
    }
}
