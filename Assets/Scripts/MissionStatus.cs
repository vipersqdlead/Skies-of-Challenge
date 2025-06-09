using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;

public class MissionStatus : MonoBehaviour
{
    public int MissionNumber;
    public GameObject BattleUI, MissionFailure, MissionSuccess, Retry, GameOverCamera, MissionStart, PauseUI;
    public KillCounter KillCounter;
    public GameObject player;
    [SerializeField] AircraftHub playerAcHub;
    public float MissionTimer;
    public int KillsObjective;
    public bool MissionEnd = false;
    public Camera overCam, extCam, intCam;
    [SerializeField] float timerToNextMission;
    public bool FinalMission = false;
    public Image BlackBG;
    bool reloadingMission, returningToMenu;
    public DeathCamera deathCam;
    public EnemyMarkers markers;

    public AudioSource bgm, jetSnd;

    public bool isPaused = false;

    [SerializeField] GameObject currentLockedTarget;
    public TMP_Text KillCountUI, PointCount, TimeLeft, currentTarget;
    // Update is called once per frame

    float missionMaxTime;

    int initialPlayerHealth;
    private void Start()
    {
        deathCam.Player = player;
        missionMaxTime = MissionTimer;
        initialPlayerHealth = (int)player.GetComponent<HealthPoints>().HP;
        playerAcHub = player.GetComponent<AircraftHub>();

    }

    void Update()
    {
        if (MissionEnd == false)
        {
            Pause();
        }

        if (reloadingMission)
        {
            MissionRetry();
        }
        if (returningToMenu)
        {
            ReturnToMenu();
        }

        if (MissionTimer < (missionMaxTime - 7f))
        {
            MissionStart.SetActive(false);
        }

        if (BlackBG.fillAmount > 0 && MissionEnd == false)
        {
            Fade(true);
        }

        TargetTracking();
        TimeLeft.text = "Time: " + (int)MissionTimer;
        KillCountUI.text = "Destroyed: " + KillCounter.Kills + "/" + KillsObjective;
        PointCount.text = "Points: " + KillCounter.Points;
        BlackBG.fillClockwise = true;

        if (!MissionEnd)
        {
            MissionTimer -= Time.deltaTime;
        }
        if (MissionTimer < 0 || player == null)
        {
            MissionEnd = true;
        }

        if (KillCounter.Kills >= KillsObjective && player != null && MissionTimer > 0f || forceMissionSuccess)
        {
            CalculateFinalScore();
            MissionAccomplished();
        }

        if ((KillCounter.Kills < KillsObjective && MissionTimer < 0) || player == null || forceMissionFailure)
        {
            MissionFail();
        }

        if (SetRetry == true)
        {
            MissionRetry();
        }
    }

    void TargetTracking()
    {
        if (playerAcHub.fm.target != null)
        {
            currentTarget.text = "Target: " + playerAcHub.fm.target.gameObject.name + " (" + playerAcHub.fm.target.health.pointsWorth + ")";
        }
        else
        {
            currentTarget.text = "Target: None";
        }
		if(playerAcHub.fm.target != null)
		{
			markers.targetLockedHub = playerAcHub.fm.target.hub;
		}
		else
		{
			markers.targetLockedHub = null;
		}
    }
    void MissionAccomplished()
    {
        bgm.gameObject.SetActive(false);

        if (player != null)
        {
            print("Mission Accomplished!");
            BattleUI.SetActive(false);
            MissionSuccess.SetActive(true);
            MissionEnd = true;
            timerToNextMission += Time.deltaTime;
            if (timerToNextMission >= 5f)
            {
                Fade(false);
            }
            if (timerToNextMission >= 7f)
            {
                SaveScore();
                if (!FinalMission)
                {
                    SceneManager.LoadScene(MissionNumber + 1);
                }
                if (FinalMission)
                {
                    Time.timeScale = 1f;
                    SceneManager.LoadScene("EndingScreen");
                }
            }
        }
    }

    bool SetRetry = false;
    void MissionFail()
    {
        bgm.gameObject.SetActive(false);
        BattleUI.SetActive(false);
        MissionSuccess.SetActive(false);
        MissionFailure.SetActive(true);
        KillCounter.Points = 0;
        MissionEnd = true;
        if (SetRetry == false)
        {
            timerToNextMission += Time.deltaTime;
            if (timerToNextMission > 1f)
            {
                Retry.SetActive(true);
                if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetAxis("FireCannon") != 0)
                {
                    SetRetry = true;
                    Retry.SetActive(false);
                    timerToNextMission = 0f;
                }
            }
            if (timerToNextMission >= 8f)
            {
                Retry.SetActive(false);
                Fade(false);
            }
            if (timerToNextMission >= 10f)
            {
                Time.timeScale = 1f;
                SaveScore();
                SceneManager.LoadScene("ResultsScreen");
            }
        }
    }

    void Pause()
    {
        PauseUI.SetActive(isPaused);
        if (Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.P))
        {
            if (!isPaused)
            {
                Time.timeScale = 0f;
                isPaused = true;
                bgm.Pause();
                BattleUI.SetActive(false);
            }
            else if (isPaused)
            {
                Time.timeScale = 1f;
                isPaused = false;
                bgm.UnPause();
                BattleUI.SetActive(true);
            }
        }

        if (isPaused)
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton3))
            {
                buttonRetrying();
            }

            if(Input.GetKeyDown(KeyCode.Joystick1Button4))
            {
                buttonReturnToMenu();
            }
        }
    }

    public void UnPause()
    {
        isPaused = false;
        Time.timeScale = 1f;
        bgm.UnPause();
    }

    public void buttonReturnToMenu()
    {
        UnPause();
        MissionEnd = true;
        returningToMenu = true;
    }

    public void buttonRetrying()
    {
        print("Retrying");
        UnPause();
        MissionEnd = true;
        reloadingMission = true;
    }

    void ReturnToMenu()
    {
        Fade(false);
        timerToNextMission += Time.unscaledDeltaTime;
        if (timerToNextMission >= 2f)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }
    }

    void MissionRetry()
    {
        Fade(false);
        timerToNextMission += Time.unscaledDeltaTime;
        if (timerToNextMission >= 2f)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(MissionNumber);
        }
    }

    int timeBonus;
    int finalScore;
    int noDamageBonus = 5000;
    bool finalScoreCalculated;
    void CalculateFinalScore()
    {
        if (!finalScoreCalculated)
        {
            timeBonus = (int)MissionTimer * 100;
            if (initialPlayerHealth == (int)player.GetComponent<HealthPoints>().HP)
            {
                finalScore = KillCounter.Points + timeBonus + noDamageBonus;
            }
            else
            {
                finalScore = KillCounter.Points + timeBonus + ((int)player.GetComponent<HealthPoints>().HP * 10);
            }
            print(finalScore);
            finalScoreCalculated = true;
        }
    }

    void SaveScore()
    {
        PlayerPrefs.SetInt("Mission" + MissionNumber + "Score", finalScore);
        PlayerPrefs.Save();
    }

    void Fade(bool fadeInOrOut)
    {
        if (fadeInOrOut)
        {
            BlackBG.fillOrigin = 1;
            BlackBG.fillAmount -= Time.deltaTime * 2f;
            
        }

        if(!fadeInOrOut)
        {
            BlackBG.fillOrigin = 2;
            BlackBG.fillAmount += Time.deltaTime * 2f;
        }
    }

    bool forceMissionSuccess;
    public void ForceMissionSuccess()
    {
        forceMissionSuccess = true;
        MissionEnd = true;
    }

    bool forceMissionFailure;
    public void ForceMissionFailure()
    {
        MissionEnd = true;
        forceMissionFailure = true;
    }
}
