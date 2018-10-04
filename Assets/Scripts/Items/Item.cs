using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Item : MonoBehaviour
{
    public const string STORED_NAME = "Stored";
    public const string DROPPED_NAME = "Dropped";
    public static readonly int STORED_ID = Animator.StringToHash(STORED_NAME);
    public static readonly int DROPPED_ID = Animator.StringToHash(DROPPED_NAME);

    private static Dictionary<ushort, Item> loaded = new Dictionary<ushort, Item>();

    public Collider2D Collider
    {
        get
        {
            if (_coll == null)
                _coll = GetComponent<Collider2D>();
            return _coll;
        }
    }
    private Collider2D _coll;

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
    public Sprite Icon;
    public ItemSlot Slot = ItemSlot.STORED;

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

    private static List<string> stateErrors = new List<string>();
    public List<string> GetStateErrors()
    {
        stateErrors.Clear();
        if(Icon == null)
        {
            stateErrors.Add("Icon is missing. (null)");
        }
        if (string.IsNullOrWhiteSpace(Name))
        {
            stateErrors.Add("Name is null or whitespace ('{0}')".Form(Name));
        }

        return stateErrors;
    }

    private void Start()
    {
        Collider.isTrigger = true;
    }

    private void Update()
    {
        if(Animator != null)
        {
            Animator.SetBool(STORED_ID, OnCharacter && !InHands);
            Animator.SetBool(DROPPED_ID, Dropped);
        }
    }

    public bool CanBePickedUp(Character c)
    {
        if (c == null)
            return false;
        float dst = Vector2.Distance(c.transform.position, transform.position);
        return dst <= 4f;
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
                // Check for errors.
                var errors = item.GetStateErrors();
                foreach (var e in errors)
                {
                    Debug.LogError("Item '{0}'({1}) has error: {2}".Form(item.Name, item.ID, e.Trim()));
                }

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

    [DebugCommand("Spawns an item beneith the local player.", GodModeOnly = true, Parameters = "INT:ID:The numerical ID of the item to spawn. Is never less than zero.")]
    private static void Spawn_Item(int id)
    {
        if(Player.Character == null)
        {
            Commands.LogError("Player character not found! Item not spawned.");
            return;
        }

        var pos = Player.Character.transform.position;
        Spawn_Item(id, pos.x, pos.y);
    }

    [DebugCommand("Spawns an item at a position.", GodModeOnly = true, Parameters = "INT:ID:The numerical ID of the item to spawn. Is never less than zero., FLOAT:x:The x position to spawn at., FLOAT:y:The y position to spawn at.")]
    private static void Spawn_Item(int id, float x, float y)
    {
        if (id < 0)
        {
            Commands.LogError("Item for Id {0} not found! ID value is outside of the valid bounds.".Form(id));
            return;
        }

        if (id > ushort.MaxValue)
        {
            Commands.LogError("Item for Id {0} not found! ID value is outside of the valid bounds.");
            return;
        }

        Item found = Item.Get((ushort)id);

        if (found == null)
        {
            Commands.LogError("Item for Id {0} not found!".Form(id));
            return;
        }
        Vector2 pos = new Vector2(x, y);
        var spawned = Item.Spawn(found.ID, pos);

        Commands.Log("Spawned a new '{0}' at {1}.".Form(spawned.Name, pos));
    }

    public void OnDestroy()
    {
        if (Dropped)
            return;

        if(Character != null && Character.HasHandManager && (InHands || OnCharacter))
        {
            Character.Hands.NotifyDestroyed(this);
        }
    }
}
