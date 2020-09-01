using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;


public class Manager : MonoBehaviour
{
    public static Vector2 targetPosition;
    public GameObject hookPrefab;

    GameObject target;

    public GameObject fishPrefab;
    public Vector2 fishPoisition;

    public int fishNum = 10;
    const float AgentDensity = 0.08f;

    public int randNum;

    public Movement move;
    public Vector2 force;
    public bool getRipple;
    public float currentTime=120;
    public GameObject attractedFish;

    public GameObject fishPathPrefab;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Hook");
        //createAgent();
    }

    void Update()
    {
        InputChangeTarget();
        checkRippleClick();
        //move.wander = true;
        //move.state = moveState.Wander;
    }
    
    public void AttractFish()
    {
        GameObject[] fishGroup = GameObject.FindGameObjectsWithTag("Fish");
        randNum = Random.Range(0, fishGroup.Length);
        GameObject fish = fishGroup[randNum];
        /* Choose a attract fish & change tag to destroy, at Hook script check collider to destroy*/
        fish.tag = "Destroy";

        // Trigger change in state to moveState.Chase
        move = fish.GetComponent<Movement>();
        // Debug.Log("Change to Chase state");
        // Go to Chase state 
        move.state = moveState.Chase;
        move.flee = false;
        move.wander = false;
        move.chase = true;

        //Dirty fix for pathfinding issues
        FlockAgent agent = fish.GetComponent<FlockAgent>();
        agent.LeaveFlock();
        fish.SetActive(false);
        GameObject g = Instantiate(fishPathPrefab, fish.transform.position, fish.transform.rotation);
        DestroyFlockAgent d = g.GetComponent<DestroyFlockAgent>();
        d.flockAgent = agent;

        //return fish;
    }

    public void checkRippleClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            AttractFish();
        }

    }

    bool leaveState = false;
    IEnumerator counter()
    {
        leaveState = true;
        yield return new WaitForSeconds(5f);
    }

    void createAgent()
    {
        for (int i = 0; i < fishNum; ++i)
        {
            GameObject fish = Instantiate(fishPrefab, Random.insideUnitCircle * fishNum * AgentDensity, Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)), transform);

        }
    }
    void InputChangeTarget()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;

            Vector2 pos = Camera.main.ScreenToWorldPoint(mousePos);

            targetPosition = pos;
            target.transform.position = pos;

            float inputY = Input.GetAxis("Vertical");

            // ** Must be checked for every fish in scene **
            //move.putHook = true;
            Movement[] fishGroup = FindObjectsOfType<Movement>();
            foreach (Movement f in fishGroup)
                f.putHook = true;
 
        }
    }
}
