using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Composite")]
public class Composite : FishBehaviour
{

    public FishBehaviour[] behaviours;
    public float[] weights;

    public override Vector2 CalculateMove(Agent agent, List<Transform> context, FishFSM fish)
    {
        // Check size of behaviours and weights is equal
        if (weights.Length != behaviours.Length)
        {
            Debug.LogError("Data mismatch in " + name, this);
            return Vector2.zero;
        }

        // Composite move
        Vector2 move = Vector2.zero;

        for (int i = 0; i < behaviours.Length; ++i)
        {
            Vector2 partialMove = behaviours[i].CalculateMove(agent, context, fish) * weights[i];

            if (partialMove != Vector2.zero)
            {
                // Limit magnitudde of partial move to extent of weight
                if (partialMove.sqrMagnitude > weights[i] * weights[i])
                {
                    partialMove.Normalize();
                    partialMove *= weights[i];
                }

                move += partialMove;
            }
        }

        return move;
    }

}
