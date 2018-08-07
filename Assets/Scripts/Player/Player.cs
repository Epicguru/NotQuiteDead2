
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
        if(!All.Contains(this))
            All.Add(this);
    }

    public void OnDestroy()
    {
        if(All.Contains(this))
            All.Remove(this);
    }
}
