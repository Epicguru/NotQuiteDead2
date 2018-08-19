using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Item : NetworkBehaviour
{
    public const string STORED_NAME = "Stored";
    public static readonly int STORED_ID = Animator.StringToHash(STORED_NAME);

    public Animator Animator
    {
        get
        {
            if (_anim == null)
                _anim = GetComponentInChildren<Animator>();
            return _anim;
        }
    }
    private Animator _anim;

    [Header("State")]
    [SyncVar]
    public bool Stored = true;    

    public HandPosition RightHand
    {
        get
        {
            if(_rhand == null)
            {
                foreach (var hand in GetComponentsInChildren<HandPosition>())
                {
                    if(hand.Hand == Hand.RIGHT)
                    {
                        _rhand = hand;
                        break;
                    }
                }
            }

            return _rhand;
        }
    }
    public HandPosition LeftHand
    {
        get
        {
            if (_lhand == null)
            {
                foreach (var hand in GetComponentsInChildren<HandPosition>())
                {
                    if (hand.Hand == Hand.LEFT)
                    {
                        _lhand = hand;
                        break;
                    }
                }
            }

            return _lhand;
        }
    }
    private HandPosition _rhand, _lhand;

    public bool CurrentlyStored
    {
        get
        {
            if (Animator == null)
                return true;

            var state = Animator.GetCurrentAnimatorStateInfo(0);

            return state.IsTag("Stored");
        }
    }

    private void Update()
    {
        if(Animator != null)
        {
            Animator.SetBool(STORED_ID, this.Stored);
        }
    }
}
