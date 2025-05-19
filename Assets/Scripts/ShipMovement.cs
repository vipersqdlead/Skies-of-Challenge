using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{

    [SerializeField] Rigidbody shipRb;
    [SerializeField] float shipSpeed;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        shipRb.AddForce(transform.forward * shipSpeed * Time.deltaTime, ForceMode.Force);
    }
}
