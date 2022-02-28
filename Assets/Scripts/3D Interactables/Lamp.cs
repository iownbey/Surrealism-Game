using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lamp : Interactable
{
    public GameObject toToggle;
    public AudioSource togglefx;

    public override void Interaction(InputAction.CallbackContext ctx)
    {
        togglefx.Play();
        toToggle.SetActive(!toToggle.activeSelf);
    }
}
