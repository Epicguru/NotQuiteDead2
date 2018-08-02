
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
    public bool Aim;

    public KeyCode ShootKey = KeyCode.Mouse0;
    public KeyCode AimKey = KeyCode.Mouse1;
    public KeyCode ReloadKey = KeyCode.R;
    public KeyCode CheckMagKey = KeyCode.F;

    private float gunTimer = 100f;

    public void Update()
    {
        // Collect raw input. The input must then be validated.
        CollectInput();

        if (Reload && !CanReload())
            Reload = false;
        if (Aim && !CanAim())
            Aim = false;
        if (CheckMag && !CanCheckMag())
            CheckMag = false;
        if (Shoot && !CanShoot())
            Shoot = false;

        if (Reload || Anim.Reloading)
            Anim.Aiming = false;

        if (CheckMag || Anim.CheckingMag)
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

        Anim.Empty = CurrentMagCount == 0;

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
        Aim = Input.GetKey(AimKey);
    }

    public bool CanAim()
    {
        return !Anim.Stored && !Anim.Reloading && !Anim.CheckingMag;
    }

    public bool CanReload()
    {
        return !Anim.Stored && !Anim.Reloading && !Anim.CheckingMag;
    }

    public bool CanCheckMag()
    {
        return !Anim.Stored && !Anim.Reloading && !Anim.CheckingMag;
    }

    public bool CanShoot()
    {
        return !Anim.Stored && Anim.Aiming && this.CurrentMagCount > 0 && !Anim.Reloading && !Anim.CheckingMag;
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
