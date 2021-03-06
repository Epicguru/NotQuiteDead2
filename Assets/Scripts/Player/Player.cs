﻿
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
            if (Instance == null)
                return null;
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

        // Create a character for the player.
        SpawnCharacter();        
    }

    private void SpawnCharacter()
    {
        // Spawn a character for this player. In the future this will load data from disk.
        Character spawned = Instantiate(CharacterPrefab);
        spawned.transform.position = Vector3.zero;
        spawned.transform.rotation = Quaternion.identity;
        Manipulator.Target = spawned;

        // Give a gun.
        Character.Hands.EquipItem(Item.Spawn(1, Vector2.zero));

        // Make the hotbar UI track this character.
        if (UI_Hotbar.Instance != null)
            UI_Hotbar.Instance.Target = Character;
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

        UpdateHotbarInput();
    }
    
    private void UpdateHotbarInput()
    {
        if (Manipulator.Target == null || !Manipulator.Target.HasHandManager)
            return;

        bool p = InputManager.IsDown("Hotbar Primary");
        bool s = InputManager.IsDown("Hotbar Secondary");
        bool b = InputManager.IsDown("Hotbar Backup");
        bool st = InputManager.IsDown("Hotbar Stored");

        var hands = Manipulator.Target.Hands;

        if (p)
        {
            hands.EquipItem(ItemSlot.PRIMARY);
        }
        else if (s)
        {
            hands.EquipItem(ItemSlot.SECONDARY);
        }
        else if (b)
        {
            hands.EquipItem(ItemSlot.BACKUP);
        }
        else if (st)
        {
            hands.EquipItem(ItemSlot.STORED);
        }
    }
}
