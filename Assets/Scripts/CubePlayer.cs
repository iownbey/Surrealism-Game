using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CubePlayer : MonoBehaviour
{
    public Color dashColor;
    public Color rechargeColor;

    public Controls c;
    Rigidbody2D rb;
    new BoxCollider2D collider;
    const float movementCoefficient = 30;

    const float dashCoefficient = 20;
    const float dashCooldownDuration = 1;
    const float dashTimeDuration = 0.5f;

    const float totalHealth = 4;

    Timer dashCooldown;
    Timer dashTime;
    Timer healthTimer;
    ColorController colorer;
    public bool Dashing { get => !dashTime.IsFinished; }
    List<IInteractable> interactables = new List<IInteractable>();
    bool frozen = false;

    // Start is called before the first frame update

    private void Awake()
    {
        c = new Controls();
        c.Enable();
        c.Player.Dash.performed += Dash;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();

        dashCooldown = gameObject.AddComponent<Timer>();
        dashTime = gameObject.AddComponent<Timer>();
        dashCooldown.OnFinish += () => { colorer.Flash(rechargeColor, 8); };
        colorer = gameObject.AddComponent<ColorController>();
        healthTimer = gameObject.AddComponent<Timer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (c.Player.Interact.triggered && !frozen && interactables.Count > 0)
        {
            interactables[0].Interact(this);
        }
        colorer.multiply = Color.Lerp(Color.white, Color.black, Mathf.Clamp01(healthTimer.time/totalHealth));
        if (healthTimer.time > totalHealth)
        {
            //die
        }
    }

    void Dash(InputAction.CallbackContext ctx)
    {
        if (dashCooldown.IsFinished)
        {
            rb.AddForce(c.Player.Movement.ReadValue<Vector2>() * dashCoefficient, ForceMode2D.Impulse);
            dashCooldown.Set(dashCooldownDuration);
            dashTime.Set(dashTimeDuration);
            colorer.Flash(dashColor, 4);
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(c.Player.Movement.ReadValue<Vector2>() * movementCoefficient);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IInteractable i = collision.collider.GetComponent<IInteractable>();
        if (i != null) interactables.Add(i);

        if (!Dashing)
        {
            //if the colliding body has a damager, take the hit.
            collision.collider.IfHasComponent<Collider2D, Damager>((d) => { TakeHit(d, collision.GetContact(0).point); });
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable i = collision.GetComponent<IInteractable>();
        if (i != null) interactables.Add(i);

        if (!Dashing)
        {
            //if the trigger body has a damager, take the hit.
            collision.IfHasComponent<Collider2D, Damager>((d) => { if (d.trigger)TakeHit(d, collider.ClosestPoint(collision.transform.position)); });
        }
    }

    void TakeHit(Damager d, Vector2 hitPos)
    {
        colorer.Flash(Color.red, 8);
        healthTimer.time += d.damage * totalHealth;
        rb.AddForceAtPosition((rb.position - hitPos).normalized * d.rebound, hitPos);
    }

    private void OnCollisionExit2D(Collision2D collision) => OnTriggerExit2D(collision.collider);
    private void OnTriggerExit2D(Collider2D collision)
    {
        interactables.Remove(collision.gameObject.GetComponent<IInteractable>());
    }

    public void Freeze()
    {
        frozen = true;
        c.Player.Movement.Disable();
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
    }

    public void Unfreeze()
    {
        frozen = false;
        c.Player.Movement.Enable();
        rb.isKinematic = false;
    }

    public IEnumerator WaitForInput()
    {
        print("Waiting...");

        //yield return new WaitUntil(() => { return c.Player.Interact.triggered; });

        bool done = false;
        void callback(InputAction.CallbackContext ctx) { done = true; }
        c.Player.Interact.performed += callback;
        yield return new WaitUntil(() => { return done == true; });
        c.Player.Interact.performed -= callback;
        print("done");
    }
}
