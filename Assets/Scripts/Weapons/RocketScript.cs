using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class RocketScript : MonoBehaviour
{
    public float timerToFuze;
    public float explosionRadius; public float explosionPower;
    Rigidbody rocketRb;
    public SphereCollider proxyFuse;
    public GameObject explosion, waterSplash;

    public delegate void KillEnemy(bool countsAsKill, int points);
    KillEnemy delKillEnemy;

    // Start is called before the first frame update
    void Start()
    {
        rocketRb = GetComponent<Rigidbody>();
		if(proxyFuse == null)
		{
			proxyFuse = GetComponent<SphereCollider>();
		}
		
		if(proxyFuse == null)
		{
			this.enabled = false;
			return;
		}
		
		if(explosionRadius == 0f)
		{
			explosionRadius = proxyFuse.radius * 3f;
		}
		

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timerToFuze -= Time.deltaTime;

        if (timerToFuze <= 0)
        {
            if(proxyFuse != null)
            {
                proxyFuse.enabled = true;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player"))
        {
            Explosion();
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Fighter") || other.CompareTag("Bomber") || other.CompareTag("Enemy") || other.CompareTag("Ground"))
        {
            Explosion();
            Destroy(gameObject);
        }
        if (other.CompareTag("Water"))
        {
            Instantiate(waterSplash, transform.position, Quaternion.identity);
            Explosion();
            Destroy(gameObject);
        }
    }

    public void Explosion()
    {
		if(explosion != null)
			Instantiate(explosion, transform.position, transform.rotation);
		
		else
			print("Rocket or missile " + gameObject.name + " has no Explosion !");
		
		RocketEngine[] engines = GetComponents<RocketEngine>();
        foreach (RocketEngine engine in engines)
        {
            engine.KeepParticlesAlive();
        }
		
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius + proxyFuse.radius);

        foreach (Collider nearbyObj in colliders)
        {
			float distanceToObj = Vector3.Distance(transform.position, nearbyObj.transform.position);
			
			float damageToTarget = explosionPower;
			if (distanceToObj > proxyFuse.radius)
			{			
				float inverseDistance = Mathf.Abs((distanceToObj - explosionRadius));
				float damagePercent = (inverseDistance * 100f / explosionRadius) / 100;
				damageToTarget *= damagePercent;
			}
			
            HealthPoints objHp = nearbyObj.GetComponent<HealthPoints>();
            if (objHp != null)
            {
                float damageDealt = damageToTarget / (objHp.Defense * (Random.Range(0.8f, 1f)));
                float critDefRandom = Random.Range(0, 100);
                if (critDefRandom < objHp.CritRate)
                {
                    if (objHp.TryKill(damageDealt * 2))
                    {
						if(delKillEnemy != null)
						{
							delKillEnemy.Invoke(objHp.countsAsKill, objHp.pointsWorth);
						}
                    }
                }
                else
                {
                    if (objHp.TryKill(damageDealt))
                    {
						if(delKillEnemy != null)
						{
							delKillEnemy.Invoke(objHp.countsAsKill, objHp.pointsWorth);
						}
                    }
                }
            }
        }
		Destroy(gameObject);
    }

    public void SetKillEnemyDelegate(KillEnemy killEnemyDel)
    {
        delKillEnemy = killEnemyDel; 
    }
}
