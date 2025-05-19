using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Mission7Conditions : MonoBehaviour
{
    [SerializeField] MissionStatus status;
    [SerializeField] AIBomberModel transport;
    [SerializeField] HealthPoints healthPoints;
    [SerializeField] TMP_Text UI;
    [SerializeField] float dist;

    private void Update()
    {
        dist = Vector3.Distance(transform.position, transport.Target.transform.position);

        if (transform.position.z > 20000f)
        {
            status.ForceMissionSuccess();
        }

        UI.text = "Transport's Health: " + (int)healthPoints.HP + " HP";

        if (healthPoints.HP < 1250 && healthPoints.HP > 500)
        {
            UI.color = Color.yellow;
        }
        if (healthPoints.HP <= 500)
        {
            UI.color = Color.red;
        }
    }

    private void OnDisable()
    {
        UI.gameObject.SetActive(false);
        status.ForceMissionFailure();
    }
}
