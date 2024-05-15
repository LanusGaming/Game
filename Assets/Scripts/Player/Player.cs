using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public float speed = 10f;

    [HideInInspector]
    public bool active = false;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (active)
            Move();
        else
            rb.velocity = Vector3.zero;
    }

    private void Move()
    {
        Vector2 velocity = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
            velocity.y += 1;
        if (Input.GetKey(KeyCode.S))
            velocity.y += -1;
        if (Input.GetKey(KeyCode.D))
        {
            velocity.x += 1;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        }
        if (Input.GetKey(KeyCode.A))
        {
            velocity.x += -1;
            transform.rotation = Quaternion.LookRotation(Vector3.back, Vector3.up);
        }

        rb.velocity = velocity.normalized * speed;
    }
}
