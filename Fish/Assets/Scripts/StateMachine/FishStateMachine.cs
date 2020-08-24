/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishStateMachine : MonoBehaviour
{
    Collider2D collider;
    public Collider2D getCollider { get { return collider; } }
    public GameObject agenPrefab;
    List<GameObject> agents = new List<GameObject>();
    
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

    private enum States
    {
        Wander,
        Flee,
        Chase
    };

    private States state;
    private GameObject target;
    void Start()
    {
        collider=GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case States.Wander:

                break;

            case States.Flee:
                break;

            case States.Chase:
                break;
        }
    }

    public void move(Vector2 velocity)
    {
        transform.up = velocity;
        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    public void joinFlock(GameObject agent)
    {
        agents.Add(agent);
    }

    public void leaveFlock(GameObject agent)
    {
        agents.Add(agent);
    }

    List<Transform>GetNearObject(GameObject agent)
    {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighborRadius);
        foreach (Collider2D c in contextColliders)
        {
            if (c != getCollider)
                context.Add(c.transform);
        }
        return context;
    }

    public Vector2 calculateCohesionMove(GameObject agent, List<Transform>context, Flock flock)
    {
        // No neighbours
        if (context.Count == 0)
            return Vector2.zero;

        // Calcuate average between all neighbours
        Vector2 cohesionMove = Vector2.zero;
        // If filter is not null, use filtered context instead
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
        foreach (Transform item in filteredContext)
            cohesionMove += (Vector2)item.position;

        // Average position
        cohesionMove /= context.Count;

        // Create offset from agent position
        cohesionMove -= (Vector2)agent.transform.position;

        // Smooth movement
        cohesionMove = Vector2.SmoothDamp(agent.transform.up, cohesionMove, ref currentVelocity, agentSmoothTime);

        return cohesionMove;
    }

}
*/