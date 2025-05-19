using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mission11Conditions : MonoBehaviour
{
    [SerializeField] MissionStatus status;
    [SerializeField] int killsObjective;
    [SerializeField] bool missionUpdate;
    [SerializeField] AircraftHub whiteDaze;

    [SerializeField] GameObject missionUpdateUI;
    [SerializeField] GameObject reinforcements;
    [SerializeField] GameObject[] objsReinforcements;
    [SerializeField] AudioClip missionUpdBGM;

    [SerializeField] GameObject[] objsEnemies;

    void Update()
    {
        status.KillCountUI.text = "Destroyed: " + status.KillCounter.Kills;
        //
        if (!missionUpdate)
        {
            CheckForRemainingEnemyObjs();
            if (status.MissionTimer < 5f || !objsEnemyRemain || status.KillCounter.Kills > killsObjective)
            {
                status.bgm.clip = missionUpdBGM; status.bgm.Play();
                missionUpdateUI.SetActive(true);
                status.MissionTimer = 300f;
                missionUpdate = true;
            }
        }

        if (missionUpdate)
        {
            if (reinforcements != null)
            {
                reinforcements.gameObject.SetActive(true);
            }

            if (status.MissionTimer > 295)
            {
                missionUpdateUI.SetActive(true);
            }

            else
            {
                missionUpdateUI.SetActive(false);
            }

            if(objReinforcementCount == 1)
            {
                whiteDaze.hp.Defense = 100f;
            }


            CheckForRemainingReinforcementObjs();
            if (!objsReinforcementRemain)
            {
                status.ForceMissionSuccess();
            }
        }
    }

    public bool objsEnemyRemain;
    public int objEnemyCount;
    void CheckForRemainingEnemyObjs()
    {
        objEnemyCount = 0;
        foreach (GameObject go in objsEnemies)
        {
            if (go != null)
            {
                if (go.activeSelf)
                {
                    objEnemyCount++;
                }
            }
        }
        if (objEnemyCount == 0)
        {
            objsEnemyRemain = false;
        }
    }


    public bool objsReinforcementRemain;
    public int objReinforcementCount;
    void CheckForRemainingReinforcementObjs()
    {
        objReinforcementCount = 0;
        foreach (GameObject go in objsReinforcements)
        {
            if (go != null)
            {
                if (go.activeSelf)
                {
                    objReinforcementCount++;
                }
            }
        }
        print(objReinforcementCount);
        if (objReinforcementCount == 0)
        {
            objsReinforcementRemain = false;
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
