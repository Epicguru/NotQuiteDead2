
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

    public int MagCapacity = 31;
    public int CurrentMagCount = 31;

    public float RPM = 500f;
    public bool Shoot;
    public bool Reload;
    public bool CheckMag;
    public bool CheckChamber;
    public bool Aim;

    // When equipped on the player, are all the sprites sorted behind the player?
    public bool CurrentlyBehindUser = false;

    public bool AllowCheckingWhenChamberEmpty = true;

    public float AimTime = 0.15f;

    [HideInInspector]
    public HandPosition LeftHand, RightHand;

    private float aimTimer = 0f;
    private float gunTimer = 100f;

    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    private int currentSpriteLayer = -1;
    private static int BEHIND_PLAYER_ID;
    private static int IN_FRONT_OF_PLAYER_ID;

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
        if (Item.Dropped)
        {
            // Animation state is managed in the Item class.
            SortSprites(BEHIND_PLAYER_ID);
            return;
        }

        // Collect raw input. The input must then be validated.
        CollectInput();

        SortSprites(CurrentlyBehindUser ? BEHIND_PLAYER_ID : IN_FRONT_OF_PLAYER_ID);

        if (Reload && !CanReload())
            Reload = false;
        if (CheckMag && !CanCheckMag())
            CheckMag = false;
        if (CheckChamber && !CanCheckChamber())
            CheckChamber = false;
        if (Shoot && !CanShoot())
            Shoot = false;
        if (Aim && !CanAim())
            Aim = false;

        if (Reload || Anim.Reloading)
            Anim.Aiming = false;

        if (CheckMag || Anim.CheckingMag || CheckChamber || Anim.CheckingChamber)
            Anim.Aiming = false;

        // Update rotation of item.
        UpdateCharacterDirection();
        UpdateRotation();

        // Update shooting timer...
        gunTimer += Time.deltaTime;

        if(gunTimer >= GetShootingInterval())
        {
            if (Shoot)
            {
                gunTimer = 0f;

                ShootGun();
            }
        }

        Anim.ChamberEmpty = CurrentMagCount == 0;
        Anim.MagEmpty = CurrentMagCount <= 1;

        if (Reload)
        {
            Anim.Trigger(GunAnimator.RELOAD_ID);
            Reload = false;
        }
        if (CheckMag)
        {
            Anim.Trigger(GunAnimator.CHECK_MAG_ID);
            CheckMag = false;
        }
        if (CheckChamber)
        {
            Anim.Trigger(GunAnimator.CHECK_CHAMBER_ID);
            CheckChamber = false;
        }

        Anim.Aiming = Aim;
        Anim.Blocked = IsBlocked();
    }

    private void UpdateRotation()
    {
        Vector2 mouseOffset = InputManager.MousePos - (Vector2)transform.position;
        if (!Direction.Right) mouseOffset.x *= -1f;
        float aimAngle = mouseOffset.ToAngle();
        float change = Time.deltaTime;

        if (Aim)
        {
            aimTimer += change;
        }
        else
        {
            aimTimer -= change;
        }

        aimTimer = Mathf.Clamp(aimTimer, 0f, AimTime);
        float aimP = aimTimer / AimTime;
        float x = aimP;

        // Curve...?
        float finalAngle = Mathf.LerpAngle(0f, aimAngle, x);

        var a = transform.localEulerAngles;
        a.z = finalAngle;
        transform.localEulerAngles = a;
    }

    private void UpdateCharacterDirection()
    {
        if (Item.Stored || !Aim)        
            return;
        
        Direction.Right = InputManager.MousePos.x >= transform.position.x;
    }

    private void OnGUI()
    {
        GUILayout.Label("Bullets: {0}".Form(CurrentMagCount));
    }

    public void CollectInput()
    {
        Shoot = InputManager.IsPressed("Shoot");
        Reload = InputManager.IsDown("Reload");
        CheckMag = InputManager.IsDown("Check Magazine");
        CheckChamber = InputManager.IsDown("Check Chamber");
        Aim = InputManager.IsPressed("Aim");
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

    public bool CanAim()
    {
        return !Item.Stored && !IsBlocked() && !Anim.Reloading && !Anim.CheckingMag && !Anim.CheckingChamber;
    }

    public bool CanReload()
    {
        return !Item.Stored && !IsBlocked() && !Anim.Reloading && !Anim.CheckingMag && !Anim.CheckingChamber;
    }

    public bool CanCheckMag()
    {
        if (Anim.ChamberEmpty && !AllowCheckingWhenChamberEmpty)
            return false;

        return !Item.Stored && !IsBlocked() && !Anim.Reloading && !Anim.CheckingMag && !Anim.CheckingChamber;
    }

    public bool CanCheckChamber()
    {
        if (Anim.ChamberEmpty && !AllowCheckingWhenChamberEmpty)
            return false;

        return !Item.Stored && !IsBlocked() && !Anim.Reloading && !Anim.CheckingMag && !Anim.CheckingChamber;
    }

    public bool CanShoot()
    {
        return !Item.Stored && !IsBlocked() && Anim.Aiming && this.CurrentMagCount > 0 && !Anim.Reloading && !Anim.CheckingMag && !Anim.CheckingChamber;
    }

    public bool IsBlocked()
    {
        return Input.GetKey(KeyCode.B);
    }

    public void ShootGun()
    {
        Anim.Trigger(GunAnimator.SHOOT_ID);
    }

    public float GetShootingInterval()
    {
        const float SECONDS_PER_MINUTE = 60f;
        float rps = RPM / SECONDS_PER_MINUTE;

        return 1f / rps;
    }
}
