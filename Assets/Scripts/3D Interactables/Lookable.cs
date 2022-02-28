using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lookable : Interactable
{
    new Transform transform;
    public Transform restPos;
    public Transform activePos;
    public AudioSource pickupFX;
    public AudioSource putdownFX;
    Dampener blend;

    override protected void Start()
    {
        blend = gameObject.AddComponent<Dampener>().Init(0, 0.2f);
        transform = gameObject.transform;
        base.Start();
    }

    void Update()
    {
        transform.position = Vector3.Lerp(restPos.position, activePos.position, blend);
        transform.rotation = Quaternion.Slerp(restPos.rotation, activePos.rotation, blend);
    }

    public override void Interaction(InputAction.CallbackContext ctx)
    {
        if (blend.target == 1)
        {
            blend.target = 0;
            putdownFX?.Play();
        }
        else
        {
            blend.target = 1;
            pickupFX?.Play();
        }
    }
}
