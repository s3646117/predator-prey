using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public enum States
{
    Chase,
    Wander,
    Seek,
    Flee
}
public class Manager : MonoBehaviour
{
    public static Vector2 targetPosition;
    public GameObject hookPrefab;

    GameObject target;

    public GameObject fishPrefab;
    public Vector2 fishPoisition;

    public int fishNum = 50;
    const float AgentDensity = 0.08f;

    public int randNum;

    public States states;

    public Movement move;
    public Vector2 force;

    public float currentTime=120;
    private void Start()
    {
        target = Instantiate(hookPrefab, targetPosition, Quaternion.identity);
        createAgent();
/*        getAttractedFish();*/
        states = States.Wander;

    }

    void Update()
    {
        InputChangeTarget();

        switch (states)
        {
            case States.Wander:
                move.wander = true;
                break;
            case States.Flee:
                move.flee = true;
                getAttractedFish();
                break;
            case States.Chase:
                move.chase = true;
                break;

        }

    }

    public GameObject getAttractedFish()
    {
        GameObject[] fishGroup = GameObject.FindGameObjectsWithTag("Fish");
        randNum = Random.Range(0, fishNum);
        GameObject fish;
        fish = fishGroup[randNum];
        Debug.Log(randNum);
        return fish;
    }

    public bool feelHook()
    {
        float distance = Vector2.Distance(move.hookPosition, move.myPosition);
        if (distance <= move.fleeRange)
        {
            return true;
        }

        else
        {
            return false;
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
        }
        float inputY = Input.GetAxis("Vertical");
        move.putHook = true;
    }
}
