using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interactable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool hovered;

    static event System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> OnClick;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        print("added check");
        OnClick += Internal_Interaction;
    }

    public static void Trigger(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        print("A click happened");
        OnClick?.Invoke(ctx);
    }

    void Internal_Interaction(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        print("Checking for hover");
        if (hovered) Interaction(ctx);
    }

    public virtual void Interaction(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("hovering");
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
    }
}
