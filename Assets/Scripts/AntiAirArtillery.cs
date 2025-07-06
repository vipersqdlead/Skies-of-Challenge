using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiAirArtillery : MonoBehaviour
{
    public int side;
    [SerializeField] FlightModel target;
    public Gun[] guns;
    public bool trigger;
    public bool pathOfFireObstructed = false;
    [SerializeField] float firingRange;
    [SerializeField] float cannonBurstLength;
    [SerializeField] float cannonBurstCooldown;
    KillCounter kc;

    [SerializeField] bool hasTimedFuze;
    [SerializeField] float estimatedTimeToBurst;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            target = Utilities.GetNearestTarget(gameObject, side, 50000f);
            return;
        }

        {
            dist = Vector3.Distance(transform.position, target.transform.position);

            RotateTurret();
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, dist - 15f))
            {
                if (hitInfo.collider.gameObject == gameObject || hitInfo.collider.gameObject == target.gameObject)
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

            if (trigger)
            {
                foreach (Gun gun in guns)
                {
                    gun.Fire();
                    if (hasTimedFuze)
                    {
                        gun.shellTimer = estimatedTimeToBurst * Random.Range(0.95f, 1.05f);
                    }
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

            if (hasTimedFuze)
            {
                estimatedTimeToBurst = Utilities.FirstOrderInterceptTime(guns[0].muzzleVelocity, (target.transform.position - transform.position), target.rb.velocity - Vector3.zero);
            }

            if (cannonBurstTimer == 0)
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
        Vector3 targetDirection = Utilities.FirstOrderIntercept(
            transform.position,
            Vector3.zero,
            guns[0].muzzleVelocity,
            target.transform.position,
            target.rb.velocity
        );
            // Rotate toward the target
            transform.LookAt(targetDirection);
            //Quaternion targetRotation = transform.rotation;
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            CalculateCannon();
    }
}
