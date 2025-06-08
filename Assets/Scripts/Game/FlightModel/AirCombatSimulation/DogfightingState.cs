using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DogfightingState : StateBase
{
    public AIController controller;
    public float targetLookRange = 50000f;
	public Rigidbody incomingMissile;
	public PilotLevel pilotLevel;
    [SerializeField]IRMissileControl irController;
	[SerializeField]FlareDispenser flares;
    bool missileCooldown;
    public float missileCooldownTime;
    public float missileCooldownTimer;
	public float lookAroundCooldownTime;
    public float lookAroundCooldownTimer;
	public float lookAroundRange;
	
	public enum PilotLevel
	{
		Novice,
		Average,
		Experienced,
		Ace
	}

    public override void OnStateStart(StateUser user)
    {
        controller = user as AIController;
        if (controller.plane.target == null)
        {
            controller.plane.target = Utilities.GetNearestTarget(gameObject, controller.plane.side, targetLookRange);
        }
        if (controller.canUseMissiles)
        {
            irController = GetComponent<IRMissileControl>();
        }
		
		switch (pilotLevel)
		{
			case PilotLevel.Novice:
			{
				controller.steeringSpeed = 1f;
				controller.reactionDelayMin = 0.5f;
				controller.reactionDelayMax = 2.5f;
				controller.reactionDelayDistance = 200f;
				controller.minMissileDodgeDistance = 100f;
				lookAroundCooldownTime = UnityEngine.Random.Range(3f, 6f);
				lookAroundRange = UnityEngine.Random.Range(1500f, 2200f);
				break;
			}
			
			case PilotLevel.Average:
			{
				controller.steeringSpeed = 2f;
				controller.reactionDelayMin = 0.4f;
				controller.reactionDelayMax = 1.5f;
				controller.reactionDelayDistance = 350f;
				controller.minMissileDodgeDistance = UnityEngine.Random.Range(800f, 1500f);
				lookAroundCooldownTime = UnityEngine.Random.Range(1.5f, 3f);
				lookAroundRange = UnityEngine.Random.Range(1800f, 2500f);
				flares = controller.hub.flareDispenser;
				break;
			}
			
			case PilotLevel.Experienced:
			{
				controller.steeringSpeed = 3f;
				controller.reactionDelayMin = 0.25f;
				controller.reactionDelayMax = 0.8f;
				controller.reactionDelayDistance = 450f;
				controller.minMissileDodgeDistance = UnityEngine.Random.Range(1200f, 1800f);
				lookAroundCooldownTime = UnityEngine.Random.Range(0.8f, 2f);
				lookAroundRange = UnityEngine.Random.Range(2300f, 3200f);
				flares = controller.hub.flareDispenser;
				break;
			}
			
			case PilotLevel.Ace:
			{
				controller.steeringSpeed = 5f;
				controller.reactionDelayMin = 0.1f;
				controller.reactionDelayMax = 0.5f;
				controller.reactionDelayDistance = 650f;
				controller.minMissileDodgeDistance = UnityEngine.Random.Range(1500f, 2000f);
				lookAroundCooldownTime = UnityEngine.Random.Range(0.15f, 0.6f);
				lookAroundRange = UnityEngine.Random.Range(4500f, 7000f);
				flares = controller.hub.flareDispenser;
				break;
			}
		}

        missileCooldownTimer = missileCooldownTime;

    }

    public override void OnStateStay()
    {   if(controller == null)
        {
            print("Controller is null!");
            return;
        }
        
        if(controller.plane.target == null)
        {
            LookingForTargets();
            controller.guns.trigger = false;
            return;
        }

        if (incomingMissile != null)
        {
            if (controller.dodging == false)
            {
                //start dodging
                controller.dodging = true;
                controller.lastDodgePoint = controller.plane.rb.position;
                controller.dodgeTimer = 0;
				controller.plane.target = Utilities.GetNearestTarget(gameObject, controller.plane.side, targetLookRange); 
            }

            var dodgePosition = controller.GetMissileDodgePosition(Time.deltaTime, incomingMissile);
            controller.steering = controller.CalculateSteering(dodgePosition);
            controller.emergency = true;
        }
        else
        {
			controller.dodging = false;
			controller.targetPosition = controller.GetTargetPosition();
        }
		
		if(flares != null)
		{
			flares.trigger = controller.dodging;
		}

        if (incomingMissile == null && (controller.plane.currentSpeed < controller.recoverSpeedMin || controller.isRecoveringSpeed))
        {
            controller.isRecoveringSpeed = controller.plane.currentSpeed < controller.recoverSpeedMax;

            controller.steering = controller.RecoverSpeed();
            controller.throttle = 1;
            controller.emergency = true;
        }
        else
        {
            controller.emergency = false;
            controller.throttle = controller.CalculateThrottle(controller.minSpeed, controller.maxSpeed);
        }

        controller.engineControl.SetThrottle(controller.throttle);

        if (controller.emergency)
        {
            controller.guns.trigger = false;
            controller.plane.SetControlInput(controller.steering);
            return;
        }
        else if(controller.plane.target != null)
        {
            controller.SteerToTarget(controller.plane.target.transform.position);
            controller.CalculateWeapons(Time.fixedDeltaTime);
            if (!missileCooldown)
            {
                CalculateMissiles();
            }
            else if (missileCooldown)
            {
                missileCooldownTimer -= Time.deltaTime;
                if(missileCooldownTimer < 0)
                {
                    missileCooldownTimer = missileCooldownTime;
                    missileCooldown = false;
                }
            }
        }
		    if (lookAroundCooldownTimer <= 0f)
            {
                CheckForMissiles();
				lookAroundCooldownTimer = lookAroundCooldownTime;
            }
            else if (lookAroundCooldownTimer > 0f)
            {
                lookAroundCooldownTimer -= Time.deltaTime;
            }
		
    }

    public override void OnStateEnd()
    {
        throw new System.NotImplementedException();
    }

    float lookTimer;
    void LookingForTargets()
    {
        lookTimer += Time.deltaTime;
        if(lookTimer > lookAroundCooldownTime)
        {
            controller.plane.target = Utilities.GetNearestTarget(gameObject, controller.plane.side, targetLookRange);
            lookTimer = 0f;
        }
    }

    void CalculateMissiles()
    {
        if(controller.canUseMissiles)
        {
            if (missileCooldown)
            {
                return;
            }

            float angleToPlayer = Mathf.Abs(Vector3.Angle(transform.forward, controller.plane.target.transform.position - transform.position));
            if (angleToPlayer < 45f)
            {
                float distanceToTarget = Vector3.Distance(transform.position, controller.plane.target.transform.position);
                if(distanceToTarget > controller.cannonRange)
                {
                    irController.Acquiring = true;
                }
                if (irController.Locked && irController.Target != null)
                {
                    if (irController.Target == controller.plane.target.gameObject)
                    {
                        if (!missileCooldown)
                        {
                            irController.FireMissile();
                            irController.Locked = false;
                            irController.Acquiring = false;
                            missileCooldown = true;
                            return;
                        }
                    }
                }
            }
            else
            {
                irController.Locked = false;
                irController.Acquiring = false;
            }
        }
    }
	
	void CheckForMissiles()
    {
		incomingMissile = Utilities.GetNearestMissile(gameObject, lookAroundRange);
    }
}
