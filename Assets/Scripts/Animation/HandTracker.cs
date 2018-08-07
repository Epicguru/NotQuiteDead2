using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HandTracker : MonoBehaviour
{
    public SpriteRenderer Renderer;
    public Hand Hand;

    public Transform IdleTarget;

    public HandPosition Target
    {
        get
        {
            return _target;
        }
        set
        {
            if (value == _target)
                return;

            StartTimeToTarget();
            _target = value;
        }
    }
    private HandPosition _target;

    public float ReturnToIdleSpeed = 0.5f;
    public float TimeToTarget = 0.5f;
    public AnimationCurve CurveToTarget = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public bool AtTarget
    {
        get
        {
            return Target != null && timerToTarget / TimeToTarget >= 1f;
        }
    }

    private float timerToTarget = 0f;
    private Vector3 oldPos;
    private Vector3 oldScale;
    private Quaternion oldRotation;

    private const string CHARACTER_LAYER = "Characters";
    private const string ITEM_LAYER = "Equipped Items";

    public void LateUpdate()
    {
        timerToTarget += Time.deltaTime;
        float targetP = Mathf.Clamp01(timerToTarget / TimeToTarget);

        if(Target != null)
        {
            if(Target.Hand == this.Hand)
            {
                // Transform...
                float curved = CurveToTarget.Evaluate(targetP);
                transform.position = Vector3.Lerp(oldPos, Target.transform.position, curved);
                transform.rotation = Quaternion.Lerp(oldRotation, Target.transform.rotation, curved);
                transform.localScale = Vector3.Lerp(oldScale, Target.transform.localScale, curved);

                // Layer and order...
                if (Target.BehindItem)
                {
                    // This hand is supposed to be behind the item, so we go to the character layer...
                    if(Renderer.sortingLayerName != CHARACTER_LAYER)
                    {
                        Renderer.sortingLayerName = CHARACTER_LAYER;
                        Renderer.sortingOrder = 1000;
                    }
                }
                else
                {
                    // Hand goes in front of the item, put it in that layer and make sure the order is higher than anything on the item.
                    if(Renderer.sortingLayerName != ITEM_LAYER)
                    {
                        Renderer.sortingLayerName = ITEM_LAYER;
                        Renderer.sortingOrder = 1000;
                    }
                }

                // Flip or not...
                bool normal = Hand == Hand.RIGHT;
                bool targetFlip = Target.Flipped ? !normal : normal;
                if (Renderer.flipX != targetFlip)
                {
                    Renderer.flipX = targetFlip;
                }
                
            }
            else
            {
                Debug.LogError("Wrong hand assigned to HandTracker! Expected {0}, got {1}".Form(this.Hand, Target.Hand));
            }
        }
        else
        {
            Vector3 idleTarget = IdleTarget.transform.position;
            transform.position = Vector3.Lerp(transform.position, idleTarget, Time.deltaTime * ReturnToIdleSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * ReturnToIdleSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * ReturnToIdleSpeed);

            bool flipped = Hand == Hand.RIGHT;
            if(Renderer.flipX != flipped)
            {
                Renderer.flipX = flipped;
            }

            if (Renderer.sortingLayerName != CHARACTER_LAYER)
            {
                Renderer.sortingLayerName = CHARACTER_LAYER;
                Renderer.sortingOrder = 10;
            }
        }
    }

    public void StartTimeToTarget()
    {
        timerToTarget = 0f;
        oldPos = transform.position;
        oldScale = transform.localScale;
        oldRotation = transform.rotation;
    }
}
