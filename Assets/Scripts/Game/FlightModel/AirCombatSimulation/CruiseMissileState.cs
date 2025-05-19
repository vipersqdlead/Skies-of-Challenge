using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CruiseMissileState : StateBase
{
    public Transform checkpoint;
    public GameObject target;
    public Transform backupTarget;
    Vector3 GuidePoint;
    public float maxTurn;
    public GameObject explosion;
    public float explosionRadius, explosionPower;
    public AudioSource engineSnd;

    public override void OnStateStart(StateUser user)
    {
        base.OnStateStart(user);
        GuidePoint = checkpoint.transform.position;
    }

    public override void OnStateEnd()
    {

    }

    public override void OnStateStay()
    {
        Guidance();
    }

    bool reachedCheckpoint = false;
    void Guidance()
    {
        if(target == null)
        {
            target = GameObject.FindWithTag("Enemy");
        }
        if(Vector3.Distance(checkpoint.transform.position, transform.position) < 2500f && reachedCheckpoint == false)
        {
            Vector3 newGuide;
            if(target != null)
            {
                newGuide = target.transform.position + new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), Random.Range(-5, 5));
            }
            else
            {
                newGuide = backupTarget.transform.position + new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), Random.Range(-5, 5));
            }
            GuidePoint = newGuide;
            reachedCheckpoint = true;
            engineSnd.Stop();
        }

        if(GuidePoint != null)
        {
            Direction(GuidePoint, maxTurn);
        }
    }

    public void Direction(Vector3 Dir, float MaxTurn)
    {
        Quaternion rotation = Quaternion.LookRotation(Dir - transform.position);
        float angle = Quaternion.Angle(transform.rotation, rotation);
        float timetocomplete = angle / MaxTurn;
        float donePercentage = Mathf.Min(1f, Time.deltaTime / timetocomplete);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, donePercentage);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Bullet"))
        {
            Instantiate(explosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObj in colliders)
        {
            if(!nearbyObj.CompareTag("Bomber"))
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
