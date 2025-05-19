using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSpWeaponControl : MonoBehaviour
{
    public string weaponName;
    public bool isPlayer;


    public abstract void DisableWeapon();
}
