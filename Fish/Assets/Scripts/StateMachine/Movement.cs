using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public enum moveState
{
    Wander,
    Flee,
    Chase
}
public class Movement : MonoBehaviour
{
    public Rigidbody2D rb;
    public moveState state;
    public Vector2 velocity;
    public Vector2 hookPosition;

    public float seekRange = 20f;
    public int fleeRange = 5;
    public float fleeSpeed = 9f;
    float moveSpeed = 3.0f;
    float turnForce = 5f;

    public float avoidSpeed = 4.0f;
    public Vector2 myPosition;
    Vector2 steeringForce;
    public Vector2 getHookPosition;

    private float timeLeft;

    public RippleClick rc;

    private GameObject attractFish;
    public Manager manager;
  
    const float AgentDensity = 0.08f;


    public bool flee = false;
    public bool wander = false;
    public bool chase = false;
    public bool timeCount;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        velocity = Vector2.zero;
        attractFish = manager.getAttractedFish();
    }
    public bool gotRipple;
    private void Update()
    {
        myPosition = gameObject.transform.position;
        getHookPosition = GameObject.FindGameObjectWithTag("Hook").transform.position;
        float distance = Vector2.Distance(gameObject.transform.position, hookPosition);

  
        checkRippleClick();
        switch (state)
        {
            case moveState.Wander:
                if(wander==true)
                {
                    /* Debug.Log("Wander State");*/

                    /* Flocking */
                    /*  TODO */

                    /* Check hook position */
                    feelHook();
                    if(feelHook()==true)
                    {
                        wander = true;

                        /* Debug.Log("State change to Flee");*/
                        /* if near hook, FLEE */
                        state = moveState.Flee;
                    }

                }
                break;

            case moveState.Flee:
                if(flee==true)
                {
                    /*  Debug.Log("Flee State");*/
                    steeringForce += FleeForce();

                    /* Debug.Log("Count");*/
                    /* count second */
                    coounter();

                    /* Debug.Log("Back to Wander State");*/
                    /* After count, back to wander */
                    state = moveState.Wander;

                    /* After Flee and back to wander, Check Ripple */
                    checkRippleClick();
                    if (checkRippleClick() == true)
                    {
                        /* Debug.Log("Change to Chase state");*/
                        /* Go to Chase state */
                        state = moveState.Chase;
                    }
                }
                break;

            case moveState.Chase:
                if (chase==true)
                {
                    Debug.Log("move state is: chase");

                    /* Choose a attract fish & change tag to destroy, at Hook script check collider to destroy*/
                    attractFish = manager.getAttractedFish();
                    attractFish.tag = "Destroy";

                    /* The Fish chase to Hook? */
                    /*  TODO */
                    chase = false;
                }

                break;
        }

    }


    public bool checkRippleClick()
    {
        if(Input.GetMouseButtonDown(1))
        {
            return true;
        }

        else
        {
            return false;
        }
    }



    IEnumerator coounter()
    {
        yield return new WaitForSeconds(3f);

    }

    public bool putHook;
    public bool feelHook()
    {
        float distance = Vector2.Distance(getHookPosition, myPosition);
        if (distance <= fleeRange)
        {
            return true;
        }

        else
        {
            return false;
        }
    }


    public Vector2 SeekForce(Vector2 targetPosition)
    {
        Vector2 myPosition = transform.position;
        Vector2 diff = targetPosition - myPosition;
        Vector2 desiredVelocity = diff.normalized * moveSpeed;

        return desiredVelocity - rb.velocity;
    }

    public Vector2 FleeForce()
    {
        Vector2 hookPosition = GameObject.FindGameObjectWithTag("Hook").transform.position;
        Vector2 desiredVelocity = (Vector2)transform.position - hookPosition;
        Vector2 force = desiredVelocity.normalized * fleeSpeed;
        return force-rb.velocity;
    }


    public void Move(Vector2 force)
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

    public void Turn(Vector2 velocity)
    {
        // Calculate rotation from velocity vector
        float toRotation = (Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90);
        float rotation = Mathf.LerpAngle(rb.rotation, toRotation, Time.deltaTime * turnForce);

        rb.MoveRotation(rotation);
    }


    public Vector2 Separation(List<Transform>fishGroup)
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

    public Vector2 Alignment(List<Transform>fishGroup)
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

    public Vector2 Cohesion(List<Transform>fishGroup)
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

    public float flockingDist = 3.0f;
    public Vector2 FlockForce()
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
