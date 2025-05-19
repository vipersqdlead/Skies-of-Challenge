using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Utilities {
    public static float MoveTo(float value, float target, float speed, float deltaTime, float min = 0, float max = 1) {
        float diff = target - value;
        float delta = Mathf.Clamp(diff, -speed * deltaTime, speed * deltaTime);
        return Mathf.Clamp(value + delta, min, max);
    }

    //similar to Vector3.Scale, but has separate factor negative values on each axis
    public static Vector3 Scale6(
        Vector3 value,
        float posX, float negX,
        float posY, float negY,
        float posZ, float negZ
    ) {
        Vector3 result = value;

        if (result.x > 0) {
            result.x *= posX;
        } else if (result.x < 0) {
            result.x *= negX;
        }

        if (result.y > 0) {
            result.y *= posY;
        } else if (result.y < 0) {
            result.y *= negY;
        }

        if (result.z > 0) {
            result.z *= posZ;
        } else if (result.z < 0) {
            result.z *= negZ;
        }

        return result;
    }

    public static float TransformAngle(float angle, float fov, float pixelHeight) {
        return (Mathf.Tan(angle * Mathf.Deg2Rad) / Mathf.Tan(fov / 2 * Mathf.Deg2Rad)) * pixelHeight / 2;
    }

	/*
     * The MIT License (MIT)
     * 
     * Copyright (c) 2008 Daniel Brauer
     * 
     * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
     * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
     * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

     * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

     * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
     * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
     * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
     * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
     */

	//first-order intercept using absolute target position
	public static Vector3 FirstOrderIntercept(
		Vector3 shooterPosition,
		Vector3 shooterVelocity,
		float shotSpeed,
		Vector3 targetPosition,
		Vector3 targetVelocity
	) {
		Vector3 targetRelativePosition = targetPosition - shooterPosition;
		Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
		float t = FirstOrderInterceptTime(
			shotSpeed,
			targetRelativePosition,
			targetRelativeVelocity
		);
		return targetPosition + t * (targetRelativeVelocity);
	}

	//first-order intercept using relative target position
	public static float FirstOrderInterceptTime(
		float shotSpeed,
		Vector3 targetRelativePosition,
		Vector3 targetRelativeVelocity
	) {
		float velocitySquared = targetRelativeVelocity.sqrMagnitude;
		if (velocitySquared < 0.001f) {
			return 0f;
		}

		float a = velocitySquared - shotSpeed * shotSpeed;

		//handle similar velocities
		if (Mathf.Abs(a) < 0.001f) {
			float t = -targetRelativePosition.sqrMagnitude / (2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition));
			return Mathf.Max(t, 0f); //don't shoot back in time
		}

		float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
		float c = targetRelativePosition.sqrMagnitude;
		float determinant = b * b - 4f * a * c;

		if (determinant > 0f) { //determinant > 0; two intercept paths (most common)
			float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
					t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
			if (t1 > 0f) {
				if (t2 > 0f)
					return Mathf.Min(t1, t2); //both are positive
				else {
					return t1; //only t1 is positive
				}
			} else {
				return Mathf.Max(t2, 0f); //don't shoot back in time
			}
		} else if (determinant < 0f) { //determinant < 0; no intercept path
			return 0f;
		} else { //determinant = 0; one intercept path, pretty much never happens
			return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
		}
	}

    public static FlightModel GetNearestTarget(GameObject user, int side, float dist)
    {
		Collider[] colliders = Physics.OverlapSphere(user.transform.position, dist);
        FlightModel nearestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            // Skip self-collisions (if needed)
            if (collider.gameObject == user)
            {
                continue;
            }

            if(collider.CompareTag("Fighter") || collider.CompareTag("Bomber"))
            {
                FlightModel hitFM = collider.GetComponent<FlightModel>();
                if(hitFM != null)
                {
                    if (hitFM.side != side)
                    {
                        float dSqrToTarget = Vector3.Distance(user.transform.position, collider.transform.position);
                        if (dSqrToTarget < closestDistanceSqr)
                        {
                            closestDistanceSqr = dSqrToTarget;
                            nearestTarget = hitFM;
                        }
                    }
                }
            }
            else
            {
                continue;
            }

        }
        if(nearestTarget == null)
        {
            return null;
        }
        else
        {
            return nearestTarget;
        }
    }

    public static AnimationCurve airDensityAnimCurve = new AnimationCurve(
    new Keyframe(-0f, 1f, -0.5f, -0.5f),
    new Keyframe(1.2f, 0.2f, -0.85f, -0.85f));


    

    public static Quaternion SmoothDamp(Quaternion current, Quaternion target, ref Quaternion velocity, float smoothTime)
    {
        float dot = Quaternion.Dot(current, target);
        float multiplier = dot > 0 ? 1f : -1f;
        target.x *= multiplier;
        target.y *= multiplier;
        target.z *= multiplier;
        target.w *= multiplier;

        Vector4 result = new Vector4(
            Mathf.SmoothDamp(current.x, target.x, ref velocity.x, smoothTime),
            Mathf.SmoothDamp(current.y, target.y, ref velocity.y, smoothTime),
            Mathf.SmoothDamp(current.z, target.z, ref velocity.z, smoothTime),
            Mathf.SmoothDamp(current.w, target.w, ref velocity.w, smoothTime)
        ).normalized;

        Quaternion smoothQuat = new Quaternion(result.x, result.y, result.z, result.w);
        return smoothQuat;
    }

    public static Quaternion MissileGuidance(Transform selfTransform, Rigidbody selfRb, GameObject target, float maxGLoad, float launchTime, float rampUpTime)
    {
        Vector3 interceptPoint = Utilities.FirstOrderIntercept(
        selfTransform.position,
        selfRb.velocity,
        selfRb.velocity.magnitude,
        target.transform.position,
        target.GetComponent<Rigidbody>().velocity);

        Vector3 desiredDirection = (interceptPoint - selfTransform.position).normalized;

        // 1. Calculate current speed
        float speed = selfRb.velocity.magnitude;

        // 2. Convert G-load to m/s²
        float maxAccel = maxGLoad * 9.81f;

        // 3. Calculate max angular turn rate in radians/sec
        float maxTurnRate = maxAccel / Mathf.Max(speed, 0.1f); // avoid divide by zero

        // 4. Compute how far along we are in the "turn ramp-up"
        float timeSinceLaunch = Time.time - launchTime;
        float rampProgress = Mathf.Clamp01(timeSinceLaunch / rampUpTime);

        // Optional: smooth with a curve (ease in)
        rampProgress = Mathf.SmoothStep(0f, 1f, rampProgress);

        // 5. Apply turning
        Quaternion currentRot = selfTransform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(desiredDirection);

        // Limit angular velocity
        float maxDelta = maxTurnRate * Mathf.Rad2Deg * Time.fixedDeltaTime * rampProgress;
        return Quaternion.RotateTowards(currentRot, targetRot, maxDelta);
    }

    public static float GetPropEfficiencyNumber(float maxEfficiency, float maxEfficiencyMachSpeed, float currentMach)
    {
        AnimationCurve propEfficiencyCurve = new AnimationCurve(
            new Keyframe(0f, 0f, 5.8f, 5.8f),
            new Keyframe(maxEfficiencyMachSpeed, maxEfficiency, 0f, 0f),
            new Keyframe(maxEfficiencyMachSpeed * 2, 0f, -5.8f, -5.8f));

        return propEfficiencyCurve.Evaluate(currentMach);

    }
}
