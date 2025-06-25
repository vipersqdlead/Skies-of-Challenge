using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

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

    void Update()
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

        //if (Input.GetAxis("SelectTarget") != 0)
		if (Input.GetKeyDown(KeyCode.Tab))
        {
			hub.fm.target = AcquireTarget(transform);
            //SearchForTargetCameraLock();
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
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        cameraParent.transform.localPosition = originalPosition;
        camShaking = false;
        yield return null;
    }
	
	FlightModel AcquireTarget(Transform self)
	{
		List<Transform> possibleTargets = GetNearbyEnemyTargets(transform, 32000f);
		
		Transform target = GetTargetClosestToCenter(SortTargetsByScreenCenter(possibleTargets));

		if (target == null)
		{
			target = GetClosestTargetByDistance(self, possibleTargets);
		}
		
		FlightModel fm = target.GetComponent<FlightModel>();
		
		if(fm != null)
		{
			return fm;
		}
		else
		{
			return null;
		}
	}
	
	List<Transform> GetNearbyEnemyTargets(Transform playerTransform, float searchRadius)
	{
		List<Transform> nearbyTargets = new List<Transform>();

		// Get all colliders in the area
		Collider[] hits = Physics.OverlapSphere(playerTransform.position, searchRadius);

		foreach (Collider hit in hits)
		{
			if (hit.CompareTag("Fighter") || hit.CompareTag("Bomber"))
			{
				if(hit.gameObject == gameObject)
				{
					continue;
				}
            // Optional: Make sure it's an aircraft with needed components
				if (hit.transform.TryGetComponent<AircraftHub>(out var hub))
				{
					nearbyTargets.Add(hit.transform);
				}
				else
				{
					// Fallback: Add anyway if no check needed
					nearbyTargets.Add(hit.transform);
				}
			}
		}
		return nearbyTargets;
	}
	
	List<Transform> SortTargetsByScreenCenter(List<Transform> possibleTargets)
	{
		Vector2 screenCenter = new Vector2(0.5f, 0.5f); // Viewport center
		List<(Transform target, float distance)> scoredTargets = new();

		foreach (var target in possibleTargets)
		{
			Vector3 viewportPos = camera.WorldToViewportPoint(target.position);

			// Skip targets behind the camera
			if (viewportPos.z < 0) continue;

			Vector2 viewport2D = new Vector2(viewportPos.x, viewportPos.y);
			float screenDistance = Vector2.Distance(viewport2D, screenCenter);

			scoredTargets.Add((target, screenDistance));
		}

		// Sort by screen center distance (ascending)
		scoredTargets.Sort((a, b) => a.distance.CompareTo(b.distance));

		// Return just the sorted transforms
		return scoredTargets.Select(t => t.target).ToList();
	}
	
	Transform GetTargetClosestToCenter(List<Transform> possibleTargets, float maxScreenDistance = 1f)
	{
        Transform bestTarget = null;
		float closestDistance = Mathf.Infinity;

		foreach (Transform _target in possibleTargets)
		{
			
			if(hub.fm.target != null)
			{
				if(hub.fm.target.transform == _target)
				{
					continue;
				}	
			}
			
			Vector3 viewportPos = camera.WorldToViewportPoint(_target.position);

			// Check if target is in front of the camera
			if (viewportPos.z < 0)
				continue;

			// Compute distance from screen center (0.5, 0.5 in viewport)
			float dx = viewportPos.x - 0.5f;
			float dy = viewportPos.y - 0.5f;
			float screenDistance = Mathf.Sqrt(dx * dx + dy * dy);

			if (screenDistance < closestDistance && screenDistance <= maxScreenDistance)
			{				
				closestDistance = screenDistance;
				bestTarget = _target;
			}
		}
		return bestTarget;
    }
	
	Transform GetClosestTargetByDistance(Transform self, List<Transform> possibleTargets)
	{
		Transform closestTarget = null;
		float closestSqrDist = Mathf.Infinity;

		foreach (Transform target in possibleTargets)
		{
			AircraftHub hub;
			if (target.TryGetComponent<AircraftHub>(out hub))
			{
				if(hub.fm.side != 0)
				{
					continue;
				}
			}
			
			float sqrDist = (target.position - self.position).sqrMagnitude;

			if (sqrDist < closestSqrDist)
			{
				closestSqrDist = sqrDist;
				closestTarget = target;
			}
		}
		return closestTarget;
	}

    void SearchForTargetCameraLockOld()
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
            if (hub.fm.target != null)
            {
                currentMode = CameraMode.Tracking;
            }
            else
            {
                hub.fm.target = AcquireTarget(transform);
				currentMode = CameraMode.Forward;
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
                    // Get world-space LookAt rotation
                    Quaternion worldLookRotation = Quaternion.LookRotation(hub.fm.target.transform.position - cameraParent.transform.position);

                    // Convert to local space relative to the player's aircraft
                    targetRotation = Quaternion.Inverse(hub.transform.rotation) * worldLookRotation;
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
