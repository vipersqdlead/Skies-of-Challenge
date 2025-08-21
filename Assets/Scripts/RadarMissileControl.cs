using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class RadarMissileControl : BaseSpWeaponControl
{
	public AircraftHub hub;
	
    public int MaxMissileAmmo;
    public int MissileAmmo;
    public float MissileReload = 20f;
    public bool canReload;

    public float angleToPlayer;
	[HideInInspector] public Vector3 seekerDirection;

    public GameObject MissilePrefab;
    public GameObject[] MissilePos;

    public Radar_Missile missile;

    public float AcquisitionMaxTimer;
    public float AcquisitionTimer;
	
	public float radarRange = 19000f;
	public float radarGimbalLimit = 60f;
	public bool pulseDoppler = true;

    public GameObject possibleTarget, Target;

    public bool Acquiring = false;
    public bool Locked = false;
    public bool Guiding = false;

    public AudioSource AcqGrowl, LockGrowl;

    [SerializeField] KillCounter killCounter;
    void Awake()
    {
        weaponName = MissilePrefab.name;
		hub = GetComponent<AircraftHub>();
    }

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

                if (Target != null && missile == null)
                {
                    FireMissile();
                }
            }
        }
		
		if(missile == null)
        {
			if(Guiding)
			{
				Guiding = false;
				ResetLock();
			}
        }
		
        if (isPlayer && LockGrowl != null)
        {
            LockGrowl.enabled = (Locked && !Guiding);
        }

        if (Acquiring == true)
        {
            Aqcuisition();
        }
        if(missile != null)
        {
            Guiding = true;
            if (Guiding)
            {
                GuidingMissile();
            }
        }
		if(Target == null)
        {
            Locked = false;
        }

        MissilePositions();

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
	
	public bool chaffed;
	FlightModel radarTarget;
    void Aqcuisition()
    {
        AcquisitionTimer -= Time.deltaTime;
        if (MissileAmmo > 0 && missile == null)
        {
            if (AcquisitionTimer > 0)
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
					if(angleToTarget < radarGimbalLimit) { seekerDirection = dirToTgt; radarTarget = hub.fm.target; }
					else { seekerDirection = transform.forward; }
				}
				
				
				float thickness = 10f; //<-- Desired thickness here
				RaycastHit[] hits = Physics.SphereCastAll(transform.position, thickness, seekerDirection, radarRange);
				chaffed = false;
				foreach (var hit in hits)
				{
					if(hit.collider.CompareTag("Bullet") || hit.collider.gameObject == gameObject)
					{
						continue;
					}

					if (hit.collider.CompareTag("Fighter") || hit.collider.CompareTag("Bomber"))
					{
						possibleTarget = hit.collider.gameObject;
					}
					
					if(hit.collider.CompareTag("Chaff"))
					{
						if(pulseDoppler && notch)
						{
							//chaff = hit.collider.gameObject;
							chaffed = true;
						}
						else if(!pulseDoppler) { chaffed = true; Target = hit.collider.gameObject; Locked = true; }
					}
					
					if(hit.collider.CompareTag("Ground"))
					{
						if(possibleTarget != null)
						{
							float distToTarget = Vector3.Distance(transform.position, possibleTarget.transform.position);
							float distToGround = Vector3.Distance(transform.position, hit.point);
							bool isTargetCovered = (distToGround < distToTarget);
							
							if(isTargetCovered)
							{
								ResetLock();
								return;
							}
						}
					}
					
					
					if(possibleTarget != null && !chaffed)
					{
						angleToPlayer = Vector3.Angle(transform.forward, possibleTarget.transform.position - transform.position);
						
						if(pulseDoppler)
						{
							NotchFilter();
							//if (angleToPlayer < radarGimbalLimit && Mathf.Abs((Utilities.GetClosingVelocity(possibleTarget.GetComponent<AircraftHub>(), hub.rb) - hub.rb.velocity.magnitude)) > filterSize)
							if (angleToPlayer < radarGimbalLimit && !notch)
								
							
							{
								Target = possibleTarget; Locked = true;
							}
							else
							{
								ResetLock();
							}
						}
						
						else
						{
							if (angleToPlayer < radarGimbalLimit && possibleTarget.transform.position.y > 3500f)
							{
								Target = possibleTarget; Locked = true;
							}
							else
							{
								ResetLock();
							}
						}
					}
					if(chaffed)
					{
						ResetLock();
					}
				}
            }
        }

        if (AcquisitionTimer <= 0 || (MissileAmmo == 0 && missile == null))
        {
            TurnSeekerOff();
        }
    }

    void GuidingMissile()
    {
        if(Target != null)
        {
			if(pulseDoppler) { NotchFilter(); } else { CheckForChaff(); }
            angleToPlayer = Vector3.Angle(transform.forward, Target.transform.position - transform.position);
        }
        if (missile != null && angleToPlayer <= radarGimbalLimit)
        {
            missile.target = Target.GetComponent<AircraftHub>();
        }
		else
		{
			if(missile != null || Target == null)
			{
				missile.target = null;
			}
			TurnSeekerOff();
		}
    }
	
	void CheckForChaff()
	{
		float thickness = 10f; //<-- Desired thickness here
		RaycastHit[] hits = Physics.SphereCastAll(transform.position, thickness, seekerDirection, radarRange);
		chaffed = false;
		foreach (var hit in hits)
		{
			if(hit.collider.CompareTag("Bullet") || hit.collider.gameObject == gameObject)
			{
				continue;
			}

			if(hit.collider.CompareTag("Chaff"))
			{
				chaffed = true;
				missile.target = null;
				missile = null;
				Target = hit.collider.gameObject;
			}
		}
	}
	
	public float closingSpeed;
	public bool notch;
	public float filterSize = 20f;
	float timeToLockBreak = 1.25f;
	float maxTimeToLockBreak = 1.25f;
	void NotchFilter()
	{
		
		/*
		closingSpeed = Mathf.Abs((Utilities.GetClosingVelocity(possibleTarget.GetComponent<AircraftHub>(), hub.rb) - hub.rb.velocity.magnitude));
		
		if(closingSpeed < filterSize)
		{
			timeToLockBreak -= Time.deltaTime;
			notch = true;
		}
		else
		{
			if(timeToLockBreak < maxTimeToLockBreak)
			{
				timeToLockBreak += Time.deltaTime;
			}
			notch = false;
		}
		
		if(timeToLockBreak <= 0f)
		{
			print("Radar " + gameObject.name + " has been notched!");
			timeToLockBreak = maxTimeToLockBreak;
			ResetLock();
		}
		
		*/
		
		float _dotProduct = Mathf.Abs(Vector3.Dot(hub.rb.velocity.normalized, possibleTarget.GetComponent<AircraftHub>().rb.velocity.normalized));
		closingSpeed = _dotProduct;
		
		if(_dotProduct < filterSize / 100f)
		{
			timeToLockBreak -= Time.deltaTime;
			notch = true;
		}
		else
		{
			if(timeToLockBreak < maxTimeToLockBreak)
			{
				timeToLockBreak += Time.deltaTime;
			}
			notch = false;
		}
		
		if(timeToLockBreak <= 0f)
		{
			timeToLockBreak = maxTimeToLockBreak;
			ResetLock();
		}
	}
	
	void ResetLock()
    {
        Target = null;
        Locked = false;
		if(missile != null) missile.target = null;
		missile = null;
        Acquiring = true;
    }

    public void TurnSeekerOff()
    {
        ResetLock();
		Acquiring = false;
        AcquisitionTimer = AcquisitionMaxTimer;
        return;
    }

    public void FireMissile()
    {
        if (MissileAmmo > 0)
        {
            int currentIndex = MissileAmmo - 1;
            {
                Quaternion launchRotation = MissilePos[currentIndex].transform.rotation;
                Vector3 eulerRotation = launchRotation.eulerAngles;
                eulerRotation.z = 0; // Reset roll axis
                missile = Instantiate(MissilePrefab, MissilePos[currentIndex].transform.position, Quaternion.Euler(eulerRotation)).GetComponent<Radar_Missile>();
                Rigidbody missilerb = missile.GetComponent<Rigidbody>();
                missilerb.AddForce(gameObject.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
                missilerb.rotation = Quaternion.LookRotation(gameObject.GetComponent<Rigidbody>().velocity);
                missile.GetComponent<RocketScript>().SetKillEnemyDelegate(EnemyKilled);
                missile.GetComponent<Radar_Missile>().shooter = gameObject;
                MissileAmmo--;
                AcquisitionTimer = AcquisitionMaxTimer;
                return;
            }
        }
    }

    void MissilePositions()
    {
        for (int i = 0; i < MissilePos.Length; i++)
        {
            if (i < MissileAmmo)
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
         if(missile != null) { missile.target = null; }
         missile = null;
         LockGrowl.enabled = false;
    }
	
	public void PrepareForAIUse()
	{
		isPlayer = false;
		if(LockGrowl != null)
		{ LockGrowl.enabled = false; }
		LockGrowl = null;
	}
}
