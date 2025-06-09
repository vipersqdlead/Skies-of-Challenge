using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    public AircraftHub[] aircraft;
    void Start()
    {
    }

    public void AddRenderersToMarker(EnemyMarkers markers, SurvivalMissionStatus status, FlightModel player)
    {
        for (int i = 0; i < aircraft.Length; i++)
        {
            markers.AddMarker(aircraft[i]);
            if(i == 0)
            {
                aircraft[i].fm.target = player;
            }
            status.enemyFighters.Add(aircraft[i]);
        }
    }

    public void AddAllyRenderersToMarker(EnemyMarkers markers, SurvivalMissionStatus status)
    {
        for (int i = 0; i < aircraft.Length; i++)
        {
            aircraft[i].fm.side = 1;
            aircraft[i].fm.health.countsAsKill = false;
            aircraft[i].fm.health.pointsWorth = -1000;
            markers.AddAllyMarker(aircraft[i]);
        }
    }
}
