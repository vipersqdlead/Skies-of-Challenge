using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarSearch : MonoBehaviour
{
    public ActivateAirDefenseEventChannel airDefenseEventChannel;
    Transform[] targetsDetected;

    private void OnTriggerEnter(Collider other)
    {
        airDefenseEventChannel.InvokeStartDefensesEvent(targetsDetected);
    }
}
