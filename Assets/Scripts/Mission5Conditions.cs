using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mission5Conditions : MonoBehaviour
{
    [SerializeField] HealthPoints healthPoints;
    [SerializeField] TMP_Text UI;
    [SerializeField] MissionStatus status;

    void Update()
    {
        if(healthPoints == null)
        {
            status.ForceMissionSuccess();
            UI.text = "Enemy Ace's Health: 0 HP";
        }
        if(healthPoints.gameObject.activeSelf == false)
        {
            status.ForceMissionSuccess();
            UI.text = "Enemy Ace's Health: 0 HP";
        }
        UI.text = "Enemy Ace's Health: " + (int)healthPoints.HP + " HP";

        if (healthPoints.HP < 120 && healthPoints.HP > 40)
        {
            UI.color = Color.yellow;
        }
        if (healthPoints.HP <= 40)
        {
            UI.color = Color.red;
        }

    }
}
