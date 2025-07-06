using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareDispenser : MonoBehaviour
{
    public bool isPlayer;
    public bool trigger;
	Rigidbody rb;
    [SerializeField] private GameObject flarePrefab;
    [SerializeField] private Transform[] flareSpawnPoint; // Position where flares are deployed
    [SerializeField] float flareReload = 20;
    public int flareCount = 5; // Number of flares available
    [SerializeField] int maxFlareCount;
    public float rateOfFireRPM = 0; // This is the reference RPM (rounds per minute) for continuous flare dispensing.
    public float rateOfFire; // Time in seconds between shots
    [SerializeField] float rofTimer; // Timer to be used when firing
	
	void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

    private void Start()
    {
        rateOfFire = 1 / (rateOfFireRPM / 60); // This turns the reference RPM into a small float (how much time happens between bullets being fired)
        maxFlareCount = flareCount;
    }

    private void Update()
    {
        if (isPlayer)
        {
            if (Input.GetAxis("DeployFlare") != 0)
            {
                trigger = true;
            }
            else
            {
                trigger = false;
            }
        }

        if (trigger)
        {
            DeployFlare();
        }


        if (rofTimer <= rateOfFire)
        {
            rofTimer += Time.deltaTime;
        }

        if (flareCount == 0)
        {
			if(flareReload != 0)
			{
				flareReload -= Time.deltaTime;
				{
					if (flareReload <= 0)
					{
						flareCount = maxFlareCount;
						flareReload = 20f;
					}
				}

			}
        }
    }

    public void DeployFlare()
    {
        if (flareCount > 0)
        {
            if (rofTimer >= rateOfFire)
            {
                for (int i = 0; i < flareSpawnPoint.Length; i++)
                {
                    Fire(i);
                }
                rofTimer = 0;
            }
        }
    }

    public void Fire(int i)
    {
        {
			Vector3 error = new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), 0f);
            GameObject flare = Instantiate(flarePrefab, flareSpawnPoint[i].position, transform.rotation);
            flare.GetComponent<Rigidbody>().AddForce(rb.velocity + ((flareSpawnPoint[i].forward + error) * 115f), ForceMode.VelocityChange);
            flareCount--;
            Destroy(flare, 10f);
        }
    }
}
