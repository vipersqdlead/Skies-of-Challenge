using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAM_TrackRadar : MonoBehaviour
{
    private ActivateAirDefenseEventChannel activateAirDefneseEventChannel;

    private void Awake()
    {
        activateAirDefneseEventChannel.AddListenerStartDefensesEvent(OnStartDefenseEvent);
    }

    void OnStartDefenseEvent(Transform[] enemiesDetected)
    {

    }

    private void OnDestroy()
    {
        activateAirDefneseEventChannel.RemoveListenerStartDefensesEvent(OnStartDefenseEvent);
    }
}
