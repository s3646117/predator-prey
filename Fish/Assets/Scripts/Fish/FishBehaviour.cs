using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FishBehaviour : ScriptableObject
{

    public abstract Vector2 CalculateMove(Agent agent, List<Transform> context, FishFSM fish);


}
