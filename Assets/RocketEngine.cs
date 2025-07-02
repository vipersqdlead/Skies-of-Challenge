using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using static UnityEngine.ParticleSystem;

public class RocketEngine : MonoBehaviour
{
    public float rocketTimer, rocketThrust;
	float totalRocketTimer;
	[SerializeField] float initialMass, finalMass;
    [SerializeField] Rigidbody rb;
    [SerializeField] bool rocketDelay, destroyOnceOutOfFuel;
    [SerializeField] ParticleSystem rocketTrailParticle, smoke;
    [SerializeField] AudioSource launchSnd, rocketSound;

    bool isAlreadyOutofFuel;

    public float delayTimer = 0.4f;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
		if(initialMass == 0){
		initialMass = rb.mass;}
		if(finalMass == 0){
		finalMass = initialMass;}
		totalRocketTimer = rocketTimer;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(rocketDelay)
        {
            delayTimer -= Time.deltaTime;
            if(delayTimer < 0f)
            {
                RocketBooster();
            }
        }

        else
        {
            RocketBooster();
        }
    }

    void RocketBooster()
    {
        if (rocketTimer >= 0)
        {
            launchSnd.enabled = true;
            if(rocketSound != null)
            {
                rocketSound.enabled = true;
            }
            rb.AddForce(transform.forward * rocketThrust * Time.deltaTime * 60f, ForceMode.Impulse);
            if(rocketTrailParticle != null)
            {
                rocketTrailParticle.enableEmission = true;
            }
            if(smoke != null)
            {
			var smokeEmission = smoke.emission;
                smokeEmission.enabled = true;
            }
            rocketTimer -= Time.deltaTime;
			rb.mass = Mathf.Lerp(initialMass, finalMass, ((totalRocketTimer - rocketTimer) * 100f / totalRocketTimer) / 100f);
        }
        else if (rocketTimer <= 0)
        {
            if (!isAlreadyOutofFuel)
            {
                if (rocketTrailParticle != null)
                {
                    rocketTrailParticle.enableEmission = false;
                }
                if (smoke != null)
                {
					var smokeEmission = smoke.emission;
                    smokeEmission.enabled = false;
                }
                if (rocketSound != null)
                {
                    rocketSound.enabled = false;
                }
                //KeepParticlesAlive();
                isAlreadyOutofFuel = true;
				
				if(destroyOnceOutOfFuel)
				{
					GetComponent<RocketScript>().Explosion();
				}
				
                this.enabled = false;
            }
            else
            {
                return;
            }
        }
    }

    public void KeepParticlesAlive()
    {
        if(smoke != null)
        {
            var smokeEmission = smoke.emission;
            smokeEmission.enabled = false;
            var mainS = smoke.main;
            mainS.loop = false;
            smoke.transform.parent = null;
            //transform.localScale = new Vector3(1, 1, 1);  
			Destroy(smoke.gameObject, 30f);
        }
    }

    private void OnDestroy()
    {
        KeepParticlesAlive();
    }
}
