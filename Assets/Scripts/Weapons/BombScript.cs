using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    public float explosionRadius; public float explosionPower;
    Rigidbody bombRb;
    public GameObject explosion, waterSplash;

    // Start is called before the first frame update
    void Start()
    {
        bombRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.forward = bombRb.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (!collision.collider.CompareTag("Player") && !collision.collider.CompareTag("Weapon/Bomb") && !collision.collider.CompareTag("Bullet"))
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Enemy"))
        {
            Instantiate(explosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        if (collision.collider.CompareTag("Water"))
        {
            Instantiate(waterSplash, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObj in colliders)
        {
            HealthPoints objHp = nearbyObj.GetComponent<HealthPoints>();
            if (objHp != null)
            {
                if (objHp.TryKill(explosionPower))
                {
                    delKillEnemy.Invoke(objHp.countsAsKill, objHp.pointsWorth);
                }
            }
        }
        print("Explosion");
    }

    public delegate void KillEnemy(bool countsAsKill, int points);
    KillEnemy delKillEnemy;

    public void SetKillEnemyDelegate(KillEnemy killEnemyDel)
    {
        delKillEnemy = killEnemyDel;
    }
}
