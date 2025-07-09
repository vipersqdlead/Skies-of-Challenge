using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SAGMissileControl : BaseSpWeaponControl
{

    public int MaxMissileAmmo;
    public int MissileAmmo;
    float MissileReload = 20;
    public bool canReload;

    public GameObject missilePrefab;
    public GameObject[] MissilePos;
    public GameObject GuidePoint;
	[SerializeField] float guideRange = 8000f;
    [SerializeField] KillCounter killCounter;


    void Start()
    {
        weaponName = missilePrefab.name;
		GuidePoint.transform.localPosition = new Vector3(0, 0, guideRange);
    }

    void Update()
    {
        if (isPlayer)
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.LeftShift))
            {
                FireMissile();
            }
        }

        MissilePositions();

        if(canReload)
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


    void FireMissile()
    {
        if (MissileAmmo > 0)
        {
            int currentIndex = MissileAmmo - 1;
            {
                Quaternion launchRotation = MissilePos[currentIndex].transform.rotation;
                Vector3 eulerRotation = launchRotation.eulerAngles;
                eulerRotation.z = 0; // Reset roll axis
                GameObject missile = Instantiate(missilePrefab, MissilePos[currentIndex].transform.position, Quaternion.Euler(eulerRotation));
                Rigidbody missilerb = missile.GetComponent<Rigidbody>();
                missilerb.AddForce(gameObject.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
                missile.GetComponent<SAG_Missile>().LaunchingPlane = gameObject;
                missile.GetComponent<RocketScript>().SetKillEnemyDelegate(EnemyKilled);
                MissileAmmo--;
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
        if (countsAsKill)
        {
            killCounter.Kills++;
        }
        killCounter.Points += points;
        print("Got a kill!");
    }

    public override void DisableWeapon()
    {
		return;
        throw new System.NotImplementedException();
    }
}
