using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFlockAgent : MonoBehaviour
{
    public FlockAgent flockAgent;
    
    public void Destroy()
    {
        flockAgent.LeaveFlock();
        Destroy(flockAgent.gameObject);
    }
}
