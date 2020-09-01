using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishA : Fish
{
    private Vector2 myPosition;
    private Vector2 target;
    private Vector2 step;
    private Vector2 velocity;
    private Vector2 desiredVelocity;
    public float approachRange = 2.5f;
    public float fleeRange = 0.2f;
    public float arrivedRange = 0.5f;

    public float maxSpeed = 10f;

    public bool isFlocking = true;
    public bool flee = false;

    private Rigidbody2D rb;

    public List<Node> path;
    public float pathWidth = 2.5f;
    
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
        if(!isFlocking)
            Steer();
    }

    void Steer()
    {
        //target = GetMousePosition();
        myPosition = gameObject.transform.position;

        //float distance = Vector2.Distance(gameObject.transform.position, target);

        if(!flee)
        {
            if (path != null && path.Count > 0)
            {
                step = path[0].position;
                target = GetComponent<Pathfinding>().target.position;
                Seek(target, step);
                //Seek(target);
            }
            else
            {
                target = GetComponent<Pathfinding>().target.position;
                Seek(target);
            }
        }
        else
        {
            Flee(target);
        }

        Turn();
    }

    void Seek(Vector2 target)
    {
        Vector2 desired = target - myPosition;
        float distance = desired.magnitude;
        
        if(distance < approachRange && distance > arrivedRange)
        {
            // Inside approach range -> slow down
            desired = desired.normalized * maxSpeed * (distance / approachRange);
        }
        else if (distance < arrivedRange)
        {
            // Inside approach range -> slow down
            desired = desired.normalized * maxSpeed * (distance / approachRange);
            if(path.Count > 0)
                path.RemoveAt(0);
        }
        else
        {
            // Outside approach range -> maintain speed
            desired = desired.normalized * maxSpeed;
        }

        Vector2 steering = desired - rb.velocity;
        steering /= rb.mass;

        Vector2 newVelocity = rb.velocity + steering;
        if (newVelocity.magnitude > maxSpeed)
            newVelocity = newVelocity.normalized * maxSpeed;

        Move(newVelocity);
    }

    // Path Seek
    void Seek(Vector2 target, Vector2 step)
    {
        Vector2 toTarget = target - myPosition;
        float distToTarget = toTarget.magnitude;

        Vector2 desired = step - myPosition;
        float distance = desired.magnitude;

        if (distToTarget < approachRange && distToTarget > arrivedRange)
        {
            // Inside approach range -> slow down
            desired = desired.normalized * maxSpeed * (distance / approachRange);
        }
        else if (distToTarget < arrivedRange)
        {
            // Inside approach range -> slow down
            desired = desired.normalized * maxSpeed * (distance / approachRange);
        }
        else
        {
            // Outside approach range -> maintain speed
            desired = desired.normalized * maxSpeed;
        }

        // Smoothing along path, move to next step in path
        if (distance < pathWidth)
            path.RemoveAt(0);

        Vector2 steering = desired - rb.velocity;
        steering /= rb.mass;

        Vector2 newVelocity = rb.velocity + steering;
        if (newVelocity.magnitude > maxSpeed)
            newVelocity = newVelocity.normalized * maxSpeed;

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
