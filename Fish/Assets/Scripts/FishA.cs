using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishA : Fish
{
    public Vector2 myPosition;
    public Vector2 target;
    public Vector2 velocity;
    public Vector2 desiredVelocity;
    public float seekRange = 20f;
    public float approachRange = 2.5f;
    public float fleeRange = 0.2f;

    public float maxSpeed = 10f;
    public float acceleration = 0.1f;
    public float turningForce = 0.1f;

    // DEBUG ONLY - REMOVE
    public bool flee = false;

    private Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        target = GetMousePosition();
        rb = GetComponent<Rigidbody2D>();
        velocity = Vector2.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        target = GetMousePosition();
        myPosition = gameObject.transform.position;

        float distance = Vector2.Distance(gameObject.transform.position, target);

        if(!flee)
            Seek(target);
        else
            Flee(target);

        Turn();
    }

    // Replace the following functions with CalculateMove(Behaviour, target vector)? -> not relevant once FSM is implemented?
    void Seek(Vector2 target)
    {
        //Vector2 desired = (target - myPosition).normalized * maxForce;
        Vector2 desired = target - myPosition;
        float distance = desired.magnitude;
        
        if(distance < approachRange)
        {
            // Inside approach range -> slow down
            desired = desired.normalized * maxSpeed * (distance / approachRange);
            Debug.Log(distance/approachRange);
        }
        else
        {
            // Outside approach range -> maintain speed
            desired = desired.normalized * maxSpeed;
        }

        Vector2 steering = desired - rb.velocity;

        //Debug.Log(steering.magnitude < maxForce);
        //steering = (steering.magnitude < maxForce) ? steering : steering.normalized * maxForce;
        //Debug.Log(steering);

        steering /= rb.mass;
        //rb.inertia?
        Vector2 newVelocity = rb.velocity + steering;
        if (newVelocity.magnitude > maxSpeed)
            newVelocity = newVelocity.normalized * maxSpeed;


        // Debug
        //Debug.DrawRay(transform.position, rb.velocity, Color.red);
        //Debug.DrawRay(transform.position, desired, Color.blue);
        //Debug.DrawRay(transform.position, steering, Color.green);

        Move(newVelocity);
    }

    void Flee(Vector2 target)
    {
        // Inverse of seek
        Vector2 desired = (myPosition - target).normalized * maxSpeed;

        Vector2 steering = desired - rb.velocity;

        steering /= rb.mass;

        Vector2 newVelocity = rb.velocity + steering;
        if (newVelocity.magnitude > maxSpeed)
            newVelocity = newVelocity.normalized * maxSpeed;

        Move(newVelocity);
    }

    void Idle(Vector2 target)
    {
        Vector2 dir = Vector2.zero;
        Move(dir);
    }

    void Move(Vector2 newVelocity)
    {
        // ???? just zeroing out the velocity here feels stupid
        //rb.velocity = Vector2.zero;

        //rb.AddForce(transform.up * maxSpeed);
        rb.velocity = newVelocity;
    }

    void Turn()
    {
        // Calculate rotation from velocity vector
        float toRotation = (Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg - 90);
        // Turning force???
        //float rotation = Mathf.LerpAngle(rb.rotation, toRotation, Time.deltaTime * turningForce);
         
        rb.MoveRotation(toRotation);
        
    }

    // Translate screen pos to world pos
    Vector2 GetMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);

        return worldPoint;
    }
}
