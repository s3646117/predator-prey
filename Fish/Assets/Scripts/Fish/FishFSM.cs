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

    public Vector2 target;
    public Rigidbody2D rb;
    public Vector2 targetPosition;
    private float mSpeed = 5.0f;
    public float turningForce = 0.4f;

    public float disToHook = 20.0f;
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

        myPosition = gameObject.transform.position;

        /* status */
        status = Status.Flocking;

        /* target */


        // Create fish group
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


    }

    // Update is called once per frame
    void Update()
    {
        /* target */
        Vector2 sf = new Vector2();
   
        Obstacle = GameObject.FindGameObjectWithTag("Obstacle").transform.position;

        myPosition = gameObject.transform.position;
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
                if (feelHook() == true)
                {
                    Debug.Log("Feel hook");
                    status = Status.Flee;
                }

                break;
            case Status.Flee:
                flee();
                break;
            case Status.Seek:

                break;
        }

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

    public float fleeSpeedS = 3.0f;
    public void flee()
    {
        Vector2 dirV = (myPosition - getHookPosition()).normalized*3.0f ;

        foreach(Agent agent in agents)
        {
            agent.Move(fleeGet(dirV));

        }
    }

    public void seek()
    {
        foreach (Agent agent in agents)
        {
            
        }
    }

    float fleeRange =5.0f;
    bool decelerateOnStop = true;
    float timeToTarget = 0.1f;
    public Vector2 fleeGet(Vector2 targetPosition)
    {
        Vector2 mPosition = gameObject.transform.position;

        /* get direction */
        Vector2 dir = mPosition - targetPosition;
/*        dir.Normalize();*/
        if (dir.magnitude > fleeRange)
        {
            if (decelerateOnStop && rb.velocity.magnitude > 0.001f)
            {
                if (dir.magnitude > maxFleeSpeed)
                {
                    dir = -rb.velocity / timeToTarget;
                }
                return dir;
            }
            else
            {
                rb.velocity = Vector2.zero;
                return Vector2.zero;
            }
        }
        return fleeSpeed(dir);

    }

    public float maxFleeSpeed = 3.0f;
    public Vector2 fleeSpeed(Vector2 velocity)
    {
        velocity.Normalize();
        velocity *= maxFleeSpeed;
        return velocity;
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


  

    public Vector2 Seek(Vector2 target)
    {
        Vector2 desired = target - myPosition;
        desired.Normalize();

        Vector2 steering = desired - rb.velocity;
        Vector2 newVelocity = rb.velocity + steering;
        return newVelocity;
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
        float distance = Vector2.Distance(myPosition, target);
        if (distance < disToHook)
        {
            return true;
        }
        else
        { return false; }
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

    public Vector2 getHookPosition()
    {
        targetPosition = GameObject.FindGameObjectWithTag("Hook").transform.position;
        return targetPosition;
    }


    float checkDisToTarget(Vector2 targetPosition)
    {
        float dis = Vector2.Distance(gameObject.transform.position, targetPosition);
        return dis;
    }
}
