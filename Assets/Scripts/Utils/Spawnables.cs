
using UnityEngine;

public class Spawnables : MonoBehaviour
{
    public static Spawnables I;

    public PoolableObject GunFallingPart;

    public void Awake()
    {
        I = this;
    }

    public void OnDestroy()
    {
        I = null;
    }
}
