using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedObject : MonoBehaviour
{
    [SerializeField] float timerTillDestroyed;
    [SerializeField] GameObject Explosion, waterSplash, wreckage;
    [SerializeField] ParticleSystem[] particles;
    void Start()
    {
        timerTillDestroyed = timerTillDestroyed + (Random.Range(0, timerTillDestroyed / 2));
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
	    Destroy(particle, 60f);
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

}
