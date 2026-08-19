using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public void Lunch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }
}
