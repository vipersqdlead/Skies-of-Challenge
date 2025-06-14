using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneHUD : MonoBehaviour 
{
    public Transform hudCenter;
    public Transform velocityMarker;
    public Transform leadMarker;

    AircraftHub hub;
    FlightModel plane;
    Transform planeTransform;
    public new Camera camera;
    Transform cameraTransform;

    GameObject hudCenterGO;
    GameObject velocityMarkerGO;
    GameObject leadMarkerGO;

    void Start()
    {
        hub = GetComponent<AircraftHub>();
        plane = GetComponent<FlightModel>();
        planeTransform = GetComponent<Transform>();
        cameraTransform = camera.transform;
        hudCenterGO = hudCenter.gameObject;
        velocityMarkerGO = velocityMarker.gameObject;
        // leadMarkerGO = leadMarker.gameObject;
    }

    void UpdateVelocityMarker() 
    {
        var velocity = planeTransform.forward;

        if (plane.rb.velocity.sqrMagnitude > 1) 
        {
            velocity = plane.rb.velocity;
        }

        var hudPos = TransformToHUDSpace(cameraTransform.position + velocity);

        if (hudPos.z > 0) 
        {
            velocityMarkerGO.SetActive(true);
            velocityMarker.localPosition = new Vector3(hudPos.x, hudPos.y, 0);
        } 
        else 
        {
            velocityMarkerGO.SetActive(false);
        }
    }

    void ShowLeadMarker()
    {
        if(hub.planeCam.camLockedTarget != null)
        {
            float distToTarget = Vector3.Distance(transform.position, hub.planeCam.camLockedTarget.transform.position);
            if(distToTarget < 500f)
            {
                Vector3 leadPos = Utilities.FirstOrderIntercept(transform.position, hub.rb.velocity, hub.gunsControl.guns[0].muzzleVelocity, hub.planeCam.camLockedTarget.transform.position, hub.planeCam.camLockedTarget.GetComponent<Rigidbody>().velocity);

                var hudPos = TransformToHUDSpace(leadPos);

                if (hudPos.z > 0)
                {
                    leadMarkerGO.SetActive(true);
                    leadMarker.localPosition = new Vector3(hudPos.x, hudPos.y, 0);
                }
                else
                {
                    leadMarkerGO.SetActive(false);
                }
            }
            else
            {
                leadMarkerGO.SetActive(false);
            }
        }
        else
        {
            leadMarkerGO.SetActive(false);
        }
    }

    Vector3 TransformToHUDSpace(Vector3 worldSpace) 
    {
        var screenSpace = camera.WorldToScreenPoint(worldSpace);
        return screenSpace - new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2);
    }

    void UpdateHUDCenter() 
    {
        var rotation = cameraTransform.localEulerAngles;
        var hudPos = TransformToHUDSpace(cameraTransform.position + planeTransform.forward);

        if (hudPos.z > 0) 
        {
            //hudCenterGO.SetActive(true);
            hudCenter.localPosition = new Vector3(hudPos.x, hudPos.y, 0);
            hudCenter.localEulerAngles = new Vector3(0, 0, -rotation.z);
        } 
        //else 
        //{
        //    hudCenterGO.SetActive(false);
        //}
    }

    void Update()
    {
        UpdateVelocityMarker();
        UpdateHUDCenter();
        //ShowLeadMarker();
    }

    private void OnDisable()
    {
        hudCenterGO.SetActive(false);
        velocityMarkerGO.SetActive(false);
    }
}
