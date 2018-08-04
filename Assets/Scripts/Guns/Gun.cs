
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GunAnimator Anim;

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

    public KeyCode ShootKey = KeyCode.Mouse0;
    public KeyCode AimKey = KeyCode.Mouse1;
    public KeyCode ReloadKey = KeyCode.R;
    public KeyCode CheckMagKey = KeyCode.F;
    public KeyCode CheckChamberKey = KeyCode.T;

    private float gunTimer = 100f;

    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    private int currentSpriteLayer = -1;
    private static int BEHIND_PLAYER_ID;
    private static int IN_FRONT_OF_PLAYER_ID;

    [HideInInspector]
    public HandPosition LeftHand, RightHand;

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

    private void Update()
    {
        // Collect raw input. The input must then be validated.
        CollectInput();

        SortSprites(CurrentlyBehindUser ? BEHIND_PLAYER_ID : IN_FRONT_OF_PLAYER_ID);

        if (Reload && !CanReload())
            Reload = false;
        if (Aim && !CanAim())
            Aim = false;
        if (CheckMag && !CanCheckMag())
            CheckMag = false;
        if (CheckChamber && !CanCheckChamber())
            CheckChamber = false;
        if (Shoot && !CanShoot())
            Shoot = false;

        if (Reload || Anim.Reloading)
            Anim.Aiming = false;

        if (CheckMag || Anim.CheckingMag || CheckChamber || Anim.CheckingChamber)
            Anim.Aiming = false;

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
    }

    public void OnGUI()
    {
        GUILayout.Label("Bullets: {0}".Form(CurrentMagCount));
    }

    public void CollectInput()
    {
        Shoot = Input.GetKey(ShootKey);
        Reload = Input.GetKeyDown(ReloadKey);
        CheckMag = Input.GetKeyDown(CheckMagKey);
        CheckChamber = Input.GetKeyDown(CheckChamberKey);
        Aim = Input.GetKey(AimKey);
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
        return !Anim.Stored && !Anim.Reloading && !Anim.CheckingMag && !Anim.CheckingMag;
    }

    public bool CanReload()
    {
        return !Anim.Stored && !Anim.Reloading && !Anim.CheckingMag && !Anim.CheckingChamber;
    }

    public bool CanCheckMag()
    {
        if (Anim.ChamberEmpty && !AllowCheckingWhenChamberEmpty)
            return false;

        return !Anim.Stored && !Anim.Reloading && !Anim.CheckingMag && !Anim.CheckingChamber;
    }

    public bool CanCheckChamber()
    {
        if (Anim.ChamberEmpty && !AllowCheckingWhenChamberEmpty)
            return false;

        return !Anim.Stored && !Anim.Reloading && !Anim.CheckingMag && !Anim.CheckingChamber;
    }

    public bool CanShoot()
    {
        return !Anim.Stored && Anim.Aiming && this.CurrentMagCount > 0 && !Anim.Reloading && !Anim.CheckingMag && !Anim.CheckingChamber;
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
