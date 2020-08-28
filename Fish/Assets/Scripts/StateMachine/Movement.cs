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
public class Movement : Manager
{
    public Rigidbody2D rb;
    public Status state;
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


    private GameObject attractedFish;
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
    }

    private void Update()
    {
        myPosition = gameObject.transform.position;
        getHookPosition = GameObject.FindGameObjectWithTag("Hook").transform.position;
        float distance = Vector2.Distance(gameObject.transform.position, hookPosition);
        feelHook();

        updaeState();
        switch (state)
        {
            case Status.Wander:
                steeringForce = FlockForce();
                avoid();
                break;

            case Status.Flee:
                steeringForce = FlockForce();
                steeringForce += FleeForce();
                avoid();
                break;

            case Status.Chase:
                steeringForce = FlockForce();
                steeringForce = SeekForce(hookPosition);
                break;


        }

/*        steeringForce = SeekForce(getHookPosition);*/
        Move(steeringForce);

        
    }

    void updaeState()
    {
        leaveS = false;
        putHook = false;
        wander = false;
        chase = false;


        if (wander==true)
        {
            state = Status.Wander;
            flee = false;
            if(feelHook()==true)
            {
                flee = true;
                wander = false;
/*                Debug.Log("hook");*/
            }
        }
        if (flee == true)
        {
            state = Status.Flee;
            count = true;
            if(count==true)
            {
                StartCoroutine(coounter());
                count = false;
            }

        }
        if (chase == true)
        {
            state = Status.Chase;
        }
    }

    bool leaveS = false;
    bool count = false;
    IEnumerator coounter()
    {
        yield return new WaitForSeconds(3f);
        state = Status.Wander;
        flee = false;

    }

    public bool putHook;
    public bool feelHook()
    {

        float distance = Vector2.Distance(move.hookPosition, move.myPosition);
        if (distance <= move.fleeRange && !putHook)
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

    float mainWhiskerLen = 1.25f;
    float wallAvoidDistance = 0.5f;
    float sideWhiskerLen = 0.7f;
    float sideAngle = 45f;
    float maxAcceleration = 40f;

    public void Avoid()
    {
        if(Physics2D.Raycast(transform.position,transform.forward,5))
        {

        }
    }

    RaycastHit hitFoward, hitRight, hitLeft;
    //射线长度，控制距离障碍物多远的时候开始触发躲避
    float rayLength;
    //碰到障碍物时的反向作用力
    Vector3 reverseForce;
    //物体自身的速度
    public Vector2 velocitySelf;
    //判断是否在进行转弯
    bool IsTurn;

    public void avoid()
    {

        Debug.DrawLine(transform.position, transform.position + transform.forward, Color.cyan);
        Debug.DrawLine(transform.position, transform.position + (transform.forward + transform.right).normalized, Color.cyan);
        Debug.DrawLine(transform.position, transform.position + (transform.forward - transform.right).normalized, Color.cyan);

        if (Physics.Raycast(transform.position, transform.forward, out hitFoward, rayLength))
        {
            //Raycast.normal表示光线射到此表面时，在此处的法线单位向量
            reverseForce = hitFoward.normal * (rayLength - (hitFoward.point - transform.position).magnitude);
            IsTurn = true;
        }
        if (Physics.Raycast(transform.position, transform.forward + transform.right, out hitFoward, rayLength))
        {
            reverseForce = hitFoward.normal * (rayLength - (hitFoward.point - transform.position).magnitude);
            IsTurn = true;
        }
        if (Physics.Raycast(transform.position, transform.forward - transform.right, out hitFoward, rayLength))
        {
            reverseForce = hitFoward.normal * (rayLength - (hitFoward.point - transform.position).magnitude);
            IsTurn = true;
        }
        if (!IsTurn)
        {
            reverseForce = Vector3.zero;
            //通过这个控制当物体躲避完障碍物以后速度变为原来的速度，为防止物体的速度越来越大
            velocitySelf = velocitySelf.normalized * (new Vector3(3, 0, 3).magnitude);
        }
        velocitySelf += (Vector2)reverseForce;
        Move(velocitySelf);
        IsTurn = false;

    }

/*    public Vector2 rotateToVector(float orientation)
    {
        return new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation));
    }

    public bool FindObstacle(Vector2[]rayDirs, out RaycastHit firstHit)
    {
        firstHit = new RaycastHit();
        bool foundObs = false;

        for(int i=0; i<rayDirs.Length;++i)
        {
            int rayDis = (int)((i == 0) ? mainWhiskerLen : sideWhiskerLen);
            RaycastHit hit;
            if(Physics.Raycast(transform.position, rayDirs[i],out hit, rayDis))
            {
                foundObs = true;
                firstHit = hit;
                break;
            }
            Debug.DrawLine(transform.position, transform.position*rayDirs[i] * rayDis);
        }
        return foundObs;
    }*/
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
