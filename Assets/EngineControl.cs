using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineControl : MonoBehaviour
{
    [SerializeField] FlightModel aircraft;
    public float ThrottleInput;

    [Header("Engine Settings")]
    [SerializeField] float currentEnginePower;
    [SerializeField]public float engineStaticThrust;
    public float serviceCeiling = 12000f;
    [SerializeField] AnimationCurve powerByAltitudeMultiplier;
    [SerializeField] AnimationCurve thrustBySpeedMultiplier;
    [SerializeField] bool isAfterburningEngine;
    [SerializeField] bool useAirDensityMultiplier;
    public bool afterBurner = false;
    [SerializeField] public float afterburnerThrust;
    [SerializeField] public Transform[] engines;
    [SerializeField] AudioSource[] engineSound;
    [SerializeField] ParticleSystem[] engineAfterburners;
    [SerializeField] TrailRenderer[] engineVaporTrails;

    [Header("Propellers")]
    [SerializeField] public GameObject[] enginePropellers;
    [SerializeField] float minSpeed, maxSpeed;

    void Awake()
    {
        aircraft  = GetComponent<FlightModel>();
        if (enginePropellers.Length != 0)
        {
            useAirDensityMultiplier = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CalculateEnginePower();
        ApplyThrust();
    }

    public void SetThrottle(float input)
    {
        ThrottleInput = Mathf.Clamp(ThrottleInput + input, 0, 1);
        if (isAfterburningEngine)
        {
            if (ThrottleInput == 1f)
            {
                if (input > 0)
                {
                    afterBurner = true;
                }
                else
                {
                    afterBurner = false;
                }
            }
            else
            {
                afterBurner = false;
            }
        }
        else
        {
            afterBurner = false;
        }
    }
    float currentThrust;
    private void CalculateEnginePower()
    {

        currentEnginePower = (currentThrust * thrustBySpeedMultiplier.Evaluate(aircraft.IAS_Speed / 1234) * powerByAltitudeMultiplier.Evaluate(transform.position.y / 10000f)) * 60 * Time.fixedDeltaTime * ThrottleInput;
        if (useAirDensityMultiplier)
        {
            currentEnginePower = currentEnginePower * aircraft.currentDrag;
        }
        //engineSound.volume = ThrottleInput;

            if (afterBurner == true)
            {
                currentThrust = engineStaticThrust + afterburnerThrust;
            }
            else
            {
                currentThrust = engineStaticThrust;
            }
    }

    void ApplyThrust()
    {
        if(transform.position.y > serviceCeiling)
        {
            foreach (var engine in engineSound)
            {
                engine.volume = Mathf.Lerp(ThrottleInput, 0f, Time.deltaTime * 1f);
            }

            if (isAfterburningEngine)
            {
                foreach (var engine in engineAfterburners)
                {
                    engine.gameObject.SetActive(false);
                }
            }

            foreach (var prop in enginePropellers)
            {
                prop.transform.Rotate(0, 0, minSpeed * Time.fixedDeltaTime * 60f);
            }
            return;
        }

        else if(transform.position.y < serviceCeiling)
        {
            foreach (var engine in engines)
            {
                aircraft.rb.AddForce(transform.forward * currentEnginePower, ForceMode.Force);
            }
            foreach (var engine in engineSound)
            {
                engine.volume = ThrottleInput * 0.85f;
                engine.pitch = ThrottleInput * 0.85f;
                if (afterBurner == true)
                {
                    engine.volume = ThrottleInput;
                    engine.pitch = ThrottleInput;
                }

            }
            if (isAfterburningEngine)
            {
                foreach (var engine in engineAfterburners)
                {
                    engine.gameObject.SetActive(afterBurner);
                }
            }
            foreach (var vaporTrail in engineVaporTrails)
            {
                if (transform.position.y >= 6000)
                {
                    vaporTrail.emitting = true;
                }
                else
                {
                    vaporTrail.emitting = false;
                }
            }
            foreach (var prop in enginePropellers)
            {
                float speed = minSpeed + (maxSpeed * ThrottleInput);
                if (!afterBurner)
                {
                    prop.transform.Rotate(0, 0, speed * Time.fixedDeltaTime * 60f);
                }
                if (afterBurner)
                {
                    prop.transform.Rotate(0, 0, (speed * 1.5f) * Time.fixedDeltaTime * 60f);
                }
            }
        }
    }

    public void SetThrottleInput(float input)
    {
        ThrottleInput = input;
        if (isAfterburningEngine)
        {
            if (ThrottleInput == 1f)
            {
                if (input > 0)
                {
                    afterBurner = true;
                }
                else
                {
                    afterBurner = false;
                }
            }
            else
            {
                afterBurner = false;
            }
        }
        else
        {
            afterBurner = false;
        }
    }
}
