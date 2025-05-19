using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

public class PlaneCamera : MonoBehaviour
{
    [SerializeField] new Camera camera;
    [SerializeField] Vector3 cameraOffset;
    [SerializeField] Vector2 lookAngle;
    [SerializeField] float movementScale;
    [SerializeField] float lookAlpha;
    [SerializeField] float movementAlpha;
    [SerializeField] Vector3 deathOffset;
    [SerializeField] float deathSensitivity;
    [SerializeField] PlayerInputs inputs;
    [SerializeField] AircraftHub hub;
    HealthPoints hp;

    public Transform cameraTransform;
    FlightModel plane;
    Transform planeTransform;
    Vector2 lookInput;
    bool dead;

    Vector2 look;
    Vector2 lookAverage;
    Vector3 avAverage;

    public GameObject camLockedTarget;



    public enum CameraMode { FreeLook, Tracking, Forward }
    private CameraMode currentMode = CameraMode.Forward;
    private Quaternion targetRotation;
    float camTransitionSpeed = 5f; // Adjust this for smoother transitions

    void Awake()
    {
        hub = GetComponent<AircraftHub>();
        cameraTransform = camera.GetComponent<Transform>();
        plane = GetComponent<FlightModel>();
        inputs = GetComponent<PlayerInputs>();
        hp = plane.health;
        cameraParent = camera.transform.parent.gameObject;
    }

    void LateUpdate()
    {
        lookInput.x = inputs.currentInputVector.z + inputs.currentInputVector.y;
        lookInput.y = inputs.currentInputVector.x;

        var cameraOffset = this.cameraOffset;

        if (Input.GetAxis("Zoom") != 0)
        {
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 25f, Time.deltaTime * 10f);
        }
        else
        {
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, CalculateFoV(), Time.deltaTime * 10f);
        }

        if (Input.GetAxis("SelectTarget") != 0)
        {
            SearchForTargetCameraLock();
        }

        HandleCameraMode();

        cameraParent.transform.localRotation = Quaternion.Slerp(
            cameraParent.transform.localRotation,
            targetRotation,
            Time.deltaTime * camTransitionSpeed
        );



        lookAngle = Vector2.one;
        {
            var targetLookAngle = Vector2.Scale(lookInput, lookAngle);
            lookAverage = (lookAverage * (1 - lookAlpha)) + (targetLookAngle * lookAlpha);

            var angularVelocity = plane.localAngularVelocity;
            angularVelocity.z = -angularVelocity.z;
            avAverage = (avAverage * (1 - movementAlpha)) + (angularVelocity * movementAlpha);
        }

        var rotation = Quaternion.Euler(-lookAverage.y, lookAverage.x, 0);  //get rotation from camera input
        var turningRotation = Quaternion.Euler(new Vector3(-avAverage.x, -avAverage.y, avAverage.z) * movementScale);   //get rotation from plane's AV

        cameraTransform.localPosition = rotation * turningRotation * cameraOffset;  //calculate camera position;
        cameraTransform.localRotation = rotation * turningRotation;                 //calculate camera rotation

        CamShake();
    }


    public GameObject cameraParent;

    float sensitivity = 0.10f;
    Vector3 currentInputVector;
    Vector2 smoothVelocity;
    void DampInputs()
    {
        Vector2 inputs = new Vector2(Input.GetAxis("HorizontalCameraRotation"), Input.GetAxis("VerticalCameraRotation"));
        currentInputVector = Vector2.SmoothDamp(currentInputVector, inputs, ref smoothVelocity, sensitivity);
    }

    float lastHP;
    void CamShake()
    {
        if (hp.HP != lastHP)
        {
            IEnumerator coroutine = Shake(0.3f, 0.2f);
            StartCoroutine(coroutine);
            lastHP = hp.HP;
        }
    }
    [HideInInspector] public bool camShaking;
    IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = Vector3.zero;
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            camShaking = true;
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            cameraParent.transform.localPosition = originalPosition + new Vector3(x, y, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cameraParent.transform.localPosition = originalPosition;
        camShaking = false;
        yield return null;
    }

    void SearchForTargetCameraLock()
    {

        float closestDistance = Mathf.Infinity;
        GameObject closestTarget = null;
        RaycastHit[] hits = Physics.SphereCastAll(cameraParent.transform.position, 5000f, cameraParent.transform.forward);
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject == gameObject)
            {
                continue;
            }
            if (hit.collider.CompareTag("Fighter") || hit.collider.CompareTag("Bomber"))
            {
                AircraftHub closestTgtHub = (hit.collider.gameObject.GetComponent<AircraftHub>());
                if (closestTgtHub.fm.side == hub.fm.side || closestTgtHub.fm == hub.fm.target)
                {
                    continue;
                }

                Vector3 directionToTarget = hit.transform.position - cameraParent.transform.position;
                float distanceToTarget = directionToTarget.magnitude;

                // Check angle between the camera's forward direction and the direction to the target
                float angle = Vector3.Angle(cameraParent.transform.forward, directionToTarget);

                // If the target is within the specified angle and is the closest one found so far
                if (angle <= camera.fieldOfView && distanceToTarget < closestDistance)
                {
                    closestDistance = distanceToTarget;
                    closestTarget = hit.collider.gameObject;
                }




                //float angleToTarget = Vector3.Angle(transform.forward, hit.transform.position - transform.position);
                //if (angleToTarget < 90)
                //{
                //    camLockedTarget = hit.collider.gameObject;
                //    break;
                //}
            }
            camLockedTarget = closestTarget;
            if (camLockedTarget != null)
            {
                if (camLockedTarget.GetComponent<FlightModel>() != null)
                {
                    hub.fm.target = camLockedTarget.GetComponent<FlightModel>();
                }
            }
            else
            {
                hub.fm.target = null;
            }
        }
    }

    void HandleCameraMode()
    {
        float trackingInput = Input.GetAxis("TargetTrackingCamera");

        if (trackingInput != 0)
        {
            if (camLockedTarget != null)
            {
                currentMode = CameraMode.Tracking;
            }
            else
            {
                SearchForTargetCameraLock();
            }
        }
        else if (Input.GetAxis("HorizontalCameraRotation") != 0 || Input.GetAxis("VerticalCameraRotation") != 0) // Replace with your actual input check
        {
            currentMode = CameraMode.FreeLook;
        }
        else
        {
            currentMode = CameraMode.Forward;
        }

        UpdateCameraTargetRotation();
    }

    void UpdateCameraTargetRotation()
    {
        switch (currentMode)
        {
            case CameraMode.FreeLook:
                DampInputs();
                targetRotation = Quaternion.Euler(new Vector3(currentInputVector.y * 90f, currentInputVector.x * 180f, 0));
                break;

            case CameraMode.Tracking:
                if (camLockedTarget != null)
                {
                    // Get world-space LookAt rotation
                    Quaternion worldLookRotation = Quaternion.LookRotation(camLockedTarget.transform.position - cameraParent.transform.position);

                    // Convert to local space relative to the player's aircraft
                    targetRotation = Quaternion.Inverse(hub.transform.rotation) * worldLookRotation;
                }
                break;

            case CameraMode.Forward:
                targetRotation = Quaternion.identity; // Local forward orientation
                break;
        }

    }

    float CalculateFoV()
    {
        float baseFoV = 50f;
        float maxFoV = 75f;

        float currentFoV = Mathf.Lerp(baseFoV, maxFoV, Mathf.Clamp(hub.fm.machSpeed, 0.1f, 1f));
        return currentFoV;
    }
}
