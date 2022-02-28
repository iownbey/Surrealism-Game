using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstCircle : MonoBehaviour
{
    public GameObject deathParticles;
    public int movementCoefficient = 10;
    public float volleyDelay = 5;
    public float shotDelay = 0;
    public int bulletsPerVolley = 6;
    public float initialBulletVelocity = 5;

    public Rigidbody2D bulletPrefab;

    IEnumerator volley;

    Rigidbody2D rb;
    Transform following;

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

    IEnumerator Volley()
    {
        while (true)
        {
            for (int i = 0; i < bulletsPerVolley; i++)
            {
                Vector2 d = Vector2.up.Rotate((360 / bulletsPerVolley) * i) * initialBulletVelocity;
                Rigidbody2D bullet = Instantiate(bulletPrefab,transform.position, Quaternion.identity);
                bullet.velocity = d;

                yield return new WaitForSeconds(shotDelay);
            }
            yield return new WaitForSeconds(volleyDelay);
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
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(volley = Volley());
            following = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (following == collision.transform)
        {
            StopCoroutine(volley);
            following = null;
        }
    }
}
