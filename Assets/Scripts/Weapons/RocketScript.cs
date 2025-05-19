using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class RocketScript : MonoBehaviour
{
    public float timerToFuze;
    public float explosionRadius; public float explosionPower;
    Rigidbody rocketRb;
    public Collider proxyFuse;
    public GameObject explosion, waterSplash;

    public delegate void KillEnemy(bool countsAsKill, int points);
    KillEnemy delKillEnemy;

    // Start is called before the first frame update
    void Start()
    {
        rocketRb = GetComponent<Rigidbody>();
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
        if (timerToFuze <= -15f)
        {
            Destroy(gameObject);
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

    void Explosion()
    {
        Instantiate(explosion, transform.position, transform.rotation);
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObj in colliders)
        {
            HealthPoints objHp = nearbyObj.GetComponent<HealthPoints>();
            if (objHp != null)
            {
                float damageDealt = explosionPower / (objHp.Defense + (Random.Range(-5f, 5f)));
                float critDefRandom = Random.Range(0, 100);
                if (critDefRandom < objHp.CritRate)
                {
                    if (objHp.TryKill(damageDealt * 2))
                    {
                        delKillEnemy.Invoke(objHp.countsAsKill, objHp.pointsWorth);
                        Destroy(gameObject);
                    }
                }
                else
                {
                    if (objHp.TryKill(damageDealt))
                    {
                        delKillEnemy.Invoke(objHp.countsAsKill, objHp.pointsWorth);
                        Destroy(gameObject);
                    }
                }
            }
        }
        print("Explosion");
    }

    public void SetKillEnemyDelegate(KillEnemy killEnemyDel)
    {
        delKillEnemy = killEnemyDel;
    }
}
