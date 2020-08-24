using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Filter/Same Flock")]
public class SameFlockFilter : ContextFilter
{
    public override List<Transform> Filter(Agent agent, List<Transform> original)
    {
        List<Transform> filtered = new List<Transform>();
        foreach (Transform item in original)
        {
            Agent itemAgent = item.GetComponent<Agent>();
            if(itemAgent != null && itemAgent.agentFish == agent.agentFish)
            {
                filtered.Add(item);
            }
        }

        return filtered;
    }
}
