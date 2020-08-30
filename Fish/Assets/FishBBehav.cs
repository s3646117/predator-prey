using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishBBehav : MonoBehaviour
{
    public Rigidbody2D rb;
    public float maxSpeed = 10f;
    public float turningForce = 0.2f;
    public Vector3 targetPosition;
    public float SeekRange=20.0f;

    public float accelerationTime = 3f;

    public Vector2 randomM;
    public float timeLeft;

    public int inRange = 35;
    public Vector2 Obstacle;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {

        targetPosition = GameObject.FindGameObjectWithTag("Hook").transform.position;
        Obstacle = GameObject.FindGameObjectWithTag("Obstacle").transform.position;

        checkDisToTarget(targetPosition);
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            randomM = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            timeLeft += accelerationTime;
        }

/*        if (checkDisToTarget(targetPosition) > inRange)
            wander(randomM);
        else
            Chase(targetPosition);
*/
        if (feelObstacle() == true)

            Avoid(Obstacle);

        else
            wander(randomM);
    }


    public void wander(Vector2 randomTarget)
    {
        Move(randomTarget);
    }




    public void Chase(Vector3 target)
    {
        Vector2 desired = target - gameObject.transform.position;
        desired.Normalize();

        Vector2 steering = desired - rb.velocity;
        Vector2 newVelocity = rb.velocity + steering;
        Move(newVelocity);
        Turn(newVelocity);

    }


    void Avoid(Vector2 target)
    {
        Vector2 dir = (Vector2)gameObject.transform.position - target;
        dir.Normalize();

        Move(dir);
    }


    float checkDisToTarget(Vector2 targetPosition)
    {
        float dis = Vector2.Distance(gameObject.transform.position,targetPosition);
        return dis;
    }


    bool feelObstacle()
    {
        Vector2 Obstacle = GameObject.FindGameObjectWithTag("Obstacle").transform.position;
        if (checkDisToTarget(Obstacle) < 50.0)
        {
            return true;
        }
        else
            return false;
    }

    public Vector2 Arrive(Vector2 targetPosition)
    {
        Vector2 diff = targetPosition - (Vector2)transform.position;
        float distance = diff.magnitude;

        if(distance<=1)
        {
            return Vector2.zero;
        }
        float arriveSpeed = distance / 10f;
        if(arriveSpeed>maxSpeed)arriveSpeed = maxSpeed;
        return diff.normalized * arriveSpeed - rb.velocity;
    }


    void Move(Vector2 targetPosition)
    {
        transform.up = targetPosition;
        transform.position += (Vector3)targetPosition * Time.deltaTime * maxSpeed;
        Turn(targetPosition);
    }

    void Turn(Vector2 velocity)
    {
        // Calculate rotation from velocity vector
        float toRotation = (Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg);
  /*      transform.rotation = Quaternion.AngleAxis(toRotation + 270, Vector3.forward);*/
        float rotation = Mathf.LerpAngle(rb.rotation, toRotation, Time.deltaTime * turningForce);
        rb.MoveRotation(rotation);
    }
}
