using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAirDefenseEventChannel : MonoBehaviour
{
    public delegate void StartDefensesDelegate(Transform[] enemiesDetected);
    private event StartDefensesDelegate startDefensesEvent;

    public void AddListenerStartDefensesEvent(StartDefensesDelegate _onStartDefense)
    {
        startDefensesEvent += _onStartDefense;
    }
    public void RemoveListenerStartDefensesEvent(StartDefensesDelegate _onStartDefense)
    {
        startDefensesEvent -= _onStartDefense;
    }

    public void InvokeStartDefensesEvent(Transform[] enemiesDetected)
    {
        startDefensesEvent?.Invoke(enemiesDetected);
    }
}
