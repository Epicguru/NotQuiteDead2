
public class GunAnimationCallback : GenericAnimationCallback
{
    public bool BehindUser = false;

    public Gun Gun
    {
        get
        {
            if (_gun == null)
                _gun = GetComponentInParent<Gun>();
            return _gun;
        }
    }
    private Gun _gun;

    public void Awake()
    {
        var gun = Gun;
        if (gun == null)
            return;

        base.UponEvent.AddListener(gun.Anim.AnimationCallback);
    }

    public void Update()
    {
        Gun.CurrentlyBehindUser = BehindUser;
    }
}