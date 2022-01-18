using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BehaviorOn
{
    Seek,
    Flee,
    Arrive,
    Pursuit,
    Evade,
    Wander
}

public enum Deceleration
{
    Slow = 3,
    Normal = 2,
    Fast = 1
}

public class SteeringBehaviours : MonoBehaviour
{
    [SerializeField] private BehaviorOn behaviorOn = BehaviorOn.Seek;
    [Header("Pursuit/Evade Behavior Switch")]
    [SerializeField] private bool targetIsEvader = false;
    [Header("Flee Behavior Attributes")]
    [SerializeField] private float fleeingRadius = 5.0f;
    [Header("Arrive Behavior Attributes")]
    [SerializeField] private float decelerationMultiplier = 0.3f;
    [SerializeField] private Deceleration decelerationRate = Deceleration.Normal;
    [Header("Wander Attributes")]
    [SerializeField] private float wanderRadius = 5.0f;  // radius of wander circle
    [SerializeField] private float wanderDistance = 2.5f;  // distance from wander circle
    [SerializeField] private float wanderJitterFactor = 2.0f;  // maximum amount of random displacement for the point on wander circle
    [SerializeField] private float wanderJitterRange = 3.0f;
    [SerializeField] private float wanderSpeedFactor = 0.5f;
    [Header("Obstacle Detection Attributes")]
    [SerializeField] private float obstacleDetectionBoxLength = 5.0f;
    [SerializeField] private LayerMask obstacleLayerMask;

    private MovingEntity entity = null;
    private EvaderController evader = null;
    private Vector3 targetPosition = Vector3.negativeInfinity;
    private Vector3 radius = Vector3.zero;

    private CapsuleCollider capsuleCollider = null;

    private void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        entity = GetComponent<MovingEntity>();
        var randomHorizontalLength = Random.Range(0, wanderRadius);
        var randomVerticalLength = Mathf.Sqrt(wanderRadius * wanderRadius - randomHorizontalLength * randomHorizontalLength);

        if (Random.Range(0, 2) == 0)
        {
            // negative
            randomHorizontalLength *= -1;
        }
        if (Random.Range(0, 2) == 0)
        {
            // negative 
            randomVerticalLength *= -1;
        }

        radius = new Vector3(randomHorizontalLength, 0, randomVerticalLength).normalized * wanderRadius;
    }

    private void Update()
    {
        if (targetIsEvader && evader == null)
        {
            evader = FindObjectOfType<EvaderController>();
        }
        else if (!targetIsEvader)
        {
            evader = null;
        }
    }

    public BehaviorOn GetBehaviourOn()
    {
        return behaviorOn;
    }

    public void SetTargetPosition(Vector3 pos)
    {
        if (targetIsEvader && (behaviorOn == BehaviorOn.Pursuit || behaviorOn == BehaviorOn.Evade))
        {
            return;
        }
        targetPosition = pos;              
    }

    public Vector3 CalculateTotalForce()
    {
        if (targetPosition == Vector3.negativeInfinity) { return Vector3.zero; }

        switch (behaviorOn)
        {
            case BehaviorOn.Seek:
                return Seek(targetPosition);
            case BehaviorOn.Flee:
                return Flee(targetPosition);
            case BehaviorOn.Arrive:
                return Arrive(targetPosition, decelerationRate);
            case BehaviorOn.Pursuit:                                                                                                                      
                return Pursuit(evader);
            case BehaviorOn.Evade:
                return Evade(evader);
            case BehaviorOn.Wander:
                return Wander();
            default:
                return Vector3.zero;
        }
    }

    public Vector3 Seek(Vector3 targetPos)
    {
        Vector3 desiredVelocity = (targetPos - transform.position).normalized;
        desiredVelocity.Scale(Vector3.one * entity.GetMaxSpeed());

        return desiredVelocity - entity.GetVelocity();
    }

    public Vector3 Flee(Vector3 targetPos)
    {
        if (Vector3.Distance(transform.position, targetPos) > fleeingRadius)
        {
            return Vector3.zero;
        }

        Vector3 desiredVelocity = (transform.position - targetPos).normalized;
        desiredVelocity.Scale(Vector3.one * entity.GetMaxSpeed());

        return desiredVelocity - entity.GetVelocity();
    }

    public Vector3 Arrive(Vector3 targetPos, Deceleration dec)
    {
        Vector3 toTarget = targetPos - transform.position;

        // calculate the distance to the target position
        float dist = toTarget.magnitude;

        if (dist > 0)
        {
            // calculate the speed required to reach the target given the desired deceleration
            float speed = dist / ((float)dec * decelerationMultiplier);

            // cap the speed
            speed = Mathf.Min(speed, entity.GetMaxSpeed());

            Vector3 desiredVelocity = toTarget * speed / dist;

            // TO BE DELETED
            FindObstructingObstacles();

            return desiredVelocity - entity.GetVelocity();
        }

        return Vector3.zero;
    }

    public Vector3 Pursuit(EvaderController evader)
    {
        if (evader == null) { return Vector3.zero; }

        // if the evader is ahead and facing the agent we can just seek for evader's current position
        Vector3 toEvader = evader.transform.position - transform.position;
        Vector3 evaderHeadingVector = evader.GetVelocity().normalized;
        float relativeHeading = Vector3.Dot(entity.GetHeadingVector(), evaderHeadingVector);
        
        if (Vector3.Dot(toEvader, entity.GetHeadingVector()) > 0 &&
                relativeHeading < -0.95)
        {
            return Seek(evader.transform.position);
        }

        // if we are here then the evader is not considered to be ahead so we predict where the evader will be
        // look-ahead time is proportional to the distance between the evader and the pursuer, and is 
        // inversely proportional to the sum of the agents' velocities
        float lookAheadTime = toEvader.magnitude / (entity.GetMaxSpeed() + evader.GetVelocity().magnitude);

        // now seek to the predicted position of the evader
        return Seek(evader.transform.position + evader.GetVelocity() * lookAheadTime);
    }       

    public Vector3 Evade(EvaderController pursuer)
    {
        if (evader == null) { return Vector3.zero; }

        // no need to check for the facing direction here
        Vector3 toPursuer = pursuer.transform.position - transform.position;

        // look-ahead time calculation 
        float lookAheadTime = toPursuer.magnitude / (entity.GetMaxSpeed() + pursuer.GetVelocity().magnitude);

        // now flee away from predicted future position of pursuer
        return Flee(pursuer.transform.position + pursuer.GetVelocity() * lookAheadTime);
    }

    public Vector3 Wander()
    {
        var pointOnCircumference = transform.position + radius;

        // add a small random vector to the point's position
        pointOnCircumference += new Vector3(Random.Range(-wanderJitterRange, wanderJitterRange) * wanderJitterFactor, 
            0, Random.Range(-wanderJitterRange, wanderJitterRange) * wanderJitterFactor);

        // calculate a unit vector with direction to the point on circumference in vehicle's local coordinate system
        var unitVector = (pointOnCircumference - transform.position).normalized;
        // increase the length of the vector to the same as the radius
        unitVector *= wanderRadius;

        // project this point into a position wanderDistance in front of the target
        var newPointPosition = unitVector + transform.position + entity.GetHeadingVector().normalized * wanderDistance;

        return Seek(newPointPosition) * wanderSpeedFactor;
    }

    public GameObject[] FindObstructingObstacles()
    {
        //Use the OverlapBox to detect if there are any other colliders within this box area.
        //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
        Vector3 headingVector = entity.GetHeadingVector().normalized;
        var pos = transform.position + headingVector * obstacleDetectionBoxLength/2*entity.GetVelocity().magnitude/
            entity.GetMaxSpeed() - headingVector*capsuleCollider.radius;
        var size = new Vector3(capsuleCollider.radius, capsuleCollider.height / 2, obstacleDetectionBoxLength / 2 * entity.GetVelocity().magnitude/entity.GetMaxSpeed());
        Collider[] hitColliders = Physics.OverlapBox(pos, size, transform.rotation, obstacleLayerMask);
        int i = 0;
        GameObject closestIntersectingObstacle = null;
        float distanceToClosestIntersectingObstacle = float.MaxValue;
        Vector3 localPosOfClosestObstacle = Vector3.zero;
        //Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            //Output all of the collider names
            var obs = hitColliders[i];
            Debug.Log("Hit : " + obs.name);
            // Calculate this obstacle's position in local space
            var localPos = transform.InverseTransformPoint(obs.transform.position);
            Debug.Log("Local Pos: " + localPos + " for obstacle: " + obs.name);
            // We only want to deal with obstacles in front of the player
            if (localPos.z >= 0)
            {
                Debug.Log("Obstacle in front!");
                // If the distance from the z axis to the obstacle's position is less than its radius + half 
                // of the width of the detection box then there is an intersection
                float maxOfXAndZScale = Mathf.Max(hitColliders[i].transform.localScale.x / 2, hitColliders[i].transform.localScale.z / 2);
                float expandedRadius = maxOfXAndZScale + capsuleCollider.radius;
                if (Mathf.Abs(localPos.x) < expandedRadius)
                {
                    Debug.Log("Intersect!");
                    // Intersection!
                    // Do a line/circle intersection test.
                    // The centre of the circle is represented by (cZ, cX). The intersection points are given 
                    // by the formula z = cZ +/-sqrt(r^{2}-cX^{2}) for x=0.
                    // We only need to look at the smallest positive value of z because that will be the closest point of intersection
                    float cZ = localPos.z;
                    float cX = localPos.x;
                    float sqrtPart = Mathf.Sqrt(expandedRadius * expandedRadius - cX * cX);
                    float intersectionPoint = cZ - sqrtPart;

                    // If this intersection point is behind vehicle then take the other intersection point in the front
                    // If not, this intersection point is already the closest in front of the vehicle
                    if (intersectionPoint <= 0)
                    {
                        intersectionPoint = cZ + sqrtPart;
                    }
                    // Test to see if this is the closest so far. If it is, keep a record of the obstacle and its local coordinates
                    if (intersectionPoint < distanceToClosestIntersectingObstacle)
                    {
                        distanceToClosestIntersectingObstacle = intersectionPoint;
                        closestIntersectingObstacle = hitColliders[i].gameObject;
                        localPosOfClosestObstacle = localPos;
                    }
                }
            }
            i++;
        }
        if (closestIntersectingObstacle != null)
        {
            Debug.Log(closestIntersectingObstacle.name + " at " + localPosOfClosestObstacle.ToString() + " is the closest so far");
            // Found a closest intersection obstacle!
            // Calculate a force away from it
            Vector3 steeringForce = Vector3.zero;

            // The closer the obstacle the strong the steering force
            float multiplier = 1.0f + (obstacleDetectionBoxLength - localPosOfClosestObstacle.z) / obstacleDetectionBoxLength;
            // Calculate the lateral force
            //Debug.Log(closestIntersectingObstacle.transform.localScale.x / 2 + capsuleCollider.radius >= Mathf.Abs(localPosOfClosestObstacle.x));
            if (localPosOfClosestObstacle.x >= 0)
            {
                // Force is downwards!
                Debug.Log("Downwards force!");
                steeringForce.x = -(closestIntersectingObstacle.transform.localScale.x / 2 
                    + capsuleCollider.radius
                    - Mathf.Abs(localPosOfClosestObstacle.x)) * multiplier;
            }
            else
            {
                Debug.Log("Upwards force!");
                steeringForce.x = (closestIntersectingObstacle.transform.localScale.x / 2
                    + capsuleCollider.radius
                    - Mathf.Abs(localPosOfClosestObstacle.x)) * multiplier;
            }

            // Apply a braking force proportional to the obstacle's distance from the vehicle
            float brakingWeight = 0.2f;

        }
        


        return null;
    }

    void OnDrawGizmos()
    {
        if (entity == null) { return; }
        Vector3 headingVector = entity.GetHeadingVector().normalized;
        Gizmos.color = Color.red;
        Matrix4x4 prevMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        var pos = transform.position + headingVector * obstacleDetectionBoxLength / 2 *
            entity.GetVelocity().magnitude / entity.GetMaxSpeed() - headingVector *
            capsuleCollider.radius;
        pos = transform.InverseTransformPoint(pos);
        var size = new Vector3(capsuleCollider.radius * 2, capsuleCollider.height, obstacleDetectionBoxLength
            * entity.GetVelocity().magnitude/entity.GetMaxSpeed());
        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        Gizmos.DrawWireCube(pos, size);
        Gizmos.matrix = prevMatrix;
    }
}
