﻿
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CharacterManipulator))]
public class Player : NetworkBehaviour
{
    // This is NOT a character!
    // The player class sits on an empty game object and is created when a player joins the game.
    // It can control characters: normally it only controls it's own player object.

    public static List<Player> All = new List<Player>();
    public static Player Local;

    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            this._name = value;
            base.name = _name;
        }
    }
    [SyncVar]
    private string _name;

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

    public void Awake()
    {
        if (!All.Contains(this))
        {
            All.Add(this);
        }
    }

    public override void OnStartLocalPlayer()
    {
        // Double ultra check.
        if(isLocalPlayer)
            Local = this;
    }

    public void Update()
    {
        // TODO - how do we send input from client's to the server? Authorative actions, such as direction change,
        // weapon manipulation or item use can only be done on the server, but the remote clients are the players controlling
        // this server - side implementation...

        if (!isServer)
            return;

        if (!isLocalPlayer)
            return;

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
            Manipulator.Target.Movement.NormalizedInputDirection = rawInput;
        }
    }

    public void OnDestroy()
    {
        if(All.Contains(this))
            All.Remove(this);
    }
}
