using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public const string STORED_NAME = "Stored";
    public const string DROPPED_NAME = "Dropped";
    public static readonly int STORED_ID = Animator.StringToHash(STORED_NAME);
    public static readonly int DROPPED_ID = Animator.StringToHash(DROPPED_NAME);

    private static Dictionary<ushort, Item> loaded = new Dictionary<ushort, Item>();

    public Character Character
    {
        get
        {
            if (transform.parent != null && _char != null)
            {
                var x = GetComponentInParent<Character>();
                if (x != _char)
                    _char = x;
            }
            if (_char == null)
                _char = GetComponentInParent<Character>();
            if (transform.parent == null && _char != null)
                _char = null;
            
            return _char;
        }
    }
    private Character _char;

    public Animator Animator
    {
        get
        {
            if (_anim == null)
                _anim = GetComponentInChildren<Animator>();
            return _anim;
        }
    }
    private Animator _anim;

    public ushort ID
    {
        get
        {
            return _id;
        }
    }
    [Header("Info")]
    [SerializeField]
    private ushort _id;
    public string Name = "Default Name";

    [Header("State")]
    public bool OnCharacter = false; // Is the item stored on the character's body, such as on their back or hip?

    public bool Dropped = true; // Is the item dropped on the floor in the world? If it is, then it is expected that OnCharacter is false and InHands is also false.

    // Is the item not dropped, on a character, but not in that character's hands?
    public bool Stored
    {
        get
        {
            return OnCharacter && !InHands;
        }
    }

    public bool InHands = false; // Assuming that OnCharacter is true, is the item in the character's hands?

    public HandPosition RightHand
    {
        get
        {
            if(_rhand == null)
            {
                foreach (var hand in GetComponentsInChildren<HandPosition>())
                {
                    if(hand.Hand == Hand.RIGHT)
                    {
                        _rhand = hand;
                        break;
                    }
                }
            }

            return _rhand;
        }
    }
    public HandPosition LeftHand
    {
        get
        {
            if (_lhand == null)
            {
                foreach (var hand in GetComponentsInChildren<HandPosition>())
                {
                    if (hand.Hand == Hand.LEFT)
                    {
                        _lhand = hand;
                        break;
                    }
                }
            }

            return _lhand;
        }
    }
    private HandPosition _rhand, _lhand;

    public bool CurrentlyStored
    {
        get
        {
            if (Animator == null)
                return true;

            var state = Animator.GetCurrentAnimatorStateInfo(0);

            return state.IsTag("Stored");
        }
    }

    private void Update()
    {
        if(Animator != null)
        {
            Animator.SetBool(STORED_ID, OnCharacter && !InHands);
            Animator.SetBool(DROPPED_ID, Dropped);
        }
    }

    public static bool IsLoaded(ushort id)
    {
        return loaded.ContainsKey(id);
    }

    public static Item Get(ushort id)
    {
        if (IsLoaded(id))
            return loaded[id];
        else
            return null;
    }

    /// <summary>
    /// Spawns a new item instance into the game world. The item by default is dropped on the floor (Dropped), is not on any character (!OnCharacter && !InHands).
    /// </summary>
    /// <param name="id">The item ID.</param>
    /// <param name="position">The item position.</param>
    /// <returns>The new item instance.</returns>
    public static Item Spawn(ushort id, Vector2 position)
    {
        if (!IsLoaded(id))
        {
            Debug.LogError("No item found for ID {0}, item not spawned!".Form(id));
            return null;
        }

        Item i = Instantiate(Get(id));
        i.transform.position = position;
        i.transform.rotation = Quaternion.identity;
        i.transform.parent = null;

        i.Dropped = true;
        i.OnCharacter = false;
        i.InHands = false;

        return i;
    }

    public static void LoadAll()
    {
        loaded.Clear();

        var raw = Resources.LoadAll<Item>("Items");

        ushort highest = 0;
        foreach (var item in raw)
        {
            var id = item.ID;
            if (loaded.ContainsKey(id))
            {
                Debug.LogError("Duplicate item ID's {0}, items: '{1}' and '{2}'. The former will not be loaded.".Form(id, item.Name, loaded[id].Name));
            }
            else
            {
                loaded.Add(id, item);

                if(id > highest)
                {
                    highest = id;
                }
            }
        }

        Debug.Log("Loaded {0} items, the highest ID was {1}".Form(loaded.Count, highest));
    }

    public static void UnloadAll()
    {
        loaded.Clear();
    }
}
