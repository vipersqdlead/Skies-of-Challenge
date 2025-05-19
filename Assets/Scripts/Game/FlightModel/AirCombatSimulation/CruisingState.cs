using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CruisingState : StateBase
{
    public List<Transform> Waypoints = new List<Transform>(); // List of waypoints for the plane to go through when searching for targets.
    [SerializeField] int waypointIndex = 0;

    public AIController controller;

    public void OnStateStart(AIController userController)
    {
        controller = userController;
    }

    public override void OnStateStay()
    {
        if (Vector3.Distance(Waypoints[waypointIndex].transform.position, controller.transform.position) < 200)
        {
            waypointIndex++;
        }
        
        if (waypointIndex > Waypoints.Count - 1)
        {
            waypointIndex = 0;
        }

        controller.targetPosition = Waypoints[waypointIndex].position;
        controller.SteerToTarget(Waypoints[waypointIndex].position);
        controller.steering.x = Mathf.Clamp(controller.steering.x, -0.5f, 0.5f);
    }

    public override void OnStateEnd()
    {
        //throw new System.NotImplementedException();
    }
}
