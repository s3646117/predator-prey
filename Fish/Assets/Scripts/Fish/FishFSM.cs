using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FishFSM : MonoBehaviour
{
    public Agent agentPrefab;
    List<Agent> agents = new List<Agent>();
    public FishBehaviour behaviour;

    [Range(10, 500)]
    public int startingCount = 250;
    const float AgentDensity = 0.08f;

    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(0f, 100f)]
    public float maxSpeed = 5f;
    [Range(1f, 10f)]
    public float neighborRadius = 1.5f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

    public enum Status
    {
        Seek,
        Flee,
        Flocking
    }

    Status status;
    public GameObject gameobject;
    public Vector2 target;
    public Rigidbody2D rb;
    public Vector2 targetPosition;
    private float mSpeed = 5.0f;
    public float turningForce = 0.4f;

    public float disToHook = 40.0f;
    public Vector2 myPosition;

    public float SeekRange = 20.0f;

    public float accelerationTime = 3f;

    public Vector2 randomM;
    public float timeLeft;

    public int inRange = 35;
    public Vector2 Obstacle;

    public Vector2 velocity;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;



        /* status */
        status = Status.Flocking;

        /* target */
        target = GetTargetPosition();

        // Create flock
        for (int i = 0; i < startingCount; ++i)
        {
            float x = Random.Range(-5f, 5f);
            float y = Random.Range(-5f, 5f);
            Vector2 pos = new Vector2(x, y);
            velocity = Vector2.zero;
            Agent agent = Instantiate
                (
                agentPrefab,
                Random.insideUnitCircle * startingCount * AgentDensity,
                Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                transform
                );
            agent.Initialize(this);
            agents.Add(agent);
        }

        myPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        /* target */
        target = GetTargetPosition();
        Vector2 sf = new Vector2();
   
        Obstacle = GameObject.FindGameObjectWithTag("Obstacle").transform.position;

        checkDisToTarget(targetPosition);

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            randomM = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            timeLeft += accelerationTime;
        }

        switch (status)
        {
            case Status.Flocking:
                flock();
                if(feelHook()==true)
                {
                    status = Status.Seek;
                }
  
                break;
            case Status.Flee:

                break;
            case Status.Seek:
                Chase(targetPosition);
                break;
        }

    }

    private void FixedUpdate()
    {
        targetPosition = GameObject.FindGameObjectWithTag("Hook").transform.position;
    }
    public void flock()
    {
        foreach (Agent agent in agents)
        {
            List<Transform> context = GetNearbyObjects(agent);

            Vector2 move = behaviour.CalculateMove(agent, context, this);
            move *= driveFactor;
            if (move.sqrMagnitude > squareMaxSpeed)
                move = move.normalized * maxSpeed;

            agent.Move(move);
        }
    }



    List<Transform> GetNearbyObjects(Agent agent)
    {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighborRadius);
        foreach (Collider2D c in contextColliders)
        {
            if (c != agent.AgentCollider)
                context.Add(c.transform);
        }
        return context;
    }

    // Add/remove flock agent
    public void AddFlockAgent(Agent agent)
    {
        agents.Add(agent);
    }
    public void RemoveFlockAgent(Agent agent)
    {
        agents.Remove(agent);
    }


    public bool feelHook()
    {
        float distance = Vector2.Distance(myPosition, GetTargetPosition());
        if (distance < disToHook)
        {
            return true;
        }
        else
        { return false; }
    }




    public void Chase(Vector3 target)
    {
        Vector2 desired = target - gameObject.transform.position;
        desired.Normalize();

        Vector2 steering = desired - rb.velocity;
        Vector2 newVelocity = rb.velocity + steering;
        Move(newVelocity);
        Turn(newVelocity);

    }


    void Avoid(Vector2 target)
    {
        Vector2 dir = (Vector2)gameObject.transform.position - target;
        dir.Normalize();

        Move(dir);
    }


    float checkDisToTarget(Vector2 targetPosition)
    {
        float dis = Vector2.Distance(gameObject.transform.position, targetPosition);
        return dis;
    }


    bool feelObstacle()
    {
        Vector2 Obstacle = GameObject.FindGameObjectWithTag("Obstacle").transform.position;
        if (checkDisToTarget(Obstacle) < 50.0)
        {
            return true;
        }
        else
            return false;
    }



    void Move(Vector2 targetPosition)
    {
        rb.AddForce(transform.up * maxSpeed);

        Turn(targetPosition);
    }

    void Turn(Vector2 velocity)
    {
        float toRotation = (Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90);
        float rotation = Mathf.LerpAngle(rb.rotation, toRotation, Time.deltaTime * turningForce);

        rb.MoveRotation(rotation);
    }

    public Vector2 GetTargetPosition()
    {
        Vector2 targetPos = gameobject.transform.position;
        return targetPos;
    }

    void Seek(Vector2 target)
    {
        Vector2 desired = target - myPosition;
        desired.Normalize();

        Vector2 steering = desired - rb.velocity;
        Vector2 newVelocity = rb.velocity + steering;
        Move(newVelocity);
    }

    }
