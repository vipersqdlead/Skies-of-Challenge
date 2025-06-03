using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpWeaponManager : MonoBehaviour
{
    public List<BaseSpWeaponControl> specialWeapons = new List<BaseSpWeaponControl>();
    private int currentWeaponIndex = -1;

    void Start()
    {
        CycleWeapon();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Change key as needed
        {
            CycleWeapon();
        }
    }

    void CycleWeapon()
    {
        if(currentWeaponIndex < specialWeapons.Count - 1)
        {
            currentWeaponIndex++;
        }
        else
        {
            currentWeaponIndex = 0;
        }

        for (int i = 0; i < specialWeapons.Count; i++)
        {
            if(i == currentWeaponIndex)
            {
                specialWeapons[i].isPlayer = true;
            }
            else
            {
                specialWeapons[i].isPlayer = false;
                specialWeapons[i].DisableWeapon();
            }
        }
    }

}
