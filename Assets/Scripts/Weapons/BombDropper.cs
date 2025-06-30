using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDropper : MonoBehaviour
{
    public GameObject bombPrefab;
    public Transform target; // The target to hit
    public float timeToTarget;
    public float estToImpact;
    [SerializeField] Rigidbody rb;
    private float gravity = Mathf.Abs(Physics.gravity.y); // Unity's gravity (magnitude)
    [SerializeField] int bombAmmo;
    [SerializeField] float rateOfFire, rateOfFireRPM, rofTimer;
    private void Start()
    {
        rateOfFire = 1 / (rateOfFireRPM / 60); // This turns the reference RPM into a small float (how much time happens between bullets being fired)
        estToImpact += Random.Range(-1f, 1f);
		
		//
		
		if(target == null)
		{
			this.enabled = false;
		}
		
		
		//
    }

    void Update()
    {
        CalculateTimeToTarget();

        if(timeToTarget < estToImpact)
        {
            rofTimer += Time.deltaTime; // just your typical timer
            if (rofTimer >= rateOfFire)
            {
                if(bombAmmo > 0)
                {
                    DropBomb();
                    bombAmmo--;
                    rofTimer = 0;
                }

            }
        }
        
    }

    void CalculateTimeToTarget()
    {
        Vector3  targetPosYCorrected = new Vector3(target.position.x, transform.position.y, target.position.z);
        timeToTarget = Vector3.Distance(rb.transform.position, targetPosYCorrected) / rb.velocity.magnitude;
    }

    void DropBomb()
    {
        // Instantiate and drop the bomb
        GameObject bomb = Instantiate(bombPrefab, transform.position, transform.rotation);
        Rigidbody bombRb = bomb.GetComponent<Rigidbody>();
        bombRb.AddForce(rb.velocity, ForceMode.VelocityChange);
    }
}
