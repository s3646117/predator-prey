using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Vector2 targetPosition;
    public GameObject targetPrefab;
    GameObject target;

    float startPosY = 5;

    private void Start()
    {
        targetPosition = new Vector2(1, startPosY);
        target = Instantiate(targetPrefab, targetPosition, Quaternion.identity);
       
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
