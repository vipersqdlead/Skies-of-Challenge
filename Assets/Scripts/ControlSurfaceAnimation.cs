using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSurfaceAnimation : MonoBehaviour
{
    [SerializeField] AircraftHub hub;

    public Transform[] ailerons, elevators, rudders, slats, flaps, airbrakes;

    public float maxDeflectionAngle = 30f;  // Maximum deflection angle in degrees
    public AnimationCurve deflectionCurve = AnimationCurve.Linear(0, 1, 1, 0.1f); // Curve to adjust deflection based on speed
    public float flapExtensionAngle = 35f; public float flapExtensionSpeed = 20f;
    public float airbrakeExtensionAngle = 60f; public float airbrakeExtensionSpeed = 30f;
    public float flapExtensionValue, brakeExtensionValue;
    public float flapAngle, brakeAngle;

    public float slatExtensionDistance = 0.1f;  // How far the slats move forward
    public float slatSpeed = 2f;  // Speed of slat movement


    // Parameters to get from the aircraft
    public float speed;
    public float angleOfAttack;

    // Control inputs
    public float pitchInput;  // From -1 to 1
    public float rollInput;   // From -1 to 1
    public float yawInput;    // From -1 to 1

    public enum ControlType { Standard, Elevon, VTail }

    //public enum ControlType { Aileron, Elevator, Rudder, Rolleron, Elevon, VTail }
    public ControlType controlType;

    float deflectionMultiplier;

    private void Awake()
    {
        hub = GetComponent<AircraftHub>();
    }

    private void Update()
    {
        speed = hub.fm.machSpeed;
        pitchInput = hub.fm.controlInput.x;
        yawInput = hub.fm.controlInput.y;
        rollInput = hub.fm.controlInput.z;
        //pitchInput = hub.playerInputs.currentInputVector.x;
        //yawInput = hub.playerInputs.currentInputVector.y;
        //rollInput = hub.playerInputs.currentInputVector.z;

        deflectionMultiplier = deflectionCurve.Evaluate(speed);

        // Calculate deflection based on control type
        ailerons[0].localRotation = calculateDeflections(rollInput);
        ailerons[1].localRotation = calculateDeflections(-rollInput);
        switch (controlType)
        {
            case ControlType.Standard:
                foreach (Transform elevator in elevators)
                {
                    elevator.localRotation = calculateDeflections(pitchInput);
                }
                foreach (Transform rudder in rudders)
                {
                    rudder.localRotation = calculateDeflections(-yawInput);
                }
                break;
            case ControlType.Elevon:
                float inputL = (pitchInput + rollInput) / 2f;
                //elevators[0].localRotation = calculateDeflections(inputL);
                float inputR = (pitchInput - rollInput) / 2f;
                //elevators[1].localRotation = calculateDeflections(inputR);

                for (int i = 0; i < elevators.Length; i++)
                {
                    if(i % 2 == 0)
                    {
                        elevators[i].localRotation = calculateDeflections(inputL);
                    }
                    else
                    {
                        elevators[i].localRotation = calculateDeflections(inputR);
                    }
                }

                foreach (Transform rudder in rudders)
                {
                    rudder.localRotation = calculateDeflections(-yawInput);
                }
                break;
            case ControlType.VTail:
                float inputVL = (pitchInput + yawInput) / 2f;
                elevators[0].localRotation = calculateDeflections(inputVL);
                float inputVR = (pitchInput - yawInput) / 2f;
                elevators[1].localRotation = calculateDeflections(inputVR);
                break;
        }
        SlatsAnim();
        FlapsAndAirbrakeAnim();
    }

    Quaternion calculateDeflections(float input)
    {
        float targetDeflection = 0f;
        targetDeflection = input * maxDeflectionAngle * deflectionMultiplier;
        return Quaternion.Euler(targetDeflection, 0, 0f);
    }


    void SlatsAnim()
    {
        // Handle slat animation
        foreach(Transform slat in slats)
        {
            Vector3 targetSlatPosition = Vector3.zero;

            // Extend slats if pulling back hard on the stick
            if ((hub.fm.angleOfAttack > 5f) || hub.fm.flaps)  // Adjust threshold as needed
            {
                targetSlatPosition += Vector3.down * slatExtensionDistance;
            }

            // Smoothly move slats
            slat.localPosition = Vector3.Lerp(slat.localPosition, targetSlatPosition, Time.deltaTime * slatSpeed);
        }

    }

    void FlapsAndAirbrakeAnim()
    {
        bool isFlapActive = hub.fm.flaps;

        float targetValue = isFlapActive ? 1f : 0f;
        flapExtensionValue = Mathf.Lerp(flapExtensionValue, targetValue, Time.deltaTime * flapExtensionSpeed);

        bool isAirbrakeActive = hub.fm.airbrake;

        float targetBrakeValue = isAirbrakeActive ? 1f : 0f;
        brakeExtensionValue = Mathf.Lerp(brakeExtensionValue, targetBrakeValue, Time.deltaTime * airbrakeExtensionSpeed);

        foreach (Transform flap in flaps)
        {
            flapAngle = flapExtensionValue * flapExtensionAngle;
            flap.localRotation = Quaternion.Euler(flapAngle, 0f, 0f);
        }

        foreach(Transform brake in airbrakes)
        {
            brakeAngle = brakeExtensionValue * airbrakeExtensionAngle;
            brake.localRotation = Quaternion.Euler(brakeAngle, 0f, 0f);
        }
    }
}

