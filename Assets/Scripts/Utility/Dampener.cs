using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dampener : MonoBehaviour
{
    public float value;
    public float target;
    public float smoothTime;
    float velocity;

    public Dampener Init(float value, float smoothTime)
    {
        this.value = target = value;
        this.smoothTime = smoothTime;
        return this;
    }
    
    void Update()
    {
        value = Mathf.SmoothDamp(value, target, ref velocity, smoothTime);
    }

    public static implicit operator float(Dampener d) => d.value;
}
