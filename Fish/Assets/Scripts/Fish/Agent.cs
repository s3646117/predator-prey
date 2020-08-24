using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{

    Collider2D agentCollider;
    public Collider2D AgentCollider { get { return agentCollider; } }

    public FishFSM agentFish;



    // Use this for initialization
    void Start()
    {
        agentCollider = GetComponent<Collider2D>();
    }

    public void Initialize(FishFSM fish)
    {
        agentFish = fish;
    }

    public void Move(Vector2 velocity)
    {
        transform.up = velocity;
        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    public void JoinFlock()
    {
        agentFish.AddFlockAgent(this);
    }

    public void LeaveFlock()
    {
        agentFish.RemoveFlockAgent(this);
    }

}
