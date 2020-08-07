using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Alignment")]
public class Alignment : FilteredFlockBehaviour
{

    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        // No neighbours
        if (context.Count == 0)
            return agent.transform.up;

        // Calcuate average between all neighbours
        Vector2 alignmentMove = Vector2.zero;
        // If filter is not null, use filtered context instead
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
        foreach (Transform item in filteredContext)
            alignmentMove += (Vector2)item.transform.up;

        // Average position
        alignmentMove /= context.Count;

        return alignmentMove;
    }
}
