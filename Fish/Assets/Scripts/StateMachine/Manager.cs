using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Vector2 targetPosition;
    public GameObject hookPrefab;

    GameObject target;

    public GameObject fishPrefab;
    public Vector2 fishPosition;

    public float fishNum = 50;
    const float AgentDensity = 0.08f;

    private void Start()
    {

        target = Instantiate(hookPrefab, targetPosition, Quaternion.identity);
        createAgent();

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

    }

    void Update()
    {
        InputChangeTarget();
    }
}
