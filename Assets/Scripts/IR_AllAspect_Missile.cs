using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IR_AllAspect_Missile : IR_Missile
{
    KillEnemy delKillEnemy;
    
    /*
    public override void Aquire()
    {

        if (target == null)
        {
            RaycastHit hit;
            float thickness = 60f; //<-- Desired thickness here
            if (Physics.SphereCast(transform.position, thickness, transform.forward, out hit))
            {

                if (hit.collider.CompareTag("Flare"))
                {
                    target = hit.collider.gameObject;
                    print("Flare'd!");
                    return; // Prioritize the first flare detected
                }

                if (hit.collider.gameObject.CompareTag("Fighter") || hit.collider.gameObject.CompareTag("Bomber"))
                {
                    if (hit.collider.gameObject.CompareTag("Fighter") || hit.collider.gameObject.CompareTag("Bomber"))
                    {
                        target = hit.collider.gameObject;
                    }
                }
            }
        }

        else
        {
            RaycastHit hit;
            float thickness = 20f; //<-- Desired thickness here
            if (Physics.SphereCast(transform.position, thickness, target.transform.position - transform.position, out hit))
            {

                if (hit.collider.gameObject.CompareTag("Fighter") || hit.collider.gameObject.CompareTag("Bomber") || hit.collider.gameObject.CompareTag("Flare"))
                {
                    if (hit.collider.CompareTag("Flare"))
                    {
                        target = hit.collider.gameObject;
                        print("Flare'd!");
                        return; // Prioritize the first flare detected
                    }

                    if (hit.collider.gameObject.CompareTag("Fighter") || hit.collider.gameObject.CompareTag("Bomber"))
                    {
                        if (hit.collider.gameObject.CompareTag("Fighter") || hit.collider.gameObject.CompareTag("Bomber"))
                        {
                            target = hit.collider.gameObject;
                        }
                    }

                }
            }
        }
    }*/
}
