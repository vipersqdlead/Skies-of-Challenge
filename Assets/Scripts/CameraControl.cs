using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    public GameObject ExtCamera, cameraPivot;

    // Start is called before the first frame update
    void Start()
    {
        ExtCamera.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    { 
        if(Input.GetKey(KeyCode.Z)) 
        {
            ExtCamera.GetComponent<Camera>().fieldOfView = 25;
        }
        else
        {
            ExtCamera.GetComponent<Camera>().fieldOfView = 60;
        }

        {
            if (Input.GetKey(KeyCode.Keypad4))
            {
                cameraPivot.transform.localRotation = Quaternion.Euler(0, 270, 0);
            }
            else if (Input.GetKey(KeyCode.Keypad6))
            {
                cameraPivot.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
            else if (Input.GetKey(KeyCode.Keypad2))
            {
                cameraPivot.transform.localRotation = Quaternion.Euler(90, 0, 0);
            }
            else if (Input.GetKey(KeyCode.Keypad8))
            {
                cameraPivot.transform.localRotation = Quaternion.Euler(270, 0, 0);
            }
            else if (Input.GetKey(KeyCode.Keypad0))
            {
                cameraPivot.transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                cameraPivot.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}
