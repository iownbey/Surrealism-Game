using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaySound : Interactable
{
    public AudioSource fx;

    public override void Interaction(InputAction.CallbackContext ctx)
    {
        fx.Play();
    }
}
