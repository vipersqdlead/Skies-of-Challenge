using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SAM_IR_Launcher : SAM_Launcher
{
    IR_Missile msslControl;

    public override void Update()
    {
        base.Update();
        if (missile != null)
        {
            if(msslControl.target == null)
            {
                msslControl.target = target;
            }
        }
    }

    public override void OpenFire()
    {
        {
            missile = Instantiate(missilePref, msslSpawn.transform.position, msslSpawn.transform.rotation);
            msslControl = missile.GetComponent<IR_Missile>();
            msslControl.target = target;
            Shell msslShell = missile.GetComponent<Shell>();
            msslShell.DmgToAir = 4000;
        }
    }
}
