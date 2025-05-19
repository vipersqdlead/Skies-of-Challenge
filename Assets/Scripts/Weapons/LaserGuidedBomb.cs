using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LaserGuidedBomb : MonoBehaviour
{
    public float explosionRadius; public float explosionPower;
    Rigidbody bombRb;
    public GameObject explosion;

    public Vector3 target;
    float maxTurn = 60;

    public delegate void KillEnemy(bool countsAsKill, int points);
    KillEnemy delKillEnemy;

    // Start is called before the first frame update
    void Start()
    {
        bombRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bombRb.velocity = transform.forward * bombRb.velocity.magnitude;
        Guidance(target);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player") && !collision.collider.CompareTag("Weapon/Bomb"))
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Instantiate(explosion, transform.position, transform.rotation);
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

    void Guidance(Vector3 target)
    {
        var rotation = Quaternion.LookRotation(target - transform.position);

        float angle = Quaternion.Angle(transform.rotation, rotation);
        float timetocomplete = angle / maxTurn;
        float donePercentage = Mathf.Min(1f, Time.deltaTime / timetocomplete);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, donePercentage);
    }
    public void SetKillEnemyDelegate(KillEnemy killEnemyDel)
    {
        delKillEnemy = killEnemyDel;
    }
}
