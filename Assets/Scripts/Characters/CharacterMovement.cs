using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CharacterDirection))]
public class CharacterMovement : MonoBehaviour {

    public Animator Animator;
    public string RunningBool = "Running";

	public Rigidbody2D Body
    {
        get
        {
            if (_body == null)
                _body = GetComponent<Rigidbody2D>();
            return _body;
        }
    }
    private Rigidbody2D _body;

    public CharacterDirection Direction
    {
        get
        {
            if (_direction == null)
                _direction = GetComponent<CharacterDirection>();
            return _direction;
        }
    }
    private CharacterDirection _direction;

    public Vector2 NormalizedInputDirection;
    public float CurrentSpeed = 10f;

    public void Update()
    {
        // Speed cannot be less than zero...
        CurrentSpeed = Mathf.Max(0f, CurrentSpeed);

        Body.velocity = NormalizedInputDirection.normalized * CurrentSpeed;

        // Change character direction based on weather we are moving left or right, and weather there is a item being held.
        // TODO use item input or other inputs that can override this default behaviour.
        if(Body.velocity.x != 0f)
            Direction.Right = Body.velocity.x > 0f;

        // Update the character animator. It may be more complex than the default implementation, but it will at least have the running bool.
        if(Animator != null)
        {
            Animator.SetBool(RunningBool, Body.velocity != Vector2.zero);
        }
    }
}
