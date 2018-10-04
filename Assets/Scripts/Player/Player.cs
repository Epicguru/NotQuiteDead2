
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterManipulator))]
[RequireComponent(typeof(PlayerPickup))]
public class Player : MonoBehaviour
{
    // This is NOT a character!
    // The player class sits on an empty game object and is created when the player starts the game session.
    // It can control characters: normally it only controls it's own player object.

    public static Player Instance;
    public static Character Character
    {
        get
        {
            return Instance.Manipulator.Target;
        }
    }

    [SerializeField]
    public string Name = "Bob";
    public Character CharacterPrefab;

    public CharacterManipulator Manipulator
    {
        get
        {
            if (_manipulator == null)
                _manipulator = GetComponent<CharacterManipulator>();
            return _manipulator;
        }
    }
    private CharacterManipulator _manipulator;

    public PlayerPickup PlayerPickup
    {
        get
        {
            if (_pickup == null)
                _pickup = GetComponent<PlayerPickup>();
            return _pickup;
        }
    }
    private PlayerPickup _pickup;

    public void Awake()
    {
        Instance = this;

        // Spawn a character for this player. In the future this will load data from disk.
        Character spawned = Instantiate(CharacterPrefab);
        spawned.transform.position = Vector3.zero;
        spawned.transform.rotation = Quaternion.identity;
        Manipulator.Target = spawned;

        Character.Hands.EquipItem(Item.Spawn(1, Vector2.zero));
    }

    public void Update()
    {
        this.name = Name;

        var c = Manipulator.Target;
        if(c != null && MainCamera.Target != c)
        {
            MainCamera.Target = c.transform;
        }

        // If we have a target to control, normally the player object...
        if(Manipulator.Target != null)
        {
            // Get keyboard input...
            Vector2 rawInput = Vector2.zero;

            if (InputManager.IsPressed("Right"))
                rawInput.x += 1;
            if (InputManager.IsPressed("Left"))
                rawInput.x -= 1;
            if (InputManager.IsPressed("Up"))
                rawInput.y += 1;
            if (InputManager.IsPressed("Down"))
                rawInput.y -= 1;

            // Send this raw input to the movement controller.
            Manipulator.MovementDirection = rawInput;
        }
    }
}
