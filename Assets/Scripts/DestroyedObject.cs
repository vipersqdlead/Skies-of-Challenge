using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedObject : MonoBehaviour
{
    [SerializeField] float timerTillDestroyed;
	[SerializeField] float chanceToParachute = 50f;
	[SerializeField] int chutesToSpawn = 1;
    [SerializeField] GameObject Explosion, waterSplash, wreckage, parachute;
    [SerializeField] ParticleSystem[] particles;
	
	
    void Start()
    {
        timerTillDestroyed = timerTillDestroyed + (Random.Range(0, timerTillDestroyed / 2));
		StartCoroutine("SpawnParachute");
    }

    // Update is called once per frame
    void Update()
    {
        timerTillDestroyed -= Time.deltaTime;
        if(timerTillDestroyed < 0)
        {
            FullyDestroy();
        }
    }

    void FullyDestroy()
    {
        Instantiate(Explosion, transform.position, transform.rotation);
        foreach(var particle in particles)
        {
            particle.transform.parent = null;
            transform.localScale = new Vector3(1, 1, 1);
            var main = particle.main;
            main.loop = false;
			Destroy(particle.gameObject, 60f);
        }
        Destroy(gameObject);
    }

    public void InstantiateSplash()
    {
        if (waterSplash != null)
        {
            GameObject splash = Instantiate(waterSplash, transform.position, Quaternion.identity);
	    Destroy(splash, 30f);
        }
    }

    public void InstantiateWreckage()
    {
        if(wreckage != null)
        {
            GameObject wreck = Instantiate(wreckage, transform.position, Quaternion.identity);
	    Destroy(wreck, 180f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            InstantiateSplash();
            FullyDestroy();
        }
        else if (other.gameObject.CompareTag("Ground"))
        {
            InstantiateWreckage();
            FullyDestroy();
        }
    }
	
	IEnumerator SpawnParachute()
    {
		yield return new WaitForSeconds(2f);
		
		for(int i = 0; i < chutesToSpawn; i++)
		{
			int randomChance = Random.Range(0, 100);
			if(randomChance <= chanceToParachute)
			{
				GameObject chute = Instantiate(parachute, transform.position, parachute.transform.rotation);
				Rigidbody chuteRb = chute.GetComponent<Rigidbody>();
				chuteRb.AddForce(gameObject.GetComponent<Collider>().attachedRigidbody.velocity + (transform.up * 100f), ForceMode.VelocityChange);
			}
			yield return new WaitForSeconds(3f);
		}
		
		yield return null;
	}

}
