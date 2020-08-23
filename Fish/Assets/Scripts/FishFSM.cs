/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FishFSM : MonoBehaviour
{
    public Vector2 myPosition;
    public Vector2 target;
    public Vector2 velocity;
    public Vector2 desiredVelocity;
    public float seekRange = 20f;
    public float approachRange = 2.5f;
    public float fleeRange = 0.2f;


    public float acceleration = 0.1f;
    public float turningForce = 0.1f;

    private Rigidbody2D rb;


    public FlockAgent agentPrefab;
    List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehaviour behaviour;

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

    FSMManager fsm;

    private void Awake()
    {
        fsm = this.GetComponent<FSMManager>();
    }

    // Use this for initialization
    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        // Create flock
        for (int i = 0; i < startingCount; ++i)
        {
            float x = Random.Range(-5f, 5f);
            float y = Random.Range(-5f, 5f);
            Vector2 pos = new Vector2(x, y);

            FlockAgent agent = Instantiate
                (
                agentPrefab,
                Random.insideUnitCircle * startingCount * AgentDensity,
                Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                transform
                );
            agent.Initialize(this);
            agents.Add(agent);
        }

        target = GetMousePosition();
        rb = GetComponent<Rigidbody2D>();
        velocity = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        flock();
        target = GetMousePosition();
        myPosition = gameObject.transform.position;

        float distance = Vector2.Distance(gameObject.transform.position, target);

        //if (distance < fleeRange)
        //Flee(target);
        if (distance < seekRange)
            Seek(target);
        else
            Idle(target);
    }


    void flock()
    {
        foreach (FlockAgent agent in agents)
        {
            List<Transform> context = GetNearbyObjects(agent);

            Vector2 move = behaviour.Calculate(agent, context, this);
            move *= driveFactor;
            if (move.sqrMagnitude > squareMaxSpeed)
                move = move.normalized * maxSpeed;

            agent.Move(move);
        }
    }


    List<Transform> GetNearbyObjects(FlockAgent agent)
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
    public void AddFlockAgent(FlockAgent agent)
    {
        agents.Add(agent);
    }
    public void RemoveFlockAgent(FlockAgent agent)
    {
        agents.Remove(agent);
    }

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
        // ???? just zeroing out the velocity here feels stupid
        //rb.velocity = Vector2.zero;


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

    Vector2 GetMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);

        return worldPoint;
    }
}
*/