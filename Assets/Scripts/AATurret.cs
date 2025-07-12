using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AATurret : MonoBehaviour
{
    public AircraftHub hub;
	public FlightModel ownTarget;
    public int side;
    public Gun[] guns;
    public bool trigger;
    public bool pathOfFireObstructed = false;
    [SerializeField] float firingRange; 
    [SerializeField] float cannonBurstLength;
    [SerializeField] float cannonBurstCooldown;
	[SerializeField] float shellRange;
    KillCounter kc;

    // Turret rotation constraints
    [SerializeField] float maxHorizontalAngle = 60f; // Maximum rotation angle from the forward direction
    [SerializeField] float maxVerticalAngle = 45f;   // Maximum rotation angle upwards and downwards
    [SerializeField] float rotationSpeed = 30f;     // Speed of rotation in degrees per second
    public Vector3 initialForward;
	public float angleToTarget;

    // Start is called before the first frame update
    void Start()
    {
        if(hub.killcounter != null)
        {
            kc = hub.killcounter;
        }
        foreach(Gun gun in guns)
        {
            gun.SetKillCounter(kc);
        }

		side = hub.fm.side;
        initialForward = transform.localRotation * Vector3.forward; // Store the initial rotation of the turret
		
		shellRange = guns[0].muzzleVelocity * guns[0].shellTimer;
    }

    // Update is called once per frame

    public Vector3 targetDir;
    void Update()
    {
		if(hub.fm.side != side) { side = hub.fm.side; }

		
		if (ownTarget == null)
		{
			LookingForTargets();
			return;
		}
			
        {
            dist = Vector3.Distance(transform.position, ownTarget.transform.position);

            Ray ray = new Ray(guns[0].transform.position, guns[0].transform.forward);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, shellRange))
            {
				if(hitInfo.collider.gameObject == hub.gameObject)
				{
					pathOfFireObstructed = true;
				}
                if(hitInfo.collider.gameObject == ownTarget.gameObject)
                {
                    pathOfFireObstructed = false;
                }
                else
                {
                    pathOfFireObstructed = true;
                }
            }
            else
            {
                pathOfFireObstructed = false;
            }
			
			RotateTurret();

            if(trigger)
            {
                foreach (Gun gun in guns)
                {
                    gun.Fire();
                }
            }


        }

    }

    float cannonBurstTimer;
    float cannonCooldownTimer;
    float dist;
    void CalculateCannon()
    {
        if (trigger)
        {
            cannonBurstTimer = Mathf.Max(0, cannonBurstTimer - Time.deltaTime);

            if (cannonBurstTimer == 0 || pathOfFireObstructed)
            {
                cannonCooldownTimer = cannonBurstCooldown;
                trigger = false;
            }
        }
        else
        {
            cannonCooldownTimer = Mathf.Max(0, cannonCooldownTimer - Time.deltaTime);

            if (cannonCooldownTimer == 0 && !pathOfFireObstructed && dist < firingRange)
            {
                trigger = true;
                cannonBurstTimer = cannonBurstLength;
            }
        }
    }

    void RotateTurret()
    {
        // Predict target position
        Vector3 interceptPoint = Utilities.FirstOrderIntercept(
            transform.position,
            hub.rb.velocity,
            guns[0].muzzleVelocity,
            ownTarget.transform.position,
            ownTarget.rb.velocity * Random.Range(0.9f, 1.1f)
        );

        // Calculate the angle between the turret's forward direction and the target
        //Vector3 localTargetDirection = transform.InverseTransformDirection(targetDirection - transform.position);
        //float yawAngle = Mathf.Atan2(localTargetDirection.x, localTargetDirection.z) * Mathf.Rad2Deg;
        //float pitchAngle = Mathf.Atan2(localTargetDirection.y, localTargetDirection.z) * Mathf.Rad2Deg;
		
		targetDir = (ownTarget.transform.position - transform.position).normalized;
		Vector3 localTargetDirection = hub.transform.InverseTransformDirection(targetDir);
		angleToTarget = Vector3.Angle(initialForward, localTargetDirection);

        //if (Mathf.Abs(yawAngle) <= maxHorizontalAngle && Mathf.Abs(pitchAngle) <= maxVerticalAngle)
		if (angleToTarget <= (maxVerticalAngle / 2))
        {
            // Rotate toward the target
            transform.LookAt(interceptPoint);
            CalculateCannon();
        }
        else
        {
			//transform.localRotation = Quaternion.Euler(initialForward);
			transform.localRotation = Quaternion.LookRotation(initialForward);
            trigger = false;
			LookingForTargets();
        }
    }
	
	float lookTimer;
    void LookingForTargets()
    {
        lookTimer += Time.deltaTime;
        if(lookTimer > 10f)
        {
            ownTarget = Utilities.GetNearestTarget(gameObject, side, firingRange * 4f);
            lookTimer = 0f;
        }
    }
}
