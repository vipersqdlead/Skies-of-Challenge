using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{

    public int ShellVelocity, DmgToAir, DmgToGrnd;
    public bool isTracer;
    public bool isIncendiary;
    public bool selfDestruct;
    public ShellType shellType;
    public float reloadTimer;
    public GameObject ExplosionSmaller;
    public AudioSource hitSound;
    public TrailRenderer trail;

    public delegate void KillEnemy(bool countsAsKill, int points);
    KillEnemy delKillEnemy;

    public delegate void HitBonus(int points);
    HitBonus delHitBonus;

    public enum ShellType
    {
        HEF_T,
        HEF,
        HEI,
        HEFI,
        AP,
        AP_T,
        API,
        IT,
        API_T
    }

    void Awake()
    {
        if (isTracer)
        {
            trail = GetComponent<TrailRenderer>();
            trail.enabled = enabled;
        }
    }

    private void InstantiateExplosions()
    {
        Instantiate(ExplosionSmaller, transform.position, Quaternion.identity);
    }

    public void SetKillEnemyDelegate(KillEnemy killEnemyDel)
    {
        delKillEnemy = killEnemyDel;
    }

    public void SetHitBonusDelegate(HitBonus hitBonusDel)
    {
        delHitBonus = hitBonusDel;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        if (collision.collider.gameObject.GetComponent<HealthPoints>() != null)
        {
            HealthPoints hp = collision.collider.gameObject.GetComponent<HealthPoints>();
            float damageDealt = (DmgToAir + Random.Range(-5f, 5f)) / hp.Defense;
            float critDefRandom = Random.Range(0, 100);
            if (critDefRandom < hp.CritRate)
            {
                if (isIncendiary)
                {
                    float fireRandom = Random.Range(0, 100);
                    if (fireRandom < hp.CritRate)
                    {
                        hp.onFire = true;
                    }
                }
                if(hp.TryKill(damageDealt * 2))
                {
                    if (delKillEnemy != null)
                    {
                        delKillEnemy.Invoke(hp.countsAsKill, hp.pointsWorth);
                    }
                }
                if (delHitBonus != null)
                {
                    delHitBonus.Invoke(10);
                }
            }
            else
            {
                if (hp.TryKill(damageDealt))
                {
                    if(delKillEnemy != null)
                    {
                        delKillEnemy.Invoke(hp.countsAsKill, hp.pointsWorth);
                    }
                }
            }
            if (delHitBonus != null)
            {
                delHitBonus.Invoke(1);
            }
        }
        if (!collision.collider.CompareTag("Bullet"))
        {
            InstantiateExplosions();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Physics.IgnoreCollision(other, GetComponent<Collider>());
        if (other.GetComponent<HealthPoints>() != null || other.CompareTag("Ground") || other.CompareTag("Water"))
        {
            HealthPoints hp = other.gameObject.GetComponent<HealthPoints>();
            if (hp == null)
            {
                InstantiateExplosions();
                if (GetComponent<RocketEngine>() != null)
                {
                    GetComponent<RocketEngine>().KeepParticlesAlive();
                }
                Destroy(gameObject);
                return;
            }
            float damageDealt = (DmgToAir + Random.Range(-5f, 5f)) / hp.Defense;
            float critDefRandom = Random.Range(0, 100);
            if (critDefRandom < hp.CritRate)
            {
                if (isIncendiary)
                {
                    float fireRandom = Random.Range(0, 100);
                    if (fireRandom < hp.CritRate)
                    {
                        hp.onFire = true;
                    }
                }
                if (hp.TryKill(damageDealt * 2))
                {
                    if (delKillEnemy != null)
                    {
                        delKillEnemy.Invoke(hp.countsAsKill, hp.pointsWorth);
                    }
                }
            }
            else
            {
                if (hp.TryKill(damageDealt))
                {
                    if (delKillEnemy != null)
                    {
                        delKillEnemy.Invoke(hp.countsAsKill, hp.pointsWorth);
                    }
                }
            }
        {
            InstantiateExplosions();
            RocketEngine[] engines = GetComponents<RocketEngine>();
            foreach (RocketEngine engine in engines)
            {
                engine.KeepParticlesAlive();
            }
            Destroy(gameObject);
        }
        }
    }

    private void OnDestroy()
    {
        if (selfDestruct)
        {
            InstantiateExplosions();
        }
    }
}
