using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public enum ManagerStates
{
    Wander,
    Seek,
    Flee,
    Chase
}
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

    public ManagerStates states;

    public Movement move;
    public Vector2 force;
    public bool getRipple;
    public float currentTime=120;
    public GameObject attractedFish;
    private void Start()
    {
        target = Instantiate(hookPrefab, targetPosition, Quaternion.identity);
        createAgent();
    }

    void Update()
    {
        InputChangeTarget();
        move.wander = true;
        move.state = moveState.Wander;
    }
    
    public GameObject getAttractedFish()
    {
        GameObject[] fishGroup = GameObject.FindGameObjectsWithTag("Fish");
        randNum = Random.Range(0, fishGroup.Length);
        GameObject fish = fishGroup[randNum];
        return fish;
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
