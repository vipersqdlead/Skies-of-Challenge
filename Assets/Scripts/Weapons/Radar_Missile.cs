using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Radar_Missile : MonoBehaviour
{
    GameObject Missile;
    public GameObject shooter;

    public AircraftHub possibleTarget, target;

    [SerializeField] float timerToFuze = 1f;
    [SerializeField] float lifeTime = 60f;
    Rigidbody rb;
    public bool ProxyFuse;

    public float maxGLoad = 20f;
    public float energyBleedMultiplier, currentMaxGAvailable;
	public AnimationCurve maxGLoadMultiplierAtMach;
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
	Vector3 lastKnownTargetDirection;

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
		
		// "Wake-up" factor: ramp up control authority
        float timeSinceLaunch = Time.time - launchTime;
        float authorityFactor = Mathf.Clamp01(timeSinceLaunch / rampUpTime);
		
		currentMaxGAvailable = (maxGLoad * authorityFactor) * maxGLoadMultiplierAtMach.Evaluate(speedMach);

        speed = rb.velocity.magnitude * 3.6f;
        speedMach = speed / 1234f;
        CalculateGForce();
        float newDrag = drag * Utilities.airDensityAnimCurve.Evaluate(transform.position.y / 10000f);
		float turnDrag = newDrag * (Mathf.Abs(gForce) * energyBleedMultiplier * Utilities.airDensityAnimCurve.Evaluate(transform.position.y / 10000f));
        rb.drag = Mathf.Clamp(turnDrag, newDrag, 1);

        if (gForce > maxGAchieved)
        {
            maxGAchieved = gForce;
        }

        if (speedMach > maxSpeedAchievedMach)
        {
            maxSpeedAchievedMach = speedMach;
        }
		
		if(target != null)
		{
			lastKnownTargetDirection = (target.transform.position - gameObject.transform.position).normalized;
		}
    }

    float timerToSelfDestruct;
    void Guidance()
    {
        if (target != null)
        {
            transform.rotation = GuidancePN();
            if(IR_Guidance != null)
            {
                IR_Guidance.target = target.gameObject;
            }
        }

        else if(target == null)
        {
            if(!hasTerminalGuidance)
            {
                timerToSelfDestruct += Time.deltaTime;
                if (timerToSelfDestruct > 10f)
                {
                    GetComponent<RocketScript>().Explosion();
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
        Vector3 targetVel = target.rb.velocity;

        Vector3 relativePos = targetPos - missilePos;
        Vector3 relativeVel = targetVel - missileVel;

        Vector3 los = relativePos.normalized;
        Vector3 losRate = (los - lastLOS) / Time.fixedDeltaTime;

        lastLOS = los;

        float closingVelocity = -Vector3.Dot(relativeVel, los);

        // PN guidance: desired lateral acceleration
        Vector3 lateralAcceleration = navigationConstant * closingVelocity * losRate;

        // Limit lateral acceleration to max G
        float maxAccel = currentMaxGAvailable * 9.81f; // convert Gs to m/s 
        if (lateralAcceleration.magnitude > maxAccel)
        {
            lateralAcceleration = lateralAcceleration.normalized * maxAccel;
        }



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
		Vector3 lookDirection = Vector3.zero;
		
		if(target == null)
		{
			lookDirection = lastKnownTargetDirection;
		}
		else
		{
			lookDirection = (target.transform.position - gameObject.transform.position).normalized;
		}
		
        print("Acquiring");
        RaycastHit hit;
        float thickness = 300f; //<-- Desired thickness here
        if (Physics.SphereCast(transform.position, thickness, lookDirection, out hit))
        {
            if (hit.collider.CompareTag("Fighter") || hit.collider.CompareTag("Bomber"))
            {
                possibleTarget = hit.collider.GetComponent<AircraftHub>();
            }
        }

        if (possibleTarget != null)
        {
			closingSpeed = Utilities.GetClosingVelocity(possibleTarget, rb);
            angleToMissile = Vector3.Angle(transform.forward, possibleTarget.transform.position - transform.position);
            if (angleToMissile < 45f && closingSpeed - rb.velocity.magnitude > 30f)
            {
                target = possibleTarget; Locked = true; print("Target Locked!");
            }
            else if (angleToMissile >= 45f)
            {
                target = null; Locked = false;
            }
        }
    }

    void TargetReflection()
    {
		if(target == null) return;

        float missileToTargetAngle = Vector3.Angle(transform.forward, target.transform.position-transform.position);
		if(hasTerminalGuidance) { NotchFilter(); }
        if (missileToTargetAngle >= 45f)
        {
            target = null;
        }
    }
	
	public float closingSpeed;
	public float timeToLockBreak = 3f;
	float maxTimeToLockBreak;
	void NotchFilter()
	{
		closingSpeed = Utilities.GetClosingVelocity(target, rb);
		
		if((closingSpeed - rb.velocity.magnitude) < 30f)
		{
			timeToLockBreak -= Time.deltaTime;
		}
		else
		{
			if(timeToLockBreak < maxTimeToLockBreak)
			{
				timeToLockBreak += Time.deltaTime;
			}
		}
		
		if(timeToLockBreak <= 0f)
		{
			print("Missile " + gameObject.name + " has been notched!");
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
