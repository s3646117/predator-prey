using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Status/Flock")]
public class Flock : MonoBehaviour
{
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

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;
        status = Status.Flocking;
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
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 sf = new Vector2();
        sf = Seek(Manager.targetPosition);

        switch(status)
        {
            case Status.Flocking:
                flock();
                break;

        }
    }



    public void flock()
    {
        foreach (FlockAgent agent in agents)
        {
            List<Transform> context = GetNearbyObjects(agent);

            Vector2 move = behaviour.CalculateMove(agent, context, this);
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

    Vector2 getTarget()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);

        return worldPoint;
    }

    void Move(Vector2 newVelocity)
    {
        rb.AddForce(transform.up * maxSpeed);
        Turn(newVelocity);
    }
    void Turn(Vector2 velocity)
    {
        float toRotation = (Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90);
        float rotation = Mathf.LerpAngle(rb.rotation, toRotation, Time.deltaTime * turningForce);

        rb.MoveRotation(rotation);
    }

    Vector2 Seek(Vector2 target)
    {
        Vector2 diff = targetPosition - (Vector2)transform.position;
        Vector2 desiredVelocity = diff.normalized * maxSpeed;

        return desiredVelocity - rb.velocity;
    }
}
