using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class RadarMissileControl : BaseSpWeaponControl
{
    public int MaxMissileAmmo;
    public int MissileAmmo;
    public float MissileReload = 20f;
    public bool canReload;

    public float angleToPlayer;

    public GameObject MissilePrefab;
    public GameObject[] MissilePos;

    [SerializeField] Radar_Missile missile;

    public float AcquisitionMaxTimer;
    public float AcquisitionTimer;

    public GameObject possibleTarget, Target;

    public bool Acquiring = false;
    public bool Locked = false;
    public bool Guiding = false;

    public AudioSource AcqGrowl, LockGrowl;

    [SerializeField] KillCounter killCounter;
    void Start()
    {
            weaponName = MissilePrefab.name;
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
                else if (missile != null)
                {
                    print("Missile already in the air!");
                }
            }
        }
		
		if(missile == null)
        {
            Guiding = false;
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

    void Aqcuisition()
    {
        AcquisitionTimer -= Time.deltaTime;
        if (MissileAmmo > 0 && missile == null)
        {
            if (AcquisitionTimer > 0)
            {
				float thickness = 150f; //<-- Desired thickness here
				RaycastHit[] hits = Physics.SphereCastAll(transform.position, thickness, transform.forward, 19000f);
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

					if(possibleTarget != null)
					{
						angleToPlayer = Vector3.Angle(transform.forward, possibleTarget.transform.position - transform.position);
						if (angleToPlayer < 25f)
						{
							Target = possibleTarget; Locked = true;
						}
						else if (angleToPlayer > 60f)
						{
							Target = null; Locked = false;
						}
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
            angleToPlayer = Vector3.Angle(transform.forward, Target.transform.position - transform.position);
        }
        if (missile != null && angleToPlayer <= 60f)
        {
            missile.target = Target.GetComponent<AircraftHub>();
        }
		else
		{
			if(missile != null)
			{
				missile.target = null;
			}
			TurnSeekerOff();
		}

    }

    void TurnSeekerOff()
    {
        Target = null;
        Locked = false;
        Acquiring = false;
        AcquisitionTimer = AcquisitionMaxTimer;
        return;
    }

    void FireMissile()
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
}
