
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Item))]
[RequireComponent(typeof(GunAnimator))]
public class Gun : NetworkBehaviour
{
    public GunAnimator Anim
    {
        get
        {
            if (_anim == null)
                _anim = GetComponent<GunAnimator>();
            return _anim;
        }
    }
    private GunAnimator _anim;
    public Item Item
    {
        get
        {
            if (_item == null)
                _item = GetComponent<Item>();
            return _item;
        }
    }
    private Item _item;
    public CharacterDirection Direction
    {
        get
        {
            return Item.Character.Direction;
        }
    }

    [Header("Data")]
    public GunInfo Info;

    [Header("State")] 
    public bool CurrentlyBehindUser = false; // When equipped on the player, are all the sprites sorted behind the player?
    [SyncVar] public bool Aiming;

    [Header("Volatile")]
    [SyncVar] public int Ammo;

    [Header("Debug")]
    [SyncVar] public float Angle;

    [HideInInspector]
    public HandPosition LeftHand, RightHand;

    private bool Right
    {
        get
        {
            if (Direction == null)
                return true;
            else
                return Direction.Right;
        }
        set
        {
            if(Direction == null)
            {
                return;
            }
            if(Direction.Right != value) // TODO make the Direction class allow for cmd to be sent from client.
                Direction.Right = value;
        }
    }
    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    private int currentSpriteLayer = -1;
    private static int BEHIND_PLAYER_ID;
    private static int IN_FRONT_OF_PLAYER_ID;
    private float aimTimer = 0f;

    private void Start()
    {
        IN_FRONT_OF_PLAYER_ID = SortingLayer.NameToID("Equipped Items");
        BEHIND_PLAYER_ID = SortingLayer.NameToID("Behind Equipped Items");

        spriteRenderers.Clear();
        GetComponentsInChildren<SpriteRenderer>(true, spriteRenderers);
        spriteRenderers.RemoveAll(x => x.CompareTag("Ignore Dynamic Layering"));

        foreach (var hand in GetComponentsInChildren<HandPosition>(true))
        {
            if(hand.Hand == Hand.LEFT)
            {
                if (LeftHand != null)
                    Debug.LogError("Item {0} has more than one left hand! That's pretty wierd...".Form(name));
                else
                    LeftHand = hand;
            }
            else if(hand.Hand == Hand.RIGHT)
            {
                if (RightHand != null)
                    Debug.LogError("Item {0} has more than one right hand! That's pretty wierd...".Form(name));
                else
                    RightHand = hand;
            }
        }
    }

    private void LateUpdate()
    {        
        // Sort based on the current state. Normally driven by animation.
        SortSprites(CurrentlyBehindUser ? BEHIND_PLAYER_ID : IN_FRONT_OF_PLAYER_ID);

        bool dropped = Item.Dropped;
        if (dropped) // If dropped (anywhere, on any network node) then stop execution here.
            return;

        bool valid = Item.Character != null;
        if (!valid)
            return; // If the character is null (anywhere) then we can't do much with this item.

        var man = Item.Character.Manipulator;
        bool player = man.IsPlayer;
        bool localPlayer = player && man.Player.isLocalPlayer;
        bool auth = localPlayer ? true : (isServer && !player); // Auth means that the currnet network node (client, server, host) has control over this gun.

        if (auth)
        {
            // TODO take input.
            // TODO validate input.
            Aiming = true;

            ValidateInput();
            RotateToMouse();
            LookToMouse();
        }
        else
        {
            // Mimic what the server sees for this gun.
            // No need to take or validate input.

            // Lerp to sent angle.
            LerpToAngle();
        }

        // Here, things to do regardless of the auth state.
        Anim.Aiming = this.Aiming;
    }

    private void ValidateInput()
    {
        // Makes sure all the states are compatible and that the input is not contradicting state.
        if (Item.Dropped || !Item.InHands)
            Aiming = false;
    }

    private void LerpToAngle()
    {
        var rot = transform.localEulerAngles;
        rot.z = Mathf.LerpAngle(rot.z, Angle, Time.deltaTime * 15f);
        transform.localEulerAngles = rot;
    }

    private void RotateToMouse()
    {
        // Sets the rotation of this gun to point towards the mouse.
        Vector2 mouseOffset = InputManager.MousePos - (Vector2)transform.position;
        if (!Right) mouseOffset.x *= -1f;
        float aimAngle = mouseOffset.ToAngle();
        float change = Time.deltaTime;

        if (Aiming)
        {
            aimTimer += change;
        }
        else
        {
            aimTimer -= change;
        }

        aimTimer = Mathf.Clamp(aimTimer, 0f, Info.AimTime);
        float aimP = aimTimer / Info.AimTime;
        float x = aimP;

        // Curve...?
        float finalAngle = Mathf.LerpAngle(0f, aimAngle, x);

        var a = transform.localEulerAngles;
        a.z = finalAngle;
        transform.localEulerAngles = a;

        // If on the server, send the angle directly to all other clients through the SyncVar.
        // Otherwise it will have to sent through commands.
        if (isServer)
        {
            Angle = finalAngle;
        }
    }

    private void LookToMouse()
    {
        // Works on both server and clients!
        Vector2 mouseOffset = InputManager.MousePos - (Vector2)transform.position;
        if(mouseOffset.x >= 0f && Aiming)
        {
            Right = true;
        }
        else if(Aiming)
        {
            Right = false;
        }
    }

    public void SortSprites(int targetLayerID)
    {
        if (targetLayerID == currentSpriteLayer)        
            return;
        
        foreach(var spr in spriteRenderers)
        {
            if (spr == null)
                continue;

            spr.sortingLayerID = targetLayerID;
        }

        currentSpriteLayer = targetLayerID;
    }
}
