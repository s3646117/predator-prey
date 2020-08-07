using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Cohesion")]
public class Cohesion : FilteredFlockBehaviour
{

    // Steering behaviour to smooth movement
    Vector2 currentVelocity;
    public float agentSmoothTime = 0.5f;

    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
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
