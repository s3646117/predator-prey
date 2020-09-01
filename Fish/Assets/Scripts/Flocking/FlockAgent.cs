﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockAgent : MonoBehaviour
{

    Collider2D agentCollider;
    public Collider2D AgentCollider { get { return agentCollider; } }

    public Flock agentFlock;
    public bool isFlocking;

    // Use this for initialization
    void Start()
    {
        agentCollider = GetComponent<Collider2D>();
    }

    public void Initialize(Flock flock)
    {
        agentFlock = flock;
    }

    public void Move(Vector2 velocity)
    {
        transform.up = velocity;
        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    public void JoinFlock()
    {
        agentFlock.AddFlockAgent(this);
        isFlocking = true;
    }

    public void LeaveFlock()
    {
        agentFlock.RemoveFlockAgent(this);
        isFlocking = false;
    }

}
