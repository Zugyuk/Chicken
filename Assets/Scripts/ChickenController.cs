using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class ChickenController : MonoBehaviour
{
    // Move player in 2D space
    [SerializeField] private float currentSpeed = 5f;
    private Animator animator;

    bool facingRight = true;
    private float moveDirectionH = 0;
    private float moveDirectionV = 0;
    Rigidbody2D r2d;
    Collider2D mainCollider;
    Transform t;

    // Use this for initialization
    void Start()
    {
        t = transform;
        r2d = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        
        facingRight = t.localScale.x > 0;
    }

    void Update()
    {        
        // animate if moving
        if (moveDirectionH != 0 || moveDirectionV != 0) {animator.SetBool("IsWalking", true);}
        else {animator.SetBool("IsWalking", false);}
        
        // Movement controls that trigger move direction variables.
        if (Input.GetKey(KeyCode.D)) {moveDirectionH = 1;}
        else if(Input.GetKey(KeyCode.A)) {moveDirectionH = -1;}
        else {moveDirectionH = 0;}

        if (Input.GetKey(KeyCode.W)){moveDirectionV = 1;}
        else if (Input.GetKey(KeyCode.S)) {moveDirectionV = -1;}
        else {moveDirectionV = 0;}

        // facing
        if (moveDirectionH != 0)
        {
            if (moveDirectionH > 0 && !facingRight)
            {
                facingRight = true;
                t.localScale = new Vector3(Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
            }
            else if (moveDirectionH < 0 && facingRight)
            {
                facingRight = false;
                t.localScale = new Vector3(-Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
            }
        }
    }

    void FixedUpdate()
    {
        Bounds colliderBounds = mainCollider.bounds;

        // Apply movement velocity
        r2d.velocity = new Vector2((moveDirectionH * currentSpeed), (moveDirectionV * currentSpeed));
        print(r2d.velocity);
    }
}