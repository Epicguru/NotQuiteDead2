using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class GunInfo
{
    [Tooltip("The time in seconds that the gun takes to be brought up to the target aiming position.")]
    public float AimTime = 0.3f;
    [Tooltip("The base magazine capacity count. In non open-bolt weapons, the final capacity is normally MagCapacity + 1.")]
    public int MagCapacity = 30;
    [Tooltip("If true, the gun does not store a bullet in the chamber but instead takes ammo straight from the magazine.")]
    public bool OpenBolt = false;
    [Tooltip("if true, the player can hold down the fire button to continuously fire. AI generally ignore this and fire semi-auto weapons as if they were full auto.")]
    public bool FullAuto = true;
    [Tooltip("The maximum rounds that the gun can fire per minute. A value of 60 means that the gun can shoot once per second.")]
    public float RPM = 60f;
}
