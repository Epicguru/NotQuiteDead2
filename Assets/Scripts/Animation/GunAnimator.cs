using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimator : MonoBehaviour
{
    public Animator Anim;
    public Gun Gun;

    public bool Stored;
    public bool Aiming;

    public bool Reloading;
    public bool CheckingMag;
    public bool Empty;

    public const string SHOOT_NAME = "Shoot";
    public const string STORED_NAME = "Stored";
    public const string AIM_NAME = "Aim";
    public const string EMPTY_NAME = "Empty";
    public const string RELOAD_NAME = "Reload";
    public const string CHECK_MAG_NAME = "Check Mag";

    public const string SHOOT_CALLBACK = "Shoot";
    public const string RELOAD_CALLBACK = "Reload";
    public const string CHECK_MAG_CALLBACK = "Check Mag";

    public static readonly int SHOOT_ID = Animator.StringToHash(SHOOT_NAME);
    public static readonly int STORED_ID = Animator.StringToHash(STORED_NAME);
    public static readonly int AIMING_ID = Animator.StringToHash(AIM_NAME);
    public static readonly int EMPTY_ID = Animator.StringToHash(EMPTY_NAME);
    public static readonly int RELOAD_ID = Animator.StringToHash(RELOAD_NAME);
    public static readonly int CHECK_MAG_ID = Animator.StringToHash(CHECK_MAG_NAME);

    public void AnimationCallback(AnimationEvent e)
    {
        if(e.stringParameter == SHOOT_CALLBACK)
        {
            Gun.CurrentMagCount--;
        }
        if(e.stringParameter == RELOAD_CALLBACK)
        {
            Reloading = false;
            Gun.CurrentMagCount = Gun.MagCapacity;
        }
        if(e.stringParameter == CHECK_MAG_CALLBACK)
        {
            CheckingMag = false;
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

        Anim.SetTrigger(id);
    }

    public void Update()
    {
        Anim.SetBool(STORED_ID, Stored);
        Anim.SetBool(AIMING_ID, Aiming);
        Anim.SetBool(AIMING_ID, Aiming);
        Anim.SetBool(EMPTY_ID, Empty);
    }
}
