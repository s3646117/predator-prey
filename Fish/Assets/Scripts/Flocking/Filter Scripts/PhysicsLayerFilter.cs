using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Filter/Physics Layer")]
public class PhysicsLayerFilter : ContextFilter
{

    public LayerMask layerMask;

    public override List<Transform> Filter(FlockAgent agent, List<Transform> fullContext)
    {
        List<Transform> filteredContext = new List<Transform>();
        foreach (Transform t in fullContext)
        {
            // If objects are on the same layer, add to filteredContext
            if (layerMask == (layerMask | (1 << t.gameObject.layer)))
                filteredContext.Add(t);
        }

        return filteredContext;
    }

}
