using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mission9Conditions : MonoBehaviour
{
    [SerializeField] MissionStatus status;
    [SerializeField] int killsObjective;
    [SerializeField] bool missionUpdate;

    [SerializeField] TMP_Text enemyAceHealthUI;
    [SerializeField] GameObject missionUpdateUI;
    [SerializeField] HealthPoints enemyAceHP;

    [SerializeField] AudioClip missionUpdBGM;

    void Start()
    {
        
    }

    void LateUpdate()
    {
        if(!missionUpdate)
        {
            status.KillCountUI.text = "Destroyed: " + status.KillCounter.Kills + "/" + killsObjective;
            if (status.KillCounter.Kills >= killsObjective) 
            {
                status.KillCountUI.enabled = false;
                status.bgm.clip = missionUpdBGM; status.bgm.Play();
                missionUpdateUI.SetActive(true);
                status.MissionTimer = 180f;
                missionUpdate = true;
            }
        }

        if (missionUpdate)
        {
            if(status.player != null)
            {
                if (enemyAceHP != null)
                {
                    enemyAceHP.gameObject.SetActive(true);
                }
                if (status.MissionTimer > 175f)
                {
                    missionUpdateUI.SetActive(true);
                }
                else
                {
                    missionUpdateUI.SetActive(false);
                }
                enemyAceHealthUI.enabled = true;
                enemyAceHealthUI.text = "Enemy Ace's Health: " + (int)enemyAceHP.HP + " HP";

                if (enemyAceHP.HP < 120 && enemyAceHP.HP > 40)
                {
                    enemyAceHealthUI.color = Color.yellow;
                }
                if (enemyAceHP.HP <= 40)
                {
                    enemyAceHealthUI.color = Color.red;
                }

                if (enemyAceHP == null)
                {
                    status.ForceMissionSuccess();
                    enemyAceHealthUI.text = "Enemy Ace's Health: 0 HP";
                }

                if (enemyAceHP.gameObject.activeSelf == false)
                {
                    status.ForceMissionSuccess();
                    enemyAceHealthUI.text = "Enemy Ace's Health: 0 HP";
                }
            }
        }
    }
}
