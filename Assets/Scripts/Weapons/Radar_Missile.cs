using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Radar_Missile : MonoBehaviour
{
    GameObject Missile;
    public GameObject shooter;

    public GameObject possibleTarget, target;

    [SerializeField] float timerToFuze = 1f;
    [SerializeField] float lifeTime = 60f;
    Rigidbody rb;
    public bool ProxyFuse;

    public float maxGLoad = 20f;
    public float energyBleedMultiplier;
    public float rampUpTime = 2f;
    [SerializeField] float launchTime, drag;

    public float angleToMissile;
    public bool hasTerminalGuidance;
    public IR_AllAspect_Missile IR_Guidance;

    public bool Acquiring = false;
    public bool Locked = false;

    public delegate void KillEnemy();
    KillEnemy delKillEnemy;

    [Header("Missile State")]
    public float speed;
    public float speedMach;
    [SerializeField] public float gForce;
    private float maxGAchieved;
    private float maxSpeedAchievedMach;

    // Start is called before the first frame update
    void Start()
    {
        Missile = gameObject;
        rb = GetComponent<Rigidbody>();
        drag = rb.drag;
        launchTime = Time.time;
	Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timerToFuze -= Time.deltaTime;
        
        if (timerToFuze <= 0)
        {
            ProxyFuse = true;
            Guidance();
        }

        TargetReflection();
        rb.velocity = transform.forward * rb.velocity.magnitude;

        speed = rb.velocity.magnitude * 3.6f;
        speedMach = speed / 1234f;
        CalculateGForce();
        float newDrag = drag * Utilities.airDensityAnimCurve.Evaluate(transform.position.y / 10000f);
        rb.drag = Mathf.Clamp(newDrag * (Mathf.Abs(gForce) * energyBleedMultiplier), newDrag, 1);

        if (gForce > maxGAchieved)
        {
            maxGAchieved = gForce;
        }

        if (speedMach > maxSpeedAchievedMach)
        {
            maxSpeedAchievedMach = speedMach;
        }
    }

    float timerToSelfDestruct;
    void Guidance()
    {
        /*if(target != null)
        {
            //var rotation = Quaternion.LookRotation(target.transform.position - transform.position);
            var rotation = Quaternion.LookRotation(Utilities.FirstOrderIntercept(transform.position, Rigidbody.velocity, Rigidbody.velocity.magnitude, target.transform.position, target.GetComponent<Rigidbody>().velocity) - transform.position);
            float angle = Quaternion.Angle(transform.rotation, rotation);
            float timetocomplete = angle / maxGLoad;
            float donePercentage = Mathf.Min(1f, Time.fixedDeltaTime / timetocomplete);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, donePercentage);
        }


        if(target != null)
        {
            Vector3 interceptPoint = Utilities.FirstOrderIntercept(
            transform.position,
            rb.velocity,
            rb.velocity.magnitude,
            target.transform.position,
            target.GetComponent<Rigidbody>().velocity);

            Vector3 desiredDirection = (interceptPoint - transform.position).normalized;

            // 1. Calculate current speed
            float speed = rb.velocity.magnitude;

            // 2. Convert G-load to m/s 
            float maxAccel = maxGLoad * 9.81f;

            // 3. Calculate max angular turn rate in radians/sec
            float maxTurnRate = maxAccel / Mathf.Max(speed, 0.1f); // avoid divide by zero

            // 4. Compute how far along we are in the "turn ramp-up"
            float timeSinceLaunch = Time.time - launchTime;
            float rampProgress = Mathf.Clamp01(timeSinceLaunch / rampUpTime);

            // Optional: smooth with a curve (ease in)
            rampProgress = Mathf.SmoothStep(0f, 1f, rampProgress);

            // 5. Apply turning
            Quaternion currentRot = transform.rotation;
            Quaternion targetRot = Quaternion.LookRotation(desiredDirection);

            // Limit angular velocity
            float maxDelta = maxTurnRate * Mathf.Rad2Deg * Time.fixedDeltaTime * rampProgress;
            transform.rotation = Quaternion.RotateTowards(currentRot, targetRot, maxDelta);
        }*/

        if (target != null)
        {
            transform.rotation = GuidancePN();
            if(IR_Guidance != null)
            {
                IR_Guidance.target = target;
            }
        }

        else if(target == null)
        {
            if(!hasTerminalGuidance)
            {
                timerToSelfDestruct += Time.deltaTime;
                if (timerToSelfDestruct > 10f)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                if(IR_Guidance == null)
                {
                    Acquisition();
                    return;
                }

                else if(IR_Guidance != null)
                {
                    IR_Guidance.enabled = true;
                    this.enabled = false;
                }
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

    void Acquisition()
    {
        print("Acquiring");
        RaycastHit hit;
        float thickness = 300f; //<-- Desired thickness here
        if (Physics.SphereCast(transform.position, thickness, transform.forward, out hit))
        {
            if (hit.collider.CompareTag("Fighter") || hit.collider.CompareTag("Bomber"))
            {
                possibleTarget = hit.collider.gameObject;
            }
        }

        if (possibleTarget != null)
        {
            angleToMissile = Vector3.Angle(transform.forward, possibleTarget.transform.position - transform.position);
            if (angleToMissile < 25f)
            {
                target = possibleTarget; Locked = true; print("Target Locked!");
            }
            else if (angleToMissile >= 90f)
            {
                target = null; Locked = false;
            }
        }
    }

    void TargetReflection()
    {
	if(target == null) return;

        float missileToTargetAngle = Vector3.Angle(transform.forward, target.transform.position-transform.position);
        if (missileToTargetAngle >= 60f)
        {
            target = null;
        }
    }

    Vector3 lastVelocity;
    Vector3 LocalGForce;
	void CalculateGForce()
{
		// Get the change in velocity over time (acceleration)
		Vector3 acceleration = (rb.velocity - lastVelocity) / Time.fixedDeltaTime;
		lastVelocity = rb.velocity;

		// Remove the component of acceleration in the direction of velocity (i.e., forward acceleration)
		Vector3 velocityDir = rb.velocity.normalized;
		Vector3 lateralAcceleration = Vector3.ProjectOnPlane(acceleration, velocityDir);

		// Compute G-force based only on lateral acceleration
		float lateralG = lateralAcceleration.magnitude / 9.81f;

		gForce = lateralG;
	}
    void OnDestroy()
    {
        print("Missile " + gameObject.name + " max G achieved was: " + maxGAchieved);
        print("Missile " + gameObject.name + " max speed achieved was: M " + maxSpeedAchievedMach);
    }
}
