
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
[RequireComponent(typeof(GunAnimator))]
public class Gun : MonoBehaviour
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
    public bool s_Aiming;
    public bool s_CheckChamber;
    public bool s_CheckMagazine;
    public bool s_Reload;
    public bool s_Shoot;

    [Header("Volatile")]
    public int Ammo;

    [HideInInspector]
    public HandPosition LeftHand, RightHand;
    public bool BulletInChamber
    {
        get
        {
            return Ammo > 0;
        }
    }

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
    private float shootTimer = 0f;
    private float shotInterval { get { return 1f / (Info.RPM / 60f); } }

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

        // Allow gun to shoot instantly upon spawning.
        shootTimer = shotInterval + 1f;

        // Give full magazine upon spawning.
        Ammo = Info.MagCapacity;
    }

    private void Update()
    {        
        // Sort based on the current state. Normally driven by animation.
        SortSprites(CurrentlyBehindUser ? BEHIND_PLAYER_ID : IN_FRONT_OF_PLAYER_ID);
        transform.localScale = Vector3.one;

        bool dropped = Item.Dropped;
        if (dropped) // If dropped (anywhere, on any network node) then stop execution here.
            return;

        bool valid = Item.Character != null;
        if (!valid)
            return; // If the character is null (anywhere) then we can't do much with this item.

        TakeInput();
        UpdateShotTimer();
        ValidateInput();
        LookToMouse();
        RotateToMouse();

        Anim.MagEmpty =     (Info.OpenBolt ? Ammo == 0 : Ammo <= 1);
        Anim.ChamberEmpty = (Ammo == 0);

        Anim.Aiming = this.s_Aiming;
        ResolveTrigger(ref s_Reload, GunAnimator.RELOAD_ID);
        ResolveTrigger(ref s_CheckChamber, GunAnimator.CHECK_CHAMBER_ID);
        ResolveTrigger(ref s_CheckMagazine, GunAnimator.CHECK_MAG_ID);
        ResolveTrigger(ref s_Shoot, GunAnimator.SHOOT_ID);
    }

    private void ResolveTrigger(ref bool flag, int ID)
    {
        if (flag)
        {
            Anim.Trigger(ID);
            flag = false;
        }
    }

    private void TakeInput()
    {
        s_Aiming = InputManager.IsPressed("Aim");
        s_CheckChamber = InputManager.IsDown("Check Chamber");
        s_CheckMagazine = InputManager.IsDown("Check Magazine");
        s_Reload = InputManager.IsDown("Reload");
        s_Shoot = Info.FullAuto ? InputManager.IsPressed("Shoot") : InputManager.IsDown("Shoot");
    }

    private void ValidateInput()
    {
        // Makes sure all the states are compatible and that the input is not contradicting state.
        if (Item.Dropped || !Item.InHands)
        {
            s_Aiming = false;
            s_Reload = false;
            s_CheckChamber = false;
            s_CheckMagazine = false;
            s_Shoot = false;
            return;
        }

        if (Ammo >= Info.MagCapacity + (Info.OpenBolt ? 0 : 1)) // Cannot reload if magazine is full.
            s_Reload = false;
        if (Ammo <= 0) // Cannot shoot if there are no bullets left.
            s_Shoot = false;
        if (s_Aiming) // Cannot check chamber or magazine while aiming. Reloading should cancel aim.
        {
            s_CheckChamber = false;
            s_CheckMagazine = false;
        }
        if (Anim.Reloading || Anim.CheckingChamber || Anim.CheckingMag) // Cannot aim when reloading, checking mag or chamber.
            s_Aiming = false;

        if (shootTimer <= shotInterval)
        {
            // Cannot shoot regardless of actual input state.
            s_Shoot = false;
        }
        else if(s_Shoot)
        {
            // Input wants to shoot, approved!
            shootTimer = 0f;
        }
    }

    private void RotateToMouse()
    {
        // Sets the rotation of this gun to point towards the mouse.
        Vector2 mouseOffset = InputManager.MousePos - (Vector2)transform.position;
        if (!Right) mouseOffset.x *= -1f;
        float aimAngle = mouseOffset.ToAngle();
        float change = Time.deltaTime;

        if (s_Aiming)
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
    }

    private void LookToMouse()
    {
        // Works on both server and clients!
        Vector2 mouseOffset = InputManager.MousePos - (Vector2)transform.position;
        if(mouseOffset.x >= 0f && s_Aiming)
        {
            Right = true;
        }
        else if(s_Aiming)
        {
            Right = false;
        }
    }

    private void UpdateShotTimer()
    {
        shootTimer += Time.deltaTime;
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
