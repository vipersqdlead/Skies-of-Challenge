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
	[SerializeField]RadarMissileControl radarController;
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
		Ace,
		Ace2
	}

    public override void OnStateStart(StateUser user)
    {
        controller = user as AIController;
		
		AssignPilotVariables();

		if (controller.plane.target == null)
        {
            controller.plane.target = Utilities.GetNearestTarget(gameObject, controller.plane.side, lookAroundRange);
        }
        if (controller.canUseMissiles)
        {
			if(controller.irMissile) { irController = controller.irMissile; }
			if(controller.radarMissile) { radarController = controller.radarMissile; }
        }

		missileCooldownTime = UnityEngine.Random.Range(10f,30f);
        missileCooldownTimer = missileCooldownTime;
		missileCooldown = true;
    }
	
	public void ChangePilotLevel(int level)
	{
		pilotLevel = (PilotLevel)level;
	}
	
	void AssignPilotVariables()
	{
		switch (pilotLevel)
		{
			case PilotLevel.Novice:
			{
				controller.steeringSpeed = UnityEngine.Random.Range(1.2f, 1.7f);
				controller.reactionDelayMin = UnityEngine.Random.Range(0.4f, 0.6f);
				controller.reactionDelayMax = UnityEngine.Random.Range(1.8f, 2.2f);
				controller.reactionDelayDistance = UnityEngine.Random.Range(180f, 220f);
				controller.minMissileDodgeDistance = UnityEngine.Random.Range(80f, 120f);
				
				controller.plane.gTolerance = UnityEngine.Random.Range(4.7f, 5.2f);
				controller.plane.gMax = UnityEngine.Random.Range(7.8f, 8.2f);
				
				lookAroundCooldownTime = UnityEngine.Random.Range(3f, 6f);
				lookAroundRange = UnityEngine.Random.Range(1500f, 2200f);
				break;
			}
			
			case PilotLevel.Average:
			{
				controller.steeringSpeed = UnityEngine.Random.Range(1.8f, 2.2f);
				controller.reactionDelayMin = UnityEngine.Random.Range(0.35f, 0.45f);
				controller.reactionDelayMax = UnityEngine.Random.Range(1.3f, 1.7f);
				controller.reactionDelayDistance = UnityEngine.Random.Range(330f, 370f);
				controller.minMissileDodgeDistance = UnityEngine.Random.Range(800f, 1500f);
				
				controller.plane.gTolerance = UnityEngine.Random.Range(5.8f, 6.2f);
				controller.plane.gMax = UnityEngine.Random.Range(8.5f, 9.5f);
				
				lookAroundCooldownTime = UnityEngine.Random.Range(1.5f, 3f);
				lookAroundRange = UnityEngine.Random.Range(1800f, 2500f);
				
				flares = controller.hub.flareDispenser;
				break;
			}
			
			case PilotLevel.Experienced:
			{
				controller.steeringSpeed = UnityEngine.Random.Range(2.8f, 3.2f);
				controller.reactionDelayMin = UnityEngine.Random.Range(0.22f, 0.26f);
				controller.reactionDelayMax = UnityEngine.Random.Range(0.7f, 0.9f);
				controller.reactionDelayDistance = UnityEngine.Random.Range(430f, 470f);
				controller.minMissileDodgeDistance = UnityEngine.Random.Range(1200f, 1800f);
				
				controller.plane.gTolerance = UnityEngine.Random.Range(7f, 8f);
				controller.plane.gMax = UnityEngine.Random.Range(10.5f, 11.5f);
				
				controller.advancedGunnery = true;
				
				lookAroundCooldownTime = UnityEngine.Random.Range(0.8f, 2f);
				lookAroundRange = UnityEngine.Random.Range(2300f, 3200f);
				
				flares = controller.hub.flareDispenser;
				break;
			}
			
			case PilotLevel.Ace:
			{
				controller.steeringSpeed = UnityEngine.Random.Range(4.5f, 5.5f);
				controller.reactionDelayMin = UnityEngine.Random.Range(0.08f, 0.12f);
				controller.reactionDelayMax = UnityEngine.Random.Range(0.3f, 0.5f);
				controller.reactionDelayDistance = UnityEngine.Random.Range(600f, 800f);
				controller.minMissileDodgeDistance = UnityEngine.Random.Range(1500f, 2000f);
				
				controller.plane.gTolerance = UnityEngine.Random.Range(8f, 9f);
				controller.plane.gMax = UnityEngine.Random.Range(13f, 15f);
				
				controller.advancedGunnery = true;
				
				lookAroundCooldownTime = UnityEngine.Random.Range(0.15f, 0.6f);
				lookAroundRange = UnityEngine.Random.Range(4500f, 7000f);
				
				flares = controller.hub.flareDispenser;
				break;
			}
			
			case PilotLevel.Ace2:
			{
				controller.steeringSpeed = UnityEngine.Random.Range(7f, 10f);
				controller.reactionDelayMin = UnityEngine.Random.Range(0.04f, 0.08f);
				controller.reactionDelayMax = UnityEngine.Random.Range(0.1f, 0.15f);
				controller.reactionDelayDistance = UnityEngine.Random.Range(850, 1000f);
				controller.minMissileDodgeDistance = UnityEngine.Random.Range(1800, 2500);
				
				controller.plane.gTolerance = UnityEngine.Random.Range(9f, 10f);
				controller.plane.gMax = UnityEngine.Random.Range(14f, 15f);
				
				controller.advancedGunnery = true;
				
				lookAroundCooldownTime = UnityEngine.Random.Range(0.1f, 0.4f);
				lookAroundRange = UnityEngine.Random.Range(6500, 10000);
				
				flares = controller.hub.flareDispenser;
				break;
			}
		}
	}

    public override void OnStateStay()
    {   if(controller == null)
        {
            print("Controller is null on aircraft " + gameObject.name + "!");
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
				float dotProduct = Vector3.Dot(transform.forward, controller.plane.target.transform.forward);
				float wez = 10000f / (irController.Missile.GetComponent<IR_Missile>().maxGLoad * 0.75f);
                if(distanceToTarget > wez && distanceToTarget < Mathf.Min(Utilities.GetIRLockRange(dotProduct, irController.missileLockRange, irController.allAspectSeeker), 5000f))
                {
                    irController.Acquiring = true;
					if (irController.Locked && irController.Target != null)
					{
						if (irController.Target == controller.plane.target.gameObject)
						{
							irController.FireMissile();
							irController.TurnSeekerOff();
							missileCooldown = true;
							return;
						}
					}
                }
                else if(distanceToTarget > 5000f && dotProduct < -0.3f && radarController != null)
				{
					radarController.Acquiring = true;
					if (radarController.Locked && radarController.Target != null)
					{
						if (radarController.Target == controller.plane.target.gameObject && radarController.missile != null)
						{
							radarController.FireMissile();
							missileCooldown = true;
							return;
						}
					}
				}
            }
            else
            {
                irController.TurnSeekerOff();
				
				radarController.TurnSeekerOff();
            }
        }
    }
	
	void CheckForMissiles()
    {
		incomingMissile = Utilities.GetNearestMissile(gameObject, lookAroundRange);
    }
}
