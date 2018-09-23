
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CharacterDirection))]
[RequireComponent(typeof(CharacterMovement))]
public class Character : NetworkBehaviour
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

    public Item ToEquip;

    public bool HasHandManager
    {
        get
        {
            return Hands != null;
        }
    }

    public CharacterManipulator Manipulator { get; private set; }
    public bool IsControlled
    {
        get
        {
            return Manipulator != null;
        }
    }

    public void AssignManipulator(CharacterManipulator m)
    {
        Manipulator = m;
    }
}
