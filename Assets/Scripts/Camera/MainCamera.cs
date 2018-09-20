using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MainCamera : MonoBehaviour
{
    public static MainCamera Instance;
    public static Camera Cam
    {
        get
        {
            return Instance == null ? null : Instance.Camera;
        }
    }

    public Camera Camera
    {
        get
        {
            if (_cam == null)
                _cam = GetComponent<Camera>();
            return _cam;
        }
    }
    private Camera _cam;

    public static Transform Target;
    public float MaxFollowSpeed = 100f;
    public float MaxSpeedDistance = 10f;
    public AnimationCurve Curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public void Awake()
    {
        this.tag = "MainCamera";
    }

    public void LateUpdate()
    {
        if (Target == null)
            return;

        Vector2 displacement = Target.position - transform.position;
        float distance = displacement.magnitude;
        displacement.Normalize();

        float speed = Curve.Evaluate(Mathf.Clamp01(distance / MaxSpeedDistance)) * MaxFollowSpeed;

        transform.position += (Vector3)displacement * speed * Time.deltaTime;
    }
}
