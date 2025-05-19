using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterAceKills : MonoBehaviour
{
    public string aceName;
    int currentStats;

    private void Start()
    {
        currentStats = PlayerPrefs.GetInt(aceName + " Times Killed");
    }

    private void OnDestroy()
    {
        currentStats += 1;
        PlayerPrefs.SetInt(aceName + " Times Killed", currentStats);
    }
}
