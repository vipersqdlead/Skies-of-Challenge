using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class SAM_Launcher : MonoBehaviour
{
    public GameObject target;
    [SerializeField] int SAM_Launches = 4;
	[SerializeField] int chanceOfLaunchPerSecond = 10;
    [SerializeField] int side = 0;
    [SerializeField] protected GameObject missilePref;
    [SerializeField] protected GameObject missile;
    [SerializeField] protected Transform msslSpawn;
    [SerializeField] AudioSource fireSound;
    [SerializeField] bool pathOfFireObstructed = false;
    [SerializeField] float firingRange, minimumRange;
    [SerializeField] Transform aimPoint;
    HealthPoints health;
    [SerializeField] GameObject missileAlert;

    // Start is called before the first frame update
    void Start()
    {
        if (target == null)
        {
            LookingForTargets();
            return;
        }
		
		health = GetComponent<HealthPoints>();
    }

    // Update is called once per frame
    public virtual void Update()
    {

        if (target == null)
        {
            LookingForTargets();
            return;
        }

        if (target != null)
        {
			if(aimPoint == null)
			{
				aimPoint = target.transform;
			}
			
            transform.LookAt(target.transform);
            RaycastHit hitInfo;

            if (Physics.Raycast(transform.position, transform.forward.normalized * 10f, out hitInfo))
            {
                if(hitInfo.collider.gameObject != target)
                {
                    pathOfFireObstructed = true;
                }
                else
                {
                    pathOfFireObstructed = false;
                }
            }
			else { pathOfFireObstructed = false; }
			
			float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            if ((distanceToTarget < firingRange && distanceToTarget > minimumRange) && !pathOfFireObstructed)
            {

                timer += Time.deltaTime;
                if (timer >= 1)
                {
                    random = Random.Range(0, 100);
                    timer = 0f;
                    if (random <= chanceOfLaunchPerSecond && missile == null && SAM_Launches > 0)
                    {
                        print("SAM, Open fire!");
                        OpenFire();
                        SAM_Launches--;
                        if(SAM_Launches == 0)
                        {
							if(health != null) { health.pointsWorth = 50; }
							this.enabled = false;
                        }
                        timer = 0f;
                    }
                }
            }
            if(missile != null)
            {
				/*
                if (Vector3.Distance(transform.position, target.transform.position) > Vector3.Distance(transform.position, missile.transform.position))
                {
					if(missile.GetComponent<SAG_Missile>() != null)
					{
						missile.GetComponent<SAG_Missile>().Guide = null;
					}
                    missile = null;
                } */
            }

        }
    }

    float timer; [SerializeField] int random;
    public virtual void OpenFire()
    {
            {
                missile = Instantiate(missilePref, msslSpawn.transform.position, msslSpawn.transform.rotation);
                SAG_Missile msslControl = missile.GetComponent<SAG_Missile>();
                msslControl.isPlayerTheLauncher = false;
                msslControl.LaunchingPlane = gameObject;
                msslControl.Guide = aimPoint;
            }
    }

    private void OnDisable()
    {
        if(missile != null)
        {
            missile.GetComponent<SAG_Missile>().Guide = null;
        }
    }

    float lookTimer;
    void LookingForTargets()
    {
        lookTimer += Time.deltaTime;
        if (lookTimer > 5f)
        {
            if(Utilities.GetNearestTarget(gameObject, side, 50000f) != null)
            {
                target = Utilities.GetNearestTarget(gameObject, side, 50000f).gameObject;
            }
            lookTimer = 0f;
        }
    }
}
