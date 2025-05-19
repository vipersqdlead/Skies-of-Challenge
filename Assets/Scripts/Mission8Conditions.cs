using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mission8Conditions : MonoBehaviour
{
    [SerializeField] KillCounter killCounter;
    [SerializeField] CruiseMissileState[] missiles;
    [SerializeField] int alliedLosesToFailure;
    [SerializeField] MissionStatus status;
    [SerializeField] TMP_Text alliedLosesToFailureTxt;
    void Start()
    {
        foreach (var m in missiles)
        {
            m.SetKillEnemyDelegate(EnemyKilled);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(killCounter.Kills >= alliedLosesToFailure)
        {
            status.ForceMissionFailure();
        }

        alliedLosesToFailureTxt.text = "Allied Losses: " + killCounter.Kills + "/" + alliedLosesToFailure;

        CheckForRemainingMissiles();

        if(!missilesRemain && killCounter.Kills < alliedLosesToFailure)
        {
            status.ForceMissionSuccess();
        }
    }

    public void EnemyKilled(bool countsAsKill, int points)
    {
         killCounter.Kills++;
    }

    public bool missilesRemain;
    public int objCount;
    void CheckForRemainingMissiles()
    {
        objCount = 0;
        foreach (CruiseMissileState go in missiles)
        {
            if(go != null)
            {
                if (go.gameObject.activeSelf)
                {
                    objCount++;
                }
            }
        }
        print(objCount);
        if(objCount == 0)
        {
            missilesRemain = false;
        }
    }
}
