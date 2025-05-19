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
    [SerializeField]IRMissileControl irController;
    bool missileCooldown;
    public float missileCooldownTime;
    public float missileCooldownTimer;

    public void OnStateStart(AIController userController)
    {
        controller = userController;
        if (controller.plane.target == null)
        {
            controller.plane.target = Utilities.GetNearestTarget(gameObject, controller.plane.side, targetLookRange);
            print(controller.plane.target.ToString());
        }
        if (controller.canUseMissiles)
        {
            irController = GetComponent<IRMissileControl>();
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

        //var incomingMissile = selfTarget.GetIncomingMissile();
        //if (incomingMissile != null)
        //{
        //    if (dodging == false)
        //    {
        //        //start dodging
        //        dodging = true;
        //        lastDodgePoint = plane.rb.position;
        //        dodgeTimer = 0;
        //    }

        //    var dodgePosition = GetMissileDodgePosition(dt, incomingMissile);
        //    steering = CalculateSteering(dodgePosition);
        //    emergency = true;
        //}
        //else
        //{
        controller.dodging = false;
        controller.targetPosition = controller.GetTargetPosition();
        //}

        if ((controller.plane.currentSpeed < controller.recoverSpeedMin || controller.isRecoveringSpeed))
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
    }

    public override void OnStateEnd()
    {
        throw new System.NotImplementedException();
    }

    float lookTimer;
    void LookingForTargets()
    {
        lookTimer += Time.deltaTime;
        if(lookTimer > 5f)
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
}
