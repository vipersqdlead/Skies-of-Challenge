using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using static IRMissileControl;
using static Shell;
using static UnityEngine.GraphicsBuffer;

public class IR_Missile : MonoBehaviour
{

    GameObject Missile;

    public GameObject target, launcherPlane;
    Rigidbody rb;
    public float searchRange = 2000f;
    public float missileInnerFoV, missileOuterFoV;
    public bool isCagedSeeker;
    public IRCCMType irccmType;

    public float missileTimer = 2f;
    public float fuzeActivationTime = 0.5f;
    float fuzeTimer;
    public float maxGLoad, energyBleedMultiplier;
    public float rampUpTime = 1f;
    [SerializeField] float launchTime;
    public bool doesPurePursuit;
    float drag;
    public bool ProxyFuse;
    public Collider proxyFuseCollider;
    public GameObject explosionSmall;
    public delegate void KillEnemy();
    KillEnemy delKillEnemy;

    [Header("Missile State")]
    public float speed;
    public float speedMach;
    private float maxGAchieved;
    private float maxSpeedAchievedMach;
    Vector3 directionToTarget;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        drag = rb.drag;
        fuzeTimer = missileTimer - fuzeActivationTime;
        Missile = gameObject;
    }
    void Start()
    {
	if(target != null) {        directionToTarget = (target.transform.position - transform.position).normalized; print("Target is found");}
	else { print("target not assigned!"); }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Aquire();
        missileTimer -= Time.deltaTime;

        speed = rb.velocity.magnitude * 3.6f;
        speedMach = speed / 1234f;

        if (missileTimer <= -25f)
        {
            Instantiate(explosionSmall, transform.position, Quaternion.identity);
            Destroy(Missile);
        }

        if (missileTimer <= fuzeTimer)
        {
            ProxyFuse = true;
            proxyFuseCollider.enabled = true;
            Guidance();
        }

        TargetReflection();
        rb.velocity = transform.forward * rb.velocity.magnitude;

        float newDrag = drag * Utilities.airDensityAnimCurve.Evaluate(transform.position.y / 10000f);
        CalculateGForce();
        rb.drag = Mathf.Clamp(newDrag * (Mathf.Abs(gForce) * energyBleedMultiplier), newDrag, 1);

        if(gForce > maxGAchieved)
        {
            maxGAchieved = gForce;
        }

        if(speedMach > maxSpeedAchievedMach)
        {
            maxSpeedAchievedMach = speedMach;
        }

    }


    public virtual void Aquire()
    {
        if (irccmType == IRCCMType.NoIRCCM)
        {
	    if(SeekerNoIRCCM() != null)
	    {
		if(SeekerNoIRCCM().CompareTag("Flare"))
		{
			target = SeekerNoIRCCM(); 
		}
	    }
        }
        else if (irccmType == IRCCMType.IRCCM)
        {
	    if(SeekerIRCCM() != null)
	    {
		if(SeekerIRCCM().CompareTag("Flare"))
		{
			target = SeekerIRCCM(); 
		}
	    }
        }

        return;
         /*
        if (target == null)
        {
            RaycastHit hit;
            float thickness = 300f; //<-- Desired thickness here
            if (Physics.SphereCast(transform.position, thickness, transform.forward, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Fighter") || hit.collider.gameObject.CompareTag("Bomber") || hit.collider.gameObject.CompareTag("Flare"))
                {
                    float dotProduct;
                    dotProduct = Vector3.Dot(transform.forward, hit.collider.transform.forward);
                    if (dotProduct > 0)
                    {
                        target = hit.collider.gameObject;
                    }
                    else
                    {
                        target = null; print("Missile lost lock!");
                    }
                }
            }

            /////////////////////////////////////////


            //List<RaycastHit> hits = Physics.SphereCastAll(transform.position, 300f, transform.forward, searchRange).ToList();
            //foreach (var hit in hits)
            //{
            //    if (hit.collider.gameObject == gameObject)
            //        continue;

            //    if (hit.collider.CompareTag("Flare"))
            //    {
            //        Target = hit.collider.gameObject;
            //        return;
            //    }

            //    if (hit.collider.CompareTag("Fighter") || hit.collider.CompareTag("Bomber") || hit.collider.gameObject.CompareTag("Flare"))
            //    {
            //        float angleToTarget = Vector3.Angle(transform.forward, hit.transform.position - transform.position);
            //        float distToTempTarget = Vector3.Distance(transform.position, hit.transform.position);
            //        if (angleToTarget < missileFoV / 2)
            //        {
            //            float _dotProduct = Vector3.Dot(transform.forward, hit.transform.forward);
            //            if (_dotProduct > 0f)
            //            {
            //                Target = hit.collider.gameObject;
            //            }
            //            else
            //            {
            //                if (distToTempTarget < searchRange / 2f)
            //                {
            //                    Target = hit.collider.gameObject;
            //                }
            //            }
            //        }
            //    }
            //}
        }

        else
        {
            if (Physics.Raycast(transform.position, target.transform.position, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Fighter") || hit.collider.gameObject.CompareTag("Bomber") || hit.collider.gameObject.CompareTag("Flare"))
                {
                    if (hit.collider.gameObject.CompareTag("Flare"))
                    {
                        target = hit.collider.gameObject;
                        return;
                    }
                    if (hit.collider.gameObject == target.gameObject)
                    {
                        return;
                    }
                    else
                    {
                        float dotProduct;
                        dotProduct = Vector3.Dot(transform.forward, hit.collider.transform.forward);
                        if (dotProduct > 0)
                        {
                            target = hit.collider.gameObject;
                        }
                    }
                }
        }
            }*/
    }

    GameObject SeekerNoIRCCM()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 300f, directionToTarget, searchRange);

        List<(int, float, RaycastHit)> sortedHits = new List<(int, float, RaycastHit)>();

        foreach (var hit in hits)
        {
            int typePriority = 1;

            if (hit.collider.CompareTag("Flare"))
            {
		typePriority = 0;
            }
            else if ((hit.collider.CompareTag("Fighter") && hit.collider.gameObject != launcherPlane) || hit.collider.CompareTag("Bomber"))
            {
		typePriority = 1;
            }

            else
            {
                continue;
            }

            if (hit.collider.CompareTag("Flare"))
            {
                typePriority = 0;
            }

	    Vector3 toHit = (hit.point - transform.position).normalized;
	    float angleToHit = Vector3.Angle(directionToTarget, toHit);
            if (angleToHit < missileInnerFoV)
            {
                continue;
            }

            float distance = hit.distance;

            sortedHits.Add((typePriority, distance, hit));
        }

        sortedHits.Sort((a, b) =>
        {
            int typeCompare = a.Item1.CompareTo(b.Item1);

            if (typeCompare != 0)
            {
                return typeCompare;
            }
            return a.Item2.CompareTo(b.Item2);
        }
        );
        if(sortedHits.Count != 0)
        {
            return sortedHits[0].Item3.collider.gameObject;
        }
        else
        {
            return null;
        }
    }
    GameObject SeekerIRCCM()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 2.5f, directionToTarget, searchRange);

        List<(int, float, RaycastHit)> sortedHits = new List<(int, float, RaycastHit)>();

        foreach (var hit in hits)
        {
            int typePriority = 1;

            if (hit.collider.CompareTag("Flare"))
            {
		typePriority = 0;
            }
            else if ((hit.collider.CompareTag("Fighter") && hit.collider.gameObject != launcherPlane) || hit.collider.CompareTag("Bomber"))
            {
		typePriority = 1;
            }

            else
            {
                continue;
            }

            if (hit.collider.CompareTag("Flare"))
            {
                typePriority = 0;
            }

	    Vector3 toHit = (hit.point - transform.position).normalized;
	    float angleToHit = Vector3.Angle(directionToTarget, toHit);
            if (angleToHit < missileInnerFoV)
            {
                continue;
            }

            float distance = hit.distance;

            sortedHits.Add((typePriority, distance, hit));
        }

        sortedHits.Sort((a, b) =>
        {
            int typeCompare = a.Item1.CompareTo(b.Item1);

            if (typeCompare != 0)
            {
                return typeCompare;
            }
            return a.Item2.CompareTo(b.Item2);
        }
        );
        if (sortedHits.Count != 0)
        {
            return sortedHits[0].Item3.collider.gameObject;
        }
        else
        {
            return null;
        }
    }


    float timerToSelfDestruct;
    void Guidance()
    {
        //if (target != null)
        //{
        //    //var rotation = Quaternion.LookRotation(target.transform.position - transform.position);
        //    var rotation = Quaternion.Euler(Vector3.zero);
        //    if (doesPurePursuit)
        //    {
        //        rotation = Quaternion.LookRotation(target.transform.position - transform.position);
        //    }
        //    else
        //    {
        //        rotation = Quaternion.LookRotation(Utilities.FirstOrderIntercept(transform.position, rb.velocity, rb.velocity.magnitude, target.transform.position, target.GetComponent<Rigidbody>().velocity) - transform.position);
        //    }
        //    float angle = Quaternion.Angle(transform.rotation, rotation);
        //    float timetocomplete = angle / maxGLoad;;
        //    float donePercentage = Mathf.Min(1f, Time.fixedDeltaTime / timetocomplete);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, donePercentage);
        //}

        if (target != null)
        {
            transform.rotation = GuidancePN();
        }

        else if (target == null)
        {
            timerToSelfDestruct += Time.deltaTime;
            if (timerToSelfDestruct > 6f)
            {
                Instantiate(explosionSmall, transform.position, transform.rotation);
                GetComponent<RocketEngine>().KeepParticlesAlive();
                Destroy(gameObject);
            }
        }
    }

    private Vector3 lastLOS;
    public float navigationConstant = 3f;
    Quaternion GuidancePN()
    {
        Vector3 missilePos = rb.position;
        Vector3 targetPos = target.transform.position;
        Vector3 missileVel = rb.velocity;
        Vector3 targetVel = target.GetComponent<Rigidbody>().velocity;

        Vector3 relativePos = targetPos - missilePos;
        Vector3 relativeVel = targetVel - missileVel;

        Vector3 los = relativePos.normalized;
        Vector3 losRate = (los - lastLOS) / Time.fixedDeltaTime;

        lastLOS = los;

        float closingVelocity = -Vector3.Dot(relativeVel, los);

        // PN guidance: desired lateral acceleration
        Vector3 lateralAcceleration = navigationConstant * closingVelocity * losRate;

        // Limit lateral acceleration to max G
        float maxAccel = maxGLoad * 9.81f; // convert Gs to m/s 
        if (lateralAcceleration.magnitude > maxAccel)
        {
            lateralAcceleration = lateralAcceleration.normalized * maxAccel;
        }

        // "Wake-up" factor: ramp up control authority
        float timeSinceLaunch = Time.time - launchTime;
        float authorityFactor = Mathf.Clamp01(timeSinceLaunch / rampUpTime);
        lateralAcceleration *= authorityFactor;

        // Now steer missile towards commanded acceleration
        Vector3 desiredDirection = (missileVel.normalized + lateralAcceleration.normalized * 0.5f).normalized;

        Quaternion desiredRotation = Quaternion.LookRotation(desiredDirection);
        float angle = Quaternion.Angle(transform.rotation, desiredRotation);

        // Rotate gradually based on max turn rate
        float turnRate = (maxAccel / missileVel.magnitude) * Mathf.Rad2Deg; // degrees per second
        float maxDelta = turnRate * Time.fixedDeltaTime;

        return Quaternion.RotateTowards(transform.rotation, desiredRotation, maxDelta);
    }

    void TargetReflection()
    {
        float missileToTargetAngle;
        if (target != null)
        {
            missileToTargetAngle = Vector3.Angle(transform.forward, target.transform.position - transform.position);
            if (missileToTargetAngle >= 90f)
            {
                target = null;
                return;
            }
        }
    }

    Vector3 lastVelocity;
    Vector3 LocalGForce;
    [SerializeField] public float gForce;
    void CalculateGForce()
    {
        var invRotation = Quaternion.Inverse(rb.rotation);
        var acceleration = (rb.velocity - lastVelocity) / Time.fixedDeltaTime;
        LocalGForce = invRotation * acceleration;
        lastVelocity = rb.velocity;
        gForce = LocalGForce.y / 9.81f;
    }

    void OnDestroy()
    {
        print("Missile " + gameObject.name + " max G achieved was: " + maxGAchieved);
        print("Missile " + gameObject.name + " max speed achieved was: M " + maxSpeedAchievedMach);
    }
}
