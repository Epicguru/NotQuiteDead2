
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CharacterDirection))]
[RequireComponent(typeof(CharacterMovement))]
public class Character : NetworkBehaviour
{
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

    public CharacterManipulator Manipulator { get; private set; }

    public void AssignManipulator(CharacterManipulator m)
    {
        Manipulator = m;
    }
}
