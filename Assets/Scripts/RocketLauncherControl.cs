using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class RocketLauncherControl : BaseSpWeaponControl
{
    public bool rocketsEnabled;
    [SerializeField] bool semiAuto;
    public bool canReload;
    public int maxRockets;
    public int rocketAmmo;
    float rocketReload = 20f;
    float reloadTime;
    public GameObject rocket;
    public GameObject[] missilePos;
    public GameObject[] rocketLaunchersGO;
    public int rktIndex = 0;

    [SerializeField] float rateOfFire, rateOfFireRPM, rofTimer;

    [SerializeField] KillCounter killCounter;

    private void Start()
    {
        weaponName = rocket.name;
        reloadTime = rocketReload;
        rateOfFire = 1 / (rateOfFireRPM / 60); // This turns the reference RPM into a small float (how much time happens between bullets being fired)
		MissilePositions();
    }
    private void Update()
    {

        if (isPlayer)
        {
            if (!semiAuto)
            {
                if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.LeftShift) && rocketAmmo != 0)
                {
                    FireRocket();
                }
            }
            if (semiAuto)
            {
                if (Input.GetAxis("FireWeapon1") != 0 && rocketAmmo != 0)
                {
                    rofTimer += Time.deltaTime; // just your typical timer
                    if (rofTimer >= rateOfFire)
                    {
                        FireRocket();
                        rofTimer = 0;
                    }

                }
            }
        }

        if (rocketAmmo == 0)
        {
            foreach (GameObject rocketLauncher in rocketLaunchersGO)
            {
                rocketLauncher.SetActive(false);
            }
        }

        if (canReload)
        {
            if (rocketAmmo == 0)
            {
                Reload();
            }
        }

    }

    void FireRocket()
    {
        GameObject rktGo = Instantiate(rocket, missilePos[rktIndex].transform.position, missilePos[rktIndex].transform.rotation);
        Rigidbody RktRb = rktGo.GetComponent<Rigidbody>();
        RktRb.AddForce(gameObject.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
        rktGo.GetComponent<RocketScript>().SetKillEnemyDelegate(EnemyKilled);
        rocketAmmo--;

        if(rktIndex < missilePos.Length - 1)
        {
            rktIndex++;
        }
        else
        {
            rktIndex = 0;
        }
		
		MissilePositions();
    }

    void Reload()
    {
        reloadTime -= Time.deltaTime;
        if(reloadTime < 0)
        {
            rocketAmmo = maxRockets;
            reloadTime = rocketReload;
			MissilePositions();
        }
    }

    public void EnableRockets()
    {
        rocketsEnabled = true;
        rocketAmmo = maxRockets;
        foreach (GameObject rocketLauncher in rocketLaunchersGO)
        {
            rocketLauncher.SetActive(true);
			MissilePositions();
        }
    }
	
	void MissilePositions()
    {
        for (int i = 0; i < missilePos.Length; i++)
        {
            if (i < rocketAmmo)
            {
                missilePos[i].SetActive(true);
            }
            else
            {
                missilePos[i].SetActive(false);
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
        return;
    }
}
