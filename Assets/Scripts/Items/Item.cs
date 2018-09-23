using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetParentSync))]
public class Item : NetworkBehaviour
{
    public const string STORED_NAME = "Stored";
    public static readonly int STORED_ID = Animator.StringToHash(STORED_NAME);

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

    public NetParentSync NetParentSync
    {
        get
        {
            if (_netPSync == null)
                _netPSync = GetComponent<NetParentSync>();
            return _netPSync;
        }
    }
    private NetParentSync _netPSync;

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
    [SyncVar]
    public bool OnCharacter = false; // Is the item stored on the character's body, such as on their back or hip?

    public bool Stored
    {
        get
        {
            return OnCharacter && !InHands;
        }
    }

    [SyncVar]
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

    [Server]
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

        NetworkServer.Spawn(i.gameObject);

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
                NetManager.Register(item.gameObject);

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
