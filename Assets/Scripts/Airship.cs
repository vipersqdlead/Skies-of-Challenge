using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airship : MonoBehaviour
{
    public float speed;
    [SerializeField] int mainEnginePower, secondaryEnginesPower;
    [SerializeField] GameObject mainEngine, engine1, engine2, leftWing, rightWing, bridge, radar1, radar2, mainEngineInlet;
    [SerializeField] ParticleSystem menginePart, eng1Part, eng2Part, lWPart, rWPart, BrdPart, rad1Part, rad2Prt, mEngInPart;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject SAMs_Parent, bigExplosionPrefab;

    public bool destroyed;

    // Update is called once per frame
    void FixedUpdate()
    {
        Engines();
        CheckCriticalSystems();
    }

    void Engines()
    {
        if (mainEngine != null)
        {
            rb.AddForceAtPosition(transform.forward * mainEnginePower * Time.deltaTime * 60f, mainEngine.transform.position, ForceMode.Force);
        }

        if (engine1 != null)
        {
            rb.AddForceAtPosition(transform.forward * secondaryEnginesPower * Time.deltaTime * 60f, engine1.transform.position, ForceMode.Force);
        }

        if (engine2 != null)
        {
            rb.AddForceAtPosition(transform.forward * secondaryEnginesPower * Time.deltaTime * 60f, engine2.transform.position, ForceMode.Force);
        }

        speed = rb.velocity.magnitude * 3.6f;
    }

    void CheckCriticalSystems()
    {

        menginePart.gameObject.SetActive(!mainEngine);
        eng1Part.gameObject.SetActive(!engine1);
        eng2Part.gameObject.SetActive(!engine2);
        lWPart.gameObject.SetActive(!leftWing);
        rWPart.gameObject.SetActive(!rightWing);
        BrdPart.gameObject.SetActive(!bridge);
        mEngInPart.gameObject.SetActive(!mainEngineInlet);

        if(!mainEngine && !engine1 && !engine2 && !leftWing && !rightWing && !bridge && !mainEngineInlet)
        {
            if (!destroyed)
            {
                Instantiate(bigExplosionPrefab, transform.position, transform.rotation);
                destroyed = true;
            }
            rb.useGravity = true;
        }

        if(radar1 == null && radar2 == null)
        {
            SAMs_Parent.SetActive(false);
        }
    }
}
