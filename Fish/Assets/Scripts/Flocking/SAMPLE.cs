using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAMPLE : MonoBehaviour
{
    private Flock flock;

    public FlockAgent agentPrefab;

    public FlockBehaviour behaviour;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        flock.flock();   
    }
}
