﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ASample : Fish
{

    public Vector2 myPosition;
    public Vector2 target;
    public Vector2 velocity;
    public Vector2 desiredVelocity;
    public float seekRange = 20f;
    public float approachRange = 2.5f;
    public float fleeRange = 0.2f;

    public float maxSpeed = 10f;
    public float acceleration = 0.1f;
    public float turningForce = 0.1f;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        target = GetTargetPositioon();
        rb = GetComponent<Rigidbody2D>();
        velocity = Vector2.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        target = GetTargetPositioon();
        myPosition = gameObject.transform.position;

        float distance = Vector2.Distance(gameObject.transform.position, target);

        //if (distance < fleeRange)
        //Flee(target);
        if (distance < seekRange)
            Seek(target);
        else
            Idle(target);

    }


    // Replace the following functions with CalculateMove(Behaviour, target vector)? -> not relevant once FSM is implemented?
    void Seek(Vector2 target)
    {
        Vector2 desired = target - myPosition;
        desired.Normalize();

        Vector2 steering = desired - rb.velocity;
        Vector2 newVelocity = rb.velocity + steering;

        // Debug
        Debug.DrawRay(transform.position, rb.velocity, Color.red);
        Debug.DrawRay(transform.position, desired, Color.blue);
        Debug.DrawRay(transform.position, steering, Color.green);

        Move(newVelocity);
    }

    void Flee(Vector2 target)
    {
        Vector2 dir = myPosition - target;
        dir.Normalize();

        Move(dir);
    }

    void Idle(Vector2 target)
    {
        Vector2 dir = Vector2.zero;
        Move(dir);
    }

    void Move(Vector2 newVelocity)
    {
        rb.AddForce(transform.up * maxSpeed);
        Turn(newVelocity);
    }

    void Turn(Vector2 velocity)
    {
        // Calculate rotation from velocity vector
        float toRotation = (Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90);
        float rotation = Mathf.LerpAngle(rb.rotation, toRotation, Time.deltaTime * turningForce);

        rb.MoveRotation(rotation);

    }

    // Translate screen pos to world pos
    Vector2 GetMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);

        return worldPoint;
    }

    Vector2 GetTargetPositioon()
    {
        Vector2 targetPos = GameObject.FindGameObjectWithTag("Hook").transform.position;
        return targetPos;
    }
}
