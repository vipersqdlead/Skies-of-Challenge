using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{

    public int ShellVelocity, DmgToAir, DmgToGrnd;
	public Rigidbody rb;
    public bool isTracer;
	public bool isArmorPiercing;
    public bool isIncendiary;
    public bool selfDestruct;
    public ShellType shellType;
    public float reloadTimer;
    public GameObject ExplosionSmaller, waterSplashPrefab;
    public AudioSource hitSound;
    public TrailRenderer trail;
	public Collider[] collider;

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
		
		collider = GetComponents<Collider>();
		rb = GetComponent<Rigidbody>();
    }

    private void InstantiateExplosions()
    {
        Instantiate(ExplosionSmaller, transform.position, Quaternion.identity);
    }
	
	private void InstantiateExplosions(Vector3 hitPosition)
    {
        Instantiate(ExplosionSmaller, transform.position, Quaternion.identity);
    }
	
	private void InstantiateSplash()
	{
		Instantiate(waterSplashPrefab, transform.position, Quaternion.identity);
	}


    public void SetKillEnemyDelegate(KillEnemy killEnemyDel)
    {
        delKillEnemy = killEnemyDel;
    }

    public void SetHitBonusDelegate(HitBonus hitBonusDel)
    {
        delHitBonus = hitBonusDel;
    }
	
	void GetAPDamage(Rigidbody targetRb)
	{
		float hitSpeed = rb.velocity.magnitude;
		if(targetRb != null)
		{
			float targetSpeed = targetRb.velocity.magnitude;
			hitSpeed -= targetSpeed;
		}
	}

    private void OnCollisionEnter(Collision collision)
    {
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
			if(collision.collider.CompareTag("Water"))
			{
				InstantiateSplash();
			}
			else
			{
				InstantiateExplosions(collision.GetContact(0).point);
			}
            //Destroy(gameObject);
			CheckToDestroy();
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
				if(other.CompareTag("Water"))
				{
					InstantiateSplash();
				}
				else
				{
					InstantiateExplosions();
				}
                if (GetComponent<RocketEngine>() != null)
                {
                    GetComponent<RocketEngine>().KeepParticlesAlive();
                }
                //Destroy(gameObject);
				CheckToDestroy();
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
				CheckToDestroy();
			}
        }
    }

    private void OnDestroy()
    {
        if (selfDestruct)
        {
            //InstantiateExplosions();
        }
    }
	
	[SerializeField] Transform parent;
	
	public void Enable(Vector3 position, Quaternion rotation, Vector3 velocity, float timeToDisable, Transform _parent)
	{
		parent = _parent;
		transform.parent = null;
		transform.position = position;
		transform.rotation = transform.rotation;
		rb.AddForce(velocity, ForceMode.VelocityChange);
        if (isTracer)
		{
			trail.Clear();
		}
		if(selfDestruct)
		{
			InstantiateExplosions();
			return;
		}
	}

	public void Disable(Transform _parent)
	{
		parent = _parent;
		transform.parent = _parent;
		//transform.SetParent(_parent);
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		gameObject.SetActive(false);
        if (isTracer)
		{
			trail.Clear();
		}
	}
	
	public void Disable()
	{
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		gameObject.SetActive(false);
        if (isTracer)
		{
			trail.Clear();
		}
	}
	
	void CheckToDestroy()
	{
				if(parent != null)
				{
					Disable(parent);
				}
				else
				{
					Disable();
				}
	}
}
