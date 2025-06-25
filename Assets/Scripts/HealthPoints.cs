using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class HealthPoints : MonoBehaviour
{
    [Header("General Settings")]
    public bool isPlayer;
    [SerializeField] bool isGroundTgt = false;
    [SerializeField] public bool countsAsKill = true;
    [SerializeField] public int pointsWorth;
    public float HP, hpPercent, Defense, CritRate;
    float maxHP, originalDef;
    public int extraLives;
    bool lastHit = false;

    [Header("Effects")]
    [SerializeField] GameObject Explosion;
    [SerializeField] GameObject destroyedObject;
    [SerializeField] ParticleSystem smoke;
    public bool onFire = false;
    public float fireTimer = 5f;
    float initialFireTimer;
    [SerializeField] float fireDamageTimer;
    [SerializeField] ParticleSystem fire;
    [SerializeField] AudioSource hitSound, fireSound;

    public bool invulnerable;
    [SerializeField] float invulnerableTimer;

    private void Start()
    {
        maxHP = HP;
        originalDef = Defense;
        hpPercent = maxHP * 100 / HP;
        initialFireTimer = fireTimer;
    }

    public bool TryKill(float dmg)
    {
        if (!invulnerable)
        {
            HP -= dmg;
            hpPercent = HP * 100 / maxHP;
            if (hitSound != null)
            {
                hitSound.PlayOneShot(hitSound.clip);
            }

            if (HP <= 0)
            {
                if (lastHit == false)
                {
                    if (extraLives <= 0)
                    {
                        lastHit = true;
                        Kill();
                        return true;
                    }
                    else
                    {
                        ContinueUsed();
                    }
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    private void Update()
    {
        if(smoke != null)
        {
            var emission = smoke.emission;
            if (hpPercent < 25f || onFire)
            {
                emission.enabled = true;
            }
            else
            {
                emission.enabled = false;
            }
        }


        if (invulnerable)
        {
            invulnerableTimer -= Time.deltaTime;
            if(invulnerableTimer <= 0f)
            {
                invulnerable = false;
                Defense = originalDef;
            }
        }

        if (onFire)
        {
            var emission = fire.emission;
            if(invulnerable || HP < 15f)
            {
                emission.enabled = false;
                onFire = false;
                return;
            }

            fireTimer -= Time.deltaTime;
            if(fireTimer > 0)
            {
                emission.enabled = true;
                fireDamageTimer += Time.deltaTime;
                if(fireDamageTimer > 1f)
                {
                    TryKill(2.5f);
                    fireDamageTimer = 0f;
                }
            }
            else
            {
                emission.enabled = false;
                fireTimer = initialFireTimer;
                onFire = false;
            }
        }
    }

    float dpsTimer;
    public void DealExternalDamagePerSecond()
    {
        dpsTimer += Time.deltaTime;
        if(dpsTimer > 1f)
        {
            TryKill(5f);
            dpsTimer = 0f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isGroundTgt)
        {
            if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Water") || collision.collider.CompareTag("Enemy"))
            {
                if(destroyedObject.GetComponent<DestroyedObject>() != null)
                {
                    if (collision.collider.CompareTag("Water"))
                    {
                        destroyedObject.GetComponent<DestroyedObject>().InstantiateSplash();
						KillNoExplosion();
						return;
                    }
                    if (collision.collider.CompareTag("Ground"))
                    {
                        destroyedObject.GetComponent<DestroyedObject>().InstantiateWreckage();
                    }
                }
                Kill();
            }
        }

        if (isGroundTgt)
        {
            return;
        }

        if (collision.collider.CompareTag("Fighter") || collision.collider.CompareTag("Bomber"))
        {
            Kill();
        }
    }

    void Kill()
    {
        InstantiateExplosions();
        InstantiateDestroyedObject();
        Destroy(gameObject);
        return;
    }
	
	void KillNoExplosion()
	{
        InstantiateDestroyedObject();
        Destroy(gameObject);
        return;
	}

    private void InstantiateExplosions()
    {
		if(Explosion != null)
			Instantiate(Explosion, transform.position, Quaternion.identity);
		
		else
			print("Explosion for object " + gameObject.name + " is null!");
    }

    void InstantiateDestroyedObject()
    {
        if(destroyedObject != null)
        {
            GameObject destroyedObj = Instantiate(destroyedObject, transform.position, transform.rotation);
            if (!isGroundTgt)
            {
                Rigidbody destroyedObjRb = destroyedObj.GetComponent<Rigidbody>();
                Rigidbody ownRb = GetComponent<Rigidbody>();
                destroyedObjRb.AddForce(ownRb.velocity, ForceMode.VelocityChange);
                destroyedObjRb.AddTorque(new Vector3(0, 0, Random.Range(-50f, 50f)));
                KeepParticlesAlive();
            }
        }
    }

    void KeepParticlesAlive()
    {
        if(smoke != null)
        {
            smoke.transform.parent = null;
            transform.localScale = new Vector3(1, 1, 1);
            var mainS = smoke.main;
            mainS.loop = false;
			Destroy(smoke.gameObject, 30f);
        }

        if(fire != null)
        {
            fire.transform.parent = null;
            transform.localScale = new Vector3(1, 1, 1);
            var mainF = fire.main;
            mainF.loop = false;
			Destroy(fire.gameObject, 30f);
        }

    }

    public void HealMaxHP()
    {
        HP += maxHP;
        hpPercent = HP * 100 / maxHP;

        return;
    }

    public void HealHPAmmount(float heal)
    {
        HP += heal;
        hpPercent = HP * 100 / maxHP;

        return;
    }

    public void EnableInvulerability()
    {
        invulnerable = true;
        invulnerableTimer += 180f;
        Defense = 99999f;
        return;
    }

    void ContinueUsed()
    {
        HP = maxHP;
        extraLives--;
        hpPercent = HP * 100 / maxHP;
        invulnerable = true;
        invulnerableTimer += 10f;
        Defense = 99999f;
        InstantiateExplosions();
        return;
    }

    public void GrantExtraLife()
    {
        extraLives++;
        return;
    }

}
