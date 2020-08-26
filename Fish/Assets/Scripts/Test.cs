using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject hook;
    public Vector2 hookPosition;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        getHookPosition();
    }

    public Vector2 getHookPosition()
    {
        hookPosition = GameObject.FindGameObjectWithTag("Hook").transform.position;
        return hookPosition;
    }

    public Vector2 getTargetPosition()
    {
        hookPosition = hook.transform.position;
        return hookPosition;
    }
}
