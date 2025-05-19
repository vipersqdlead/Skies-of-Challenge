using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFighterModel : BasicAIModel
{
    public float SpawnSpeed = 0;

    public SquadPosType sqdPosition;

    [SerializeField] bool isExpert = false;

    public enum SquadPosType
    {
        Leader,
        Wingman
    }

    //For detecting the ground for AvoidGround state
    RaycastHit hit;
    float dist;
    Vector3 dir;

    public List<GameObject> Waypoints = new List<GameObject>(); // List of waypoints for the plane to go through when searching for targets.
    [SerializeField]int waypointIndex = 0;

    public float MaxTurn; // How much does the plane turn at current speed?
    public float Agility; // Multiplier for agility.
    [SerializeField] float distanceToPullOff;

    public List<GameObject> Targets = new List<GameObject>(); // List of possible Targets the AI could aquire.
    public GameObject Target; // Current selected Target
    public int TgtIndex = 0;
    Rigidbody AIRigidbody; // Literally the plane's rb

    public float Power, Drag; // Estimated values for the AI's engine power and Drag. They should be similar to those of the same Player-controlledd plane (That means, an AI Bf 109 should have Power and Drag values that kinda match the MaxEnginePower and Drag of a playable Bf 109)

    public Gun[] Guns;

    Rigidbody targetRb;

    // Start is called before the first frame update
    void Start()
    {
        AIRigidbody = GetComponent<Rigidbody>();
        AIRigidbody.AddForce(transform.forward * SpawnSpeed, ForceMode.VelocityChange);

        //Targets[0] = GameObject.FindGameObjectWithTag("Player");

        Target = Targets[TgtIndex];
        targetRb = Target.GetComponent<Rigidbody>();

        //Waypoints.Add(GameObject.FindGameObjectWithTag("Waypoint"));
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        AIRigidbody.drag = Drag / 1000;

        MaxTurn = (AIRigidbody.velocity.magnitude / Agility);

        AIRigidbody.AddForce(transform.forward * Power, ForceMode.Force);
        AIRigidbody.velocity = transform.forward * AIRigidbody.velocity.magnitude;

        Flying();

    }

    void Flying()
    {

        /*
        if (Target == null)
        {
            TgtIndex++;

            if (TgtIndex < Targets.Count)
            {
                Target = Targets[TgtIndex];
            }
            if (TgtIndex >= Targets.Count)
            {
                TgtIndex = 0;
            }

            else
            {
                Target = null;
            }
        }*/

        switch (sqdPosition)
        {
            case SquadPosType.Leader:
                {
                    if (Waypoints[0] == null)
                    {
                        int index = 0;
                        GameObject[] FindWaypoints = (GameObject.FindGameObjectsWithTag("Waypoint"));
                        foreach (var Wp in FindWaypoints)
                        {
                            Waypoints.Insert(index, Wp);
                            index++;
                        }
                    }
                    if (Target == null)
                    {
                        foreach (var waypoint in Waypoints)
                        {
                            Vector3 CurrentWaypoint = (waypoint.transform.position);
                            Direction(CurrentWaypoint, MaxTurn);
                            if (Vector3.Distance(waypoint.transform.position, transform.position) < 100)
                            {
                                waypointIndex++;
                            }
                        }
                    }
                    else if (Vector3.Distance(Target.transform.position, transform.position) > 3000) // No enemy near or no enemy available
                    {
                        foreach (var waypoint in Waypoints)
                        {
                            Vector3 CurrentWaypoint = (waypoint.transform.position);
                            Direction(CurrentWaypoint, MaxTurn);
                            if (Vector3.Distance(waypoint.transform.position, transform.position) < 100)
                            {
                                waypointIndex++;
                            }
                        }
                        if (waypointIndex > Waypoints.Count - 1)
                            waypointIndex = 0;

                    }
                    else if ((Physics.Raycast(transform.position, Vector3.down, distanceToPullOff)) || ((Vector3.Distance(Target.transform.position, transform.position) <= distanceToPullOff) && MaxTurn > 15f)) // Evading collision
                    {
                        Vector3 PullUp = new Vector3(transform.position.x, 6000, transform.position.z);
                        Direction(PullUp, MaxTurn);
                        print("Avoid Collsion, going up");
                    }
                    else if (((Vector3.Distance(Target.transform.position, transform.position) <= distanceToPullOff) && MaxTurn < 15f) || MaxTurn < 15f) // Evading collision
                    {
                        Vector3 PullUp = new Vector3(transform.position.x, 500, transform.position.z);
                        Direction(PullUp, MaxTurn * 2);
                        print("Avoid Collsion, going down");
                    }
                    else if (Vector3.Distance(Target.transform.position, transform.position) <= 3000 && AIRigidbody.velocity.magnitude >= 50 || Target != null) // Enemy is near
                    {
                        if (isExpert)
                        {
                            float DistanceToTarget = Vector3.Distance(Target.transform.position, transform.position);
                            Direction(FirstOrderIntercept(transform.position, AIRigidbody.velocity, (700f * Time.deltaTime), Target.transform.position, targetRb.velocity), MaxTurn); print("Dogfighting");
                            OpenFire(DistanceToTarget);
                        }
                        else if (!isExpert)
                        {
                            float DistanceToTarget = Vector3.Distance(Target.transform.position, transform.position);
                            Direction(Target.transform.position, MaxTurn); print("Dogfighting");
                            OpenFire(DistanceToTarget);
                        }
                    }

                    break;
                }

            case SquadPosType.Wingman:
                {
                    
                    if (Waypoints[0] != null)
                    {
                        // !!! FOR WINGMEN, CHOSE THE LEADER'S TRANSFORM.POSITION AS WAYPOINT !!!
                        foreach (var waypoint in Waypoints)
                        {
                            Vector3 CurrentWaypoint = (waypoint.transform.position + new Vector3(-10, 3, -15));
                            Direction(CurrentWaypoint, MaxTurn);
                            if (Vector3.Distance(waypoint.transform.position, transform.position) < 100)
                            {
                                waypointIndex++;
                            }
                        }
                        if (waypointIndex > Waypoints.Count - 1)
                            waypointIndex = 0;
                    }
                    else if (Vector3.Distance(Target.transform.position, transform.position) <= 800 && AIRigidbody.velocity.magnitude >= 50) // Enemy is near
                    {
                        if (isExpert)
                        {
                            float DistanceToTarget = Vector3.Distance(Target.transform.position, transform.position);
                            Direction(FirstOrderIntercept(transform.position, AIRigidbody.velocity, (700f * Time.deltaTime), Target.transform.position, targetRb.velocity), MaxTurn); print("Dogfighting");
                            OpenFire(DistanceToTarget);
                        }
                        else if(!isExpert)
                        {
                            float DistanceToTarget = Vector3.Distance(Target.transform.position, transform.position);
                            Direction(Target.transform.position, MaxTurn); print("Dogfighting");
                            OpenFire(DistanceToTarget);
                        } 

                    }

                    if (Waypoints[0] == null)
                    {
                        sqdPosition = SquadPosType.Leader;
                        Waypoints[0] = Target;
                    }
                    break;
                }
        }
    }

    [SerializeField] float maxShootingAngle;
    [SerializeField] float firingrange;

    [SerializeField] float bursttime;
    [SerializeField] float timer;
    [SerializeField] float cooldown;
    void OpenFire(float DistanceToTarget) 
    {
        float angleToPlayer = Vector3.Angle(transform.forward, Target.transform.position - transform.position);
        if (!isExpert)
        {
            Vector3 heading = Target.transform.position - transform.position;
            if (DistanceToTarget <= firingrange && angleToPlayer < maxShootingAngle)
            {
                timer += Time.deltaTime;
                if(timer < bursttime)
                {
                    for (int i = 0; i < Guns.Length; i++)
                    {
                        Guns[i].Fire();
                    }
                }
                else if(timer > bursttime)
                {
                    timer = -cooldown;
                }
            }
        }
        
        else if (isExpert)
        {
            Vector3 heading = FirstOrderIntercept(transform.position, AIRigidbody.velocity, (1000f), Target.transform.position, targetRb.velocity);

            if (DistanceToTarget <= firingrange && angleToPlayer < maxShootingAngle)
            {
                    timer += Time.deltaTime;
                    if (timer < bursttime && timer >= 0)
                    {
                        for (int i = 0; i < Guns.Length; i++)
                        {
                            Guns[i].Fire();
                        }
                    }
            }
            if (timer > bursttime)
            {
                timer = -cooldown;
            }
            if (timer <= 0)
            {
                timer += Time.deltaTime;
            }
        }


    }

/*
    // === variables you need ===
    //how fast our shots move
    float shotSpd;
    //objects
    GameObject shooter;
    GameObject tgt;
 
// === derived variables ===
//positions
    void Awake()
    {
        Vector3 shooterPosition = shooter.transform.position;
        Vector3 targetPosition = tgt.transform.position;
        //velocities
        Vector3 shooterVelocity = shooter.GetComponent<Rigidbody>().velocity;
        Vector3 targetVelocity = tgt.GetComponent<Rigidbody>().velocity;

        //calculate intercept

        Vector3 interceptPoint = FirstOrderIntercept(shooterPosition, shooterVelocity, shotSpd, targetPosition, targetVelocity);

        //now use whatever method to launch the projectile at the intercept point
    }
*/

    //first-order intercept using absolute target position
    Vector3 FirstOrderIntercept(Vector3 shooterPosition, Vector3 shooterVelocity, float shotSpeed, Vector3 targetPosition, Vector3 targetVelocity)
    {
        Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
   
        float t = FirstOrderInterceptTime(shotSpeed,
                                        targetPosition - shooterPosition,
                                        targetRelativeVelocity);
 
        return targetPosition + t* (targetRelativeVelocity);
    }

    //first-order intercept using relative target position
    float FirstOrderInterceptTime(float shotSpeed,Vector3 targetRelativePosition, Vector3 targetRelativeVelocity)
    {
        float velocitySquared = targetRelativeVelocity.sqrMagnitude;
            if (velocitySquared < 0.001)
                return 0.0f;

        float a = velocitySquared - shotSpeed * shotSpeed;

    //handle similar velocities
        if (Mathf.Abs(a) < 0.001)
        {
            float t = -targetRelativePosition.sqrMagnitude / (2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition));
            return Mathf.Max(t, 0.0f); //don't shoot back in time
        }

        float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        float c = targetRelativePosition.sqrMagnitude;
        float determinant = b * b - 4f * a * c;

        if (determinant > 0.0)
        { //determinant > 0; two intercept paths (most common)
            float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a);
            float t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
            if (t1 > 0.0)
            {
                if (t2 > 0.0)
                {
                    return Mathf.Min(t1, t2); //both are positive
                }
                else
                {
                    return t1; //only t1 is positive
                }
            }
            else
            {
                return Mathf.Max(t2, 0.0f); //don't shoot back in time
            }
        }
        else if (determinant < 0.0)//determinant < 0; no intercept path
        {
                return 0.0f;
        } 
        
        else //determinant = 0; one intercept path, pretty much never happens
        return Mathf.Max(-b / (2f * a), 0.0f); //don't shoot back in time
}
 
}
