using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public enum Status
{
    Chase,
    Wander,
    Seek,
    Flee
    
}
public class Movement : FishStateMachine
{
    public Rigidbody2D rb;
    public Status state;
    public Vector2 velocity;
    public Vector2 target;
    public float seekRange = 20f;
    public int fleeRange = 2;
    public float fleeSpeed = 6f;
    float moveSpeed = 3.0f;
    float turnForce = 5f;

    public Vector2 myPosition;
    public Vector2 steeringForce;
    public Vector2 hookPosition;

    public int timeLeft;
    public int timeCount;

    public GameObject fishPrefab;
    public Vector2 fishPosition;

    public float fishNum=10;
    const float AgentDensity = 0.08f;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        velocity = Vector2.zero;
        state = Status.Wander;
        timeLeft = 3;
    }

    private void Update()
    {
        myPosition = gameObject.transform.position;
        hookPosition = GameObject.FindGameObjectWithTag("Hook").transform.position;
        float distance = Vector2.Distance(gameObject.transform.position, target);


        switch (state)
        {
            case Status.Chase:
                steeringForce = SeekForce(hookPosition);
                break;
            case Status.Flee:
                steeringForce = FlockForce();
                steeringForce += FleeForce();
                timeCount = timeLeft - 1;
                if (timeCount == 0)
                {
                    state = Status.Wander;
                    steeringForce = randomSelectFish();
                }

                break;
            case Status.Wander:
                steeringForce = FlockForce();
                if (feelHook())
                {
                    state=Status.Flee;
                }
                break;
            case Status.Seek:
                break;
        }

        Move(steeringForce);
    }

    public bool feelHook()
    {
        float distance = Vector2.Distance(hookPosition, myPosition);
        if(distance <= fleeRange)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    Vector2 randomSelectFish()
    {
        GameObject[] fishGroup = GameObject.FindGameObjectsWithTag("Fish");
        List<Transform> AttractedFish = new List<Transform>();
        GameObject fish;
        int index = Random.Range(0, fishGroup.Length);
        fish = fishGroup[index];
        AttractedFish.Add(fish.transform);
        Vector2 force = Vector2.zero;
        force += SeekForce(hookPosition);
        return force;
    }

    Vector2 SeekForce(Vector2 targetPosition)
    {
        Vector2 myPosition = transform.position;
        Vector2 diff = targetPosition - myPosition;
        Vector2 desiredVelocity = diff.normalized * moveSpeed;

        return desiredVelocity - rb.velocity;
    }

    Vector2 FleeForce()
    {
        Vector2 hookPosition = GameObject.FindGameObjectWithTag("Hook").transform.position;
        Vector2 desiredVelocity = (Vector2)transform.position - hookPosition;
        Vector2 force = desiredVelocity.normalized * fleeSpeed;
        return force-rb.velocity;
    }

    Vector2 AvoidForce()
    {
        Vector2 hookPosition = GameObject.FindGameObjectWithTag("Obstacle").transform.position;
        Vector2 desiredVelocity = (Vector2)transform.position - hookPosition;
        Vector2 force = desiredVelocity.normalized * fleeSpeed;
        return force - rb.velocity;
    }

/*    bool turn;*/
/*    Vector2 Avoid()
    {

    }*/

    void Move(Vector2 force)
    {
        Vector2 velocityNor = rb.velocity.normalized;
        Vector2 force1 = Vector2.Dot(force, velocityNor) * velocityNor;
        if(Vector2.Dot(force,velocityNor)<0)
        {
            force -= force1;
        }
        rb.AddForce(force);
        Turn(velocityNor);
    }

    void Turn(Vector2 velocity)
    {
        // Calculate rotation from velocity vector
        float toRotation = (Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90);
        float rotation = Mathf.LerpAngle(rb.rotation, toRotation, Time.deltaTime * turnForce);

        rb.MoveRotation(rotation);
    }


    Vector2 Separation(List<Transform>fishGroup)
    {
        if(fishGroup.Count==0)
        {
            return Vector2.zero;
        }

        Vector2 force = Vector2.zero;
        foreach(var fish in fishGroup)
        {
            Vector2 dis = transform.position - fish.position;
            force += dis.normalized / dis.magnitude;
        }

        return force;
    }
    
    Vector2 Alignment(List<Transform>fishGroup)
    {
        if (fishGroup.Count == 0)
        {
            return Vector2.zero;
        }
        Vector2 average = Vector2.zero;
        int fishNum = 0;
        foreach(var fish in fishGroup)
        {
            average += fish.GetComponent<Rigidbody2D>().velocity.normalized;
            fishNum++;
        }
        return average / fishNum;
    }

    Vector2 Cohesion(List<Transform>fishGroup)
    {
        if(fishGroup.Count==0)
        {
            return Vector2.zero;
        }

        Vector2 center = Vector2.zero;
        int num = 0;
        foreach (var bird in fishGroup)
        {
            center += (Vector2)bird.position;
            num++;
        }
        center /= num;
        Vector2 force = SeekForce(center);
        return force;

    }

    float flockingDist = 3.0f;
    Vector2 FlockForce()
    {
        List<Transform> neighbours = new List<Transform>();
        GameObject[] fishGroup = GameObject.FindGameObjectsWithTag("Fish");
        foreach (var fish in fishGroup)
        {
            if (fish == gameObject)
            {
                continue;
            }
            if (Vector2.Distance(transform.position, fish.transform.position) < flockingDist)
            {
                neighbours.Add(fish.transform);
            }
        }
        Vector2 force = Vector2.zero;
        force += Separation(neighbours);
        force += Alignment(neighbours);
        force += Cohesion(neighbours) * 2;
        return force;
    }
}
