using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using static SurvivalSettings;

public class MainMenu : MonoBehaviour
{

    public FadeInOut fadeinOut;
    public AudioSource BGM;
    bool loadMission;
    int scenetoLoad;
    [SerializeField] int quickGameAircraftSelectionLimit;
    [SerializeField] GameObject missionSelector;
    [SerializeField] MenuType menuType;

    [SerializeField] TMP_Text maxScoreTxt, lastScoreTxt;
    [SerializeField] GameObject controlsButton, controlsText;

    public enum MenuType
    {
        MainMenu,
        ResultsMenu,
        MissionSelectorMenu,
        EndingMenu
    }

    // Start is called before the first frame update
    void Start()
    {
        fadeinOut.ActivateFadeIn = true;
        if(menuType == MenuType.MainMenu)
        {
            maxScoreTxt.text = "Best Score: " + PlayerPrefs.GetInt("MaxScore") + " pts.";
            lastScoreTxt.text = "Last Score: " + PlayerPrefs.GetInt("LastScore") + " pts.";
        }
    }

    float timerToLoad;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            PlayerPrefs.DeleteAll();
            CloseGame();
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            PlayerPrefs.SetInt("MissionSelectKey", 1);
            CloseGame();
        }

        if (loadMission ==true)
        {
            BGM.volume -= Time.deltaTime;
            timerToLoad += Time.deltaTime;
            if(timerToLoad > 2)
            {
                LoadScene(scenetoLoad);
            }
        }

        switch (menuType)
        {
            case MenuType.MainMenu:
                {
                    ////if (Input.GetKeyDown(KeyCode.JoystickButton0))
                    ////{
                    ////    LoadMissionButton(1);
                    ////}

                    //if (PlayerPrefs.GetInt("MissionSelectKey") == 1)
                    //{
                    //    if (Input.GetAxis("FireWeapon2") != 0)
                    //    {
                    //        LoadMissionButton(13);
                    //    }
                    //}

                    //if (Input.GetAxis("Zoom") != 0)
                    //{
                    //    CloseGame();
                    //}

                    //if (Input.GetKeyDown(KeyCode.JoystickButton6))
                    //{
                    //    controlsButton.SetActive(false); controlsText.SetActive(true);
                    //}
                    break;
                }
            case MenuType.ResultsMenu:
                {
                    if (Input.GetAxis("FireCannon") != 0)
                    {
                        LoadMissionButton(0);
                    }
                    break;
                }
            case MenuType.MissionSelectorMenu:
                {
                    if (Input.GetAxis("FireWeapon2") != 0)
                    {
                        LoadMissionButton(0);
                    }
                    break;
                }
            case MenuType.EndingMenu:
                {
                    if (Input.GetAxis("FireCannon") != 0)
                    {
                        LoadMissionButton(12);
                    }
                    break;
                }
        }
    }
    public void LoadMissionButton(int sceneNo)
    {
        print("Fade");
        fadeinOut.ActivateFadeOut = true;
        loadMission = true;
        scenetoLoad = sceneNo;
    }

    public void LoadScene(int sceneNo)
    {
        SceneManager.LoadScene(sceneNo);
    }

    public void QuickPlay()
    {
        PlayerPrefs.SetInt("Survival Aircraft", Random.Range(0, quickGameAircraftSelectionLimit));
        PlayerPrefs.SetInt("Survival Map", Random.Range(0, 10));
        LoadMissionButton(16);
    }

    public void CloseGame()
    {
        BGM.volume -= Time.deltaTime;
        fadeinOut.ActivateFadeOut = true;
        Invoke("Quit", 1.8f);
    }

    void Quit()
    {
        Application.Quit();
    }
}
