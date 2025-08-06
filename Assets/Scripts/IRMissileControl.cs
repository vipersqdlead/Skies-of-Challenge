using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IRMissileControl : BaseSpWeaponControl
{
    public int MaxMissileAmmo;
    public int MissileAmmo;
    float MissileReload = 20;
    public bool canReload;
	[SerializeField] AircraftHub hub;

    public GameObject Missile;
    public GameObject[] MissilePos;

    public float AcquisitionMaxTimer;
    public float AcquisitionTimer;
    public float missileOuterFoV, missileInnerFoV, missileLockRange, missileMinRange;
    public bool isCagedSeeker, slaveToRadar, allAspectSeeker;
    public GameObject Target;
    [SerializeField] float distanceToTarget;
    [SerializeField] float _angleToTarget;
	
	[HideInInspector] public Vector3 seekerDirection;

    public bool Acquiring = false;
    public bool Locked = false;

    public AudioSource AcqGrowl, LockGrowl;

    [SerializeField] KillCounter killCounter;

    GameObject missilego;

    private void Start()
    {
		hub = GetComponent<AircraftHub>();
		
		if(hub == null && slaveToRadar)
		{	slaveToRadar = false; }
		
        weaponName = Missile.name;
        missileOuterFoV = Missile.GetComponent<IR_Missile>().missileOuterFoV;
		missileInnerFoV = Missile.GetComponent<IR_Missile>().missileInnerFoV;
		missileLockRange = Missile.GetComponent<IR_Missile>().searchRange;
		missileMinRange = Missile.GetComponent<IR_Missile>().minRange;
        isCagedSeeker = Missile.GetComponent<IR_Missile>().isCagedSeeker;
        slaveToRadar = Missile.GetComponent<IR_Missile>().slaveToRadar;
		allAspectSeeker = Missile.GetComponent<IR_Missile>().allAspectSeeker;
    }
	
	
	FlightModel radarTarget;
    void Update()
    {
        if (isPlayer)
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (Target == null)
                {
                    Acquiring = true;
                }

                if (Target != null)
                {
                    FireMissile();
                    TurnSeekerOff();
                    return;
                }
            }
        }

        if(MissileAmmo > 0)
        {
            if (Acquiring)
            {
                SearchForTarget();
            }
            else if (Locked)
            {
                if(Target == null)
                {
                    ResetLock();
                }
                MaintainLock();
            }
            else
            {

            }
        }
        else if(MissileAmmo == 0)
        {
            Acquiring = false;
            Locked = false;
            Target = null;
        }


        if(isPlayer && LockGrowl != null)
        {
             if (Acquiring)
             {
                    LockGrowl.enabled = true;
                    LockGrowl.pitch = 0.75f;
             }
             else if (Locked)
             {
                    LockGrowl.enabled = true;
                    LockGrowl.pitch = 1.1f;
             }
             else
             {
                    LockGrowl.enabled = false;
             }
        }

        MissilePositions();

        if (canReload)
        {
            if (MissileAmmo == 0)
            {
                MissileReload -= Time.deltaTime;
                {
                    if (MissileReload <= 0)
                    {
                        MissileAmmo = MaxMissileAmmo;
                        MissileReload = 20f;
                    }
                }
            }
        }
    }

    void SearchForTarget()
    {
        AcquisitionTimer -= Time.deltaTime;
		
		if(slaveToRadar)
		{
			if(hub.fm.target != radarTarget)
			{
				ResetLock();
				radarTarget = hub.fm.target;
			}
			
			if(hub.fm.target != null)
			{
				Vector3 dirToTgt = (hub.fm.target.transform.position - transform.position).normalized;
				float angleToTarget = Vector3.Angle(transform.forward, dirToTgt);
				if(angleToTarget < missileOuterFoV) { seekerDirection = dirToTgt; radarTarget = hub.fm.target; }
				else { seekerDirection = transform.forward; }
			}
			else { seekerDirection = transform.forward; }
		}
		
		else { seekerDirection = transform.forward; }

        if(AcquisitionTimer > 0)
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, 300f, seekerDirection, missileLockRange);
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject == gameObject)
                    continue;

                if (hit.collider.CompareTag("Fighter") || hit.collider.CompareTag("Bomber") || hit.collider.gameObject.CompareTag("Flare"))
                {
                    float angleToTarget = Vector3.Angle(seekerDirection, (hit.transform.position - transform.position).normalized);
                    float distToTempTarget = Vector3.Distance(transform.position, hit.transform.position);
                    if (angleToTarget < missileInnerFoV)
                    {
                        float _dotProduct = Vector3.Dot(transform.forward, hit.transform.forward);

						float range = Utilities.GetIRLockRange(_dotProduct, missileLockRange, allAspectSeeker);
								
						if(distToTempTarget < range)
						{
							LockTarget(hit.collider.gameObject, angleToTarget);
						}
						else 
						{ 
							continue; 
						}
                    }
                }
            }
        }


        if (AcquisitionTimer <= 0 || MissileAmmo == 0)
        {
            TurnSeekerOff();
        }
    }

    void LockTarget(GameObject target, float angleToTarget)
    {
        Target = target;
        Locked = true;
        Acquiring = false;
        distanceToTarget = Vector3.Distance(transform.position, Target.transform.position);
        _angleToTarget = angleToTarget;
    }

    void MaintainLock()
    {
        if (Target == null)
        {
            ResetLock();
            return;
        }

		distanceToTarget = Vector3.Distance(transform.position, Target.transform.position);
        float angleToTarget = Vector3.Angle(transform.forward, Target.transform.position - transform.position);
		float _dotProduct = Vector3.Dot(transform.forward, Target.transform.forward);
		float maxLockableRange = Utilities.GetIRLockRange(_dotProduct, missileLockRange, allAspectSeeker);
						
        if (angleToTarget < missileOuterFoV && distanceToTarget < maxLockableRange)
        {
            _angleToTarget = angleToTarget;
        }
        else
        {
            ResetLock();
        }
    }

    void ResetLock()
    {
        Target = null;
        Locked = false;
        Acquiring = true;
    }

    void TurnSeekerOff()
    {
        Target = null;
        Locked = false;
        Acquiring = false;
        AcquisitionTimer = AcquisitionMaxTimer;
        return;
    }



    public void FireMissile()
    {

        if(MissileAmmo > 0)
        {
            int currentIndex = MissileAmmo - 1;
            {
                Quaternion launchRotation = MissilePos[currentIndex].transform.rotation;
                Vector3 eulerRotation = launchRotation.eulerAngles;
                eulerRotation.z = 0; // Reset roll axis
                missilego = Instantiate(Missile, MissilePos[currentIndex].transform.position, Quaternion.Euler(eulerRotation));
                //missilego = Instantiate(Missile, MissilePos[currentIndex].transform.position, MissilePos[currentIndex].transform.rotation);
                Rigidbody missilerb = missilego.GetComponent<Rigidbody>();
                IR_Missile missileScript = missilego.GetComponent<IR_Missile>();
                missilerb.AddForce(gameObject.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
                missilerb.rotation = Quaternion.LookRotation(gameObject.GetComponent<Rigidbody>().velocity);
                missileScript.target = Target;
				missileScript.launcherPlane = gameObject;
                missilego.GetComponent<RocketScript>().SetKillEnemyDelegate(EnemyKilled);
                MissileAmmo--;
                return;
            }
        }
    }

    void MissilePositions()
    {
        for (int i = 0; i < MissilePos.Length; i++)
        {
            if(i < MissileAmmo)
            {
                MissilePos[i].SetActive(true);
            }
            else
            {
                MissilePos[i].SetActive(false);
            }
        }
    }

    public void EnemyKilled(bool countsAsKill, int points)
    {
        if (killCounter != null)
        {
            killCounter.GiveKill(countsAsKill, points);
        }
    }

    public override void DisableWeapon()
    {
        TurnSeekerOff();
        LockGrowl.enabled = false;
    }
}
