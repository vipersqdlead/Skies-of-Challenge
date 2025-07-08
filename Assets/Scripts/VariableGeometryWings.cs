using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableGeometryWings : MonoBehaviour
{
    public GameObject leftWing;
    public GameObject rightWing;

    public float minWingAngle = 28f;
    public float maxWingAngle = 0f;
    public float minWingPosition;
    public float maxWingPosition;
    public float minSpeed = 130f;
    public float maxSpeed = 330f;

    public float minSweepDrag, maxSweepDrag, drag;

    public float initialWingspan = 13.7f; // Wingspan when fully extended
    public float finalWingspan;
    public float wingArea = 38.5f; // Total wing area (assumed constant)

    public float currentWingspan;
    public float currentPosition;

    private Rigidbody rb;
    private FlightModel flightModel;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        flightModel = GetComponent<FlightModel>();
    }

    void Update()
    {
        AdjustWingSweep();
        UpdateAspectRatio();
        ChangeWingPosition();
    }

    void AdjustWingSweep()
    {
        float speed = rb.velocity.magnitude * 3.6f;
        float t = Mathf.InverseLerp(minSpeed, maxSpeed, speed);
        float wingAngle = Mathf.Lerp(minWingAngle, maxWingAngle, t);
        drag = Mathf.Lerp(minSweepDrag, maxSweepDrag, t);

        leftWing.transform.localRotation = Quaternion.Euler(0f, wingAngle, 0f);
        rightWing.transform.localRotation = Quaternion.Euler(0f, -wingAngle, 0f);
    }

    void UpdateAspectRatio()
    {
		if(flightModel == null) { return; }
		
        float speed = rb.velocity.magnitude * 3.6f;
        float t = Mathf.InverseLerp(minSpeed, maxSpeed, speed);

        currentWingspan = Mathf.Lerp(initialWingspan, finalWingspan, t);

        // Calculate the effective wingspan based on the sweep angle
        //currentWingspan = initialWingspan * Mathf.Cos(Mathf.Deg2Rad * Mathf.Lerp(minWingAngle, maxWingAngle, t));

        // Update the flight characteristics with the new aspect ratio
        flightModel.UpdateAspectRatio(currentWingspan);
        flightModel.drag = drag;
    }

    void ChangeWingPosition()
    {
        if(minWingPosition == 0f)
        {
            return;
        }

        float speed = rb.velocity.magnitude * 3.6f;
        float t = Mathf.InverseLerp(minSpeed, maxSpeed, speed);

        currentPosition = Mathf.Lerp(minWingPosition, maxWingPosition, t);

        leftWing.transform.localPosition = new Vector3(leftWing.transform.localPosition.x, leftWing.transform.localPosition.y, currentPosition);
        rightWing.transform.localPosition = new Vector3(rightWing.transform.localPosition.x, rightWing.transform.localPosition.y, currentPosition);

    }
}
