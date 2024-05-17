using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [HideInInspector]
    public float damage;
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public float acceleration;
    [HideInInspector]
    public float lifetime = 5f;

    private Rigidbody2D rb;
    private float timer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.right * (acceleration * timer + speed);

        timer += Time.fixedDeltaTime;

        if (timer > lifetime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player.instance.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
