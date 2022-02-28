using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerCircle : MonoBehaviour
{
    public GameObject deathParticles;
    const int movementCoefficient = 10;

    Rigidbody2D rb;
    Transform following;
    Vector2 velocity = Vector2.zero;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (following != null)
        {
            rb.AddForce((following.position - transform.position).normalized * movementCoefficient);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CubePlayer p = collision.gameObject.GetComponent<CubePlayer>();
        if (p != null)
        {
            if (p.Dashing)
            {
                Instantiate(deathParticles, transform.position, Quaternion.identity, null);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) following = collision.transform;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (following == collision.transform) following = null;
    }
}
