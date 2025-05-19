using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;

public class PlayerInputs : MonoBehaviour
{
    public FlightModel playerControls;
    public EngineControl engineControls;
    public float ThrottleIncrement = 1f;

    void Awake()
    {
        playerControls = GetComponent<FlightModel>();
        engineControls = GetComponent<EngineControl>();
    }

    void FixedUpdate()
    {
        ThrottleControl();
        AxisControls();
    }

    private void ThrottleControl()
    {
        engineControls.SetThrottle(Input.GetAxis("Throttle") * Time.deltaTime);
        if(Input.GetKey("joystick button 4") || Input.GetKey(KeyCode.S))
        {
            playerControls.brakeInput = true;
        }
        else
        {
            playerControls.brakeInput = false;
        }
    }

    private void AxisControls()
    {
        DampInputs();
        playerControls.SetControlInput(new Vector3(currentInputVector.x, currentInputVector.y, -currentInputVector.z));

    }
    public float sensitivity = 0.10f;

    public Vector3 currentInputVector;
    private Vector3 smoothVelocity;
    void DampInputs()
    {
        Vector3 inputs = new Vector3(Input.GetAxis("Pitch"), Input.GetAxis("Yaw"), Input.GetAxis("Roll"));
        currentInputVector = Vector3.SmoothDamp(currentInputVector, inputs, ref smoothVelocity, sensitivity);
    }
}
