using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mission6Conditions : MonoBehaviour
{
    [SerializeField] GameObject[] objs;
    [SerializeField] MissionStatus status;

    // Update is called once per frame
    void Update()
    {
        CheckForRemainingMissiles();
        if (!objsRemain)
        {
            status.ForceMissionSuccess();
        }
    }

    public bool objsRemain;
    public int objCount;
    void CheckForRemainingMissiles()
    {
        objCount = 0;
        foreach (GameObject go in objs)
        {
            if (go != null)
            {
                if (go.activeSelf)
                {
                    objCount++;
                }
            }
        }
        print(objCount);
        if (objCount == 0)
        {
            objsRemain = false;
        }
    }
}
