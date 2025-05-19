using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MissionSelector : MonoBehaviour
{
    [SerializeField] Button[] missionButtons;
    public int index = 0;
    [SerializeField] MainMenu menu;

    void Start()
    {
        missionButtons[index].targetGraphic.color = Color.red;
    }

    public bool isNewInput;
    void Update()
    {
        if(Input.GetAxis("Roll") < -0.8f && !isNewInput)
        {
            if(index < missionButtons.Length - 1)
            {
                index++;
                missionButtons[index].targetGraphic.color = Color.red;
                missionButtons[index - 1].targetGraphic.color = Color.white;

            }
            isNewInput = true;
        }
        else if(Input.GetAxis("Roll") > 0.8f && !isNewInput)
        {
            if (index > 0)
            {
                index--;
                missionButtons[index].targetGraphic.color = Color.red;
                missionButtons[index + 1].targetGraphic.color = Color.white;
            }
            isNewInput = true;
        }
        else if(Input.GetAxis("Roll") == 0f)
        {
            isNewInput = false;
        }

        print(isNewInput);
        if (Input.GetAxis("FireCannon") != 0)
        {
            menu.LoadMissionButton(index + 1);
        }
    }
}
