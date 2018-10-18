
using UnityEngine;

public class HealthTests : MonoBehaviour
{
    public Health h;

    public void Start()
    {
        var res = h.DealDamage(100f, 0f);
        Debug.Log(res);
    }
}