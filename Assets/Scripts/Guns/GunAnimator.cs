﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimator : MonoBehaviour
{
    public Animator Anim
    {
        get
        {
            if (_anim == null)
                _anim = GetComponentInChildren<Animator>();
            if (_anim == null)
                Debug.LogError("No animator component found for this Item Animator!");
            return _anim;
        }
    }
    private Animator _anim;

    public Gun Gun
    {
        get
        {
            if (_gun == null)
                _gun = GetComponent<Gun>();
            return _gun;
        }
    }
    private Gun _gun;

    public bool Aiming;

    public bool Reloading;
    public bool CheckingMag;
    public bool CheckingChamber;
    public bool ChamberEmpty;
    public bool MagEmpty;
    public bool Blocked;

    public const string SHOOT_NAME = "Shoot";
    public const string AIM_NAME = "Aim";
    public const string CHAMBER_EMPTY_NAME = "Chamber Empty";
    public const string MAG_EMPTY_NAME = "Mag Empty";
    public const string RELOAD_NAME = "Reload";
    public const string CHECK_MAG_NAME = "Check Mag";
    public const string CHECK_CHAMBER_NAME = "Check Chamber";
    public const string BLOCKED_NAME = "Blocked";    

    public static readonly int SHOOT_ID = Animator.StringToHash(SHOOT_NAME);
    public static readonly int AIMING_ID = Animator.StringToHash(AIM_NAME);
    public static readonly int CHAMBER_EMPTY_ID = Animator.StringToHash(CHAMBER_EMPTY_NAME);
    public static readonly int MAG_EMPTY_ID = Animator.StringToHash(MAG_EMPTY_NAME);
    public static readonly int RELOAD_ID = Animator.StringToHash(RELOAD_NAME);
    public static readonly int CHECK_MAG_ID = Animator.StringToHash(CHECK_MAG_NAME);
    public static readonly int CHECK_CHAMBER_ID = Animator.StringToHash(CHECK_CHAMBER_NAME);
    public static readonly int BLOCKED_ID = Animator.StringToHash(BLOCKED_NAME);

    public const string SHOOT_CALLBACK = "Shoot";
    public const string RELOAD_CALLBACK = "Reload";
    public const string CHECK_MAG_CALLBACK = "Check Mag";
    public const string CHECK_CHAMBER_CALLBACK = "Check Chamber";
    public const string DROP_MAG_CALLBACK = "Drop Mag";

    public FallingMagInfo FallingMagazineInfo;
    [System.Serializable]
    public class FallingMagInfo
    {
        public SpriteRenderer RealMag;
        public Vector2 Velocity = new Vector2(1f, -5f);
        public float AngularVelocity = 30f;
        public float Lifetime = 1.5f;
        public bool UseGravity = true;
    }

    public bool CurrentlyStored
    {
        get
        {
            return Anim.GetCurrentAnimatorStateInfo(0).IsTag("Stored");
        }
    }

    public void AnimationCallback(AnimationEvent e)
    {
        if(e.stringParameter == SHOOT_CALLBACK)
        {
            Gun.Shoot();
        }
        if(e.stringParameter == RELOAD_CALLBACK)
        {
            Reloading = false;
            int add = 0;
            if (!Gun.Info.OpenBolt && Gun.BulletInChamber) // If not open bolt and there is a bullet in the chamber.
                add = 1;
            Gun.Ammo = Gun.Info.MagCapacity + add;
        }
        if(e.stringParameter == CHECK_MAG_CALLBACK)
        {
            CheckingMag = false;
        }
        if (e.stringParameter == CHECK_CHAMBER_CALLBACK)
        {
            CheckingChamber = false;
        }
        if (e.stringParameter == DROP_MAG_CALLBACK)
        {
            // Drop magazine.
            if(FallingMagazineInfo.RealMag != null)
            {
                var spawned = Pool.Get(Spawnables.I.GunFallingPart).GetComponent<GunFallingPart>();
                Vector2 inheritedVelocity = (Gun.Item.Character == null ? Vector2.zero : Gun.Item.Character.Movement.Body.velocity);
                spawned.MimicWithVel(FallingMagazineInfo.RealMag, FallingMagazineInfo.Velocity, FallingMagazineInfo.AngularVelocity, FallingMagazineInfo.UseGravity, inheritedVelocity);
                spawned.StartLife(FallingMagazineInfo.Lifetime);
            }            
        }
    }

    public void Trigger(int id)
    {
        if (id == RELOAD_ID)
        {
            if (Reloading)
                return;
            Reloading = true;
        }
        if (id == CHECK_MAG_ID)
        {
            if (CheckingMag)
                return;
            CheckingMag = true;
        }
        if (id == CHECK_CHAMBER_ID)
        {
            if (CheckingChamber)
                return;
            CheckingChamber = true;
        }

        Anim.SetTrigger(id);
    }

    public void Update()
    {
        Anim.SetBool(AIMING_ID, Aiming);
        Anim.SetBool(MAG_EMPTY_ID, MagEmpty);
        Anim.SetBool(CHAMBER_EMPTY_ID, ChamberEmpty);
        Anim.SetBool(BLOCKED_ID, Blocked);
    }
}
