using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float shellTimer = 2f;
    public GameObject[] shells = new GameObject[4]; // For the gun belts mechanic (the gun cycles through four types of bullets that are set in the inspector. Example: incendiary/explosive/tracer/explosive. So the gun will fire in that pattern
    int index = 0; // Index for the particular bullet that will be fired from shells[].
    public float rateOfFireRPM = 0; // This is the reference RPM (rounds per minute) the gun fires. I just take a reference value from the internet, as it depends on the specific gun
    public float muzzleVelocity; // This is how fast the bullet goes when it's fired.
    [SerializeField] float accuracyError = 0.0001f; // Used for gun spread
    [SerializeField] AudioSource shot; // piu piu sound
    public float rateOfFire; // This is used later, when firing.
    [SerializeField] float rofTimer; // Similar as the one above(?

    [SerializeField]float overheatTimer;
    [SerializeField] bool overheated;
    [SerializeField] float overheatResetTimer;
    [SerializeField] float overheatedRoF;

    [SerializeField] KillCounter killCounter;


    // Start is called before the first frame update
    void Start()
    {
        rateOfFire = 1 / (rateOfFireRPM / 60); // This turns the reference RPM into a small float (how much time happens between bullets being fired)
        overheatedRoF = rateOfFire * 5f;
    }

    private void Update()
    {
        if(rofTimer <= overheatedRoF)
        {
            rofTimer += Time.deltaTime; // just your typical timer
        }

        if (overheated)
        {
            overheatResetTimer += Time.deltaTime;
            if (overheatResetTimer >= 10f)
            {   
                overheatTimer = 0;
                overheatResetTimer = 0f;
                overheated = false;
            }
        }

        if (!overheated && overheatTimer > 0)
        {
            overheatTimer -= Time.deltaTime;
        }
    }

    // Update is called once per frame
    public void Fire()
    {
        if(overheatTimer <= 12f)
        {
            overheatTimer += Time.deltaTime * 2;
        }
        else
        {
            overheated = true;
        }

        if (!overheated)
        {
            if (rofTimer >= rateOfFire)
            {
                Vector3 error = new Vector3(Random.Range(-accuracyError, accuracyError), Random.Range(-accuracyError, accuracyError), 0);
                GameObject shell = Instantiate(shells[index], transform.position, transform.rotation); // Instantiates the bullet...
                shell.GetComponent<Rigidbody>().AddForce((transform.forward + error) * muzzleVelocity, ForceMode.VelocityChange); ; // Gets the rigidbody & shoots it + error)
                Shell sh = shell.GetComponent<Shell>();
                sh.SetKillEnemyDelegate(EnemyKilled); // Sets the delegate
                sh.SetHitBonusDelegate(HitBonus);
                if(shot != null){
                    shot.PlayOneShot(shot.clip); // Just a small audio
                }
                Destroy(shell, shellTimer); // And destroy it after a pair of seconds if it didn't hit anything

                // Controlling the index so the bullets fire in the correct pattern
                if (index < shells.Length - 1)
                {
                    index++;
                }
                else
                {
                    index = 0;
                }

                rofTimer = 0f;
            }
        }
        else if (overheated)
        {
            if (rofTimer >= overheatedRoF)
            {
                Vector3 error = new Vector3(Random.Range(-0.005f, 0.005f), Random.Range(-0.005f, 0.005f), 0);
                GameObject shell = Instantiate(shells[index], transform.position, transform.rotation); // Instantiates the bullet...
                shell.GetComponent<Rigidbody>().AddForce((transform.forward + error) * muzzleVelocity, ForceMode.VelocityChange); ; // Gets the rigidbody & shoots it + error)
                Shell sh = shell.GetComponent<Shell>();
                sh.SetKillEnemyDelegate(EnemyKilled); // Sets the delegate
                sh.SetHitBonusDelegate(HitBonus);
                if (shot != null)
                {
                    shot.PlayOneShot(shot.clip); // Just a small audio
                }
                Destroy(shell, shellTimer); // And destroy it after a pair of seconds if it didn't hit anything

                // Controlling the index so the bullets fire in the correct pattern
                if (index < shells.Length - 1)
                {
                    index++;
                }
                else
                {
                    index = 0;
                }
                rofTimer = 0f;
            }
        }
        
    }

    public void SetKillCounter(KillCounter kc)
    {
        killCounter = kc;
    }

    public void EnemyKilled(bool countsAsKill, int points)
    {
        if(killCounter != null)
        {
            killCounter.GiveKill(countsAsKill, points);
        }
    }

    public void HitBonus(int points)
    {
        if (killCounter != null)
        {
            killCounter.GivePoints(points);
        }
    }
}
