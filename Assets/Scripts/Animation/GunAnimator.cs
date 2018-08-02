using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimator : StateMachineBehaviour
{
    public string ShootState = "Shoot";
    public bool Stored;
    public bool Aiming;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if (stateInfo.IsName(ShootState))
        {
            Debug.Log("Fired!");
        }
    }
}
