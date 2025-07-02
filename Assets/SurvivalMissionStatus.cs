using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static SurvivalSettings;

public class SurvivalMissionStatus : MonoBehaviour
{
    public GameObject BattleUI, MissionSuccess, Retry, GameOverCamera, MissionStart, PauseUI;
    public KillCounter KillCounter;
    public int points;
    int kills;
    public GameObject Player;
    public string aircraftName;
	[SerializeField] int battleRating;
    [SerializeField] AircraftHub playerAcHub;
    public AircraftType aircraftType;
    public float MissionTimer;
    public bool missionEnd = false;
    public Camera overCam, extCam;
    [SerializeField] float timerToMenu;
    public Image BlackBG;
    bool reloadingMission, returningToMenu;
    public AudioSource bgm;
    public Slider bgmVolume;
    public DeathCamera deathCam;
    public bool isPaused = false;
    public int currentWave = 1;
    public WaveSpawner waveSpawner;
    public EnemyMarkers markers;
    public AudioListener camListener;


    public bool enableWaveBonus, startWithBonus, startWithAllies, missilesAllowed;


    [SerializeField] GameObject currentLockedTarget;
    public TMP_Text KillCountUI, PointCount, TimeLeft, currentWaveUI, newWaveText, newWaveBonusText, bonusWaveObjectiveText, enemiesLeftText, mapBoundaryWarning, mEnd_TimeBonus, mEnd_PointScore, mEnd_FinalScore, fpsCounterTxt;
    public AudioSource mapBoundaryWarningAS;
    public AudioClip mapBoundaryWarningLight, mapBoundaryWarningStrong;

    public enum AircraftType
    {
        Trainer,
        Prop,
        JetTier1,
        JetTier2
    }

    // Update is called once per frame
    void Start()
    {
        Fade(true);
        playerAcHub = Player.GetComponent<AircraftHub>();
		
		battleRating = playerAcHub.hp.pointsWorth;
		waveSpawner.currentDifficulty = battleRating;
		waveSpawner.OrderLists();
		
        //MissionStart.GetComponent<AudioSource>().Play();

        bgm.volume = PlayerPrefs.GetFloat("BGM Volume", 0.7f);
        bgmVolume.value = bgm.volume;

        if (missilesAllowed)
        {
            if(playerAcHub.irControl != null)
            {
                playerAcHub.irControl.MissileAmmo = playerAcHub.irControl.MaxMissileAmmo;
                playerAcHub.irControl.canReload = true;
            }

            if (playerAcHub.radarMissileControl != null)
            {
                playerAcHub.radarMissileControl.MissileAmmo = playerAcHub.radarMissileControl.MaxMissileAmmo;
                playerAcHub.radarMissileControl.canReload = true;
            }

            if (playerAcHub.sagControl != null)
            {
                playerAcHub.sagControl.MissileAmmo = playerAcHub.sagControl.MaxMissileAmmo;
                playerAcHub.sagControl.canReload = true;
            }
        }
		

    }

    // Update is called once per frame
    void Update()
    {
        if (!missionEnd)
        {
            Fade(true);
            MissionTimer += Time.deltaTime;
            UpdateUI();
            Pause();
            bgm.volume = bgmVolume.value;
        }

        if (reloadingMission)
        {
            MissionRetry();
        }
        if (returningToMenu)
        {
            ReturnToMenu();
        }

        if (MissionTimer > 5f)
        {
            MissionStart.SetActive(false);
        }

        if(Player != null)
        {
            points = KillCounter.Points;
            kills = KillCounter.Kills;
        }
        if (Player == null)
        {
            MissionEnd();
        }

        CheckForRemainingFighters();

        if (SetRetry == true)
        {
            MissionRetry();
        }
    }

    void UpdateUI()
    {
        if (playerAcHub.planeCam.camShaking == true)
        {
            TimeLeft.color = Color.red;
            KillCountUI.color = Color.red;
            PointCount.color = Color.red;
            currentWaveUI.color = Color.red;
            enemiesLeftText.color = Color.red;
        }
        else
        {
            TimeLeft.color = Color.white;
            KillCountUI.color = Color.white;
            PointCount.color = Color.white;
            currentWaveUI.color = Color.white;
            enemiesLeftText.color = Color.white;
        }
        TimeLeft.text = "Time: " + (int)MissionTimer;
        KillCountUI.text = "Destroyed: " + KillCounter.Kills;
        PointCount.text = "Points: " + (((int)MissionTimer * 10) + KillCounter.Points);
        currentWaveUI.text = "Wave " + currentWave;
        enemiesLeftText.text = enemyFighters.Count + " Enemies Left";
        BlackBG.fillClockwise = true;
		if(playerAcHub.fm.target != null)
		{
			markers.targetLockedHub = playerAcHub.fm.target.hub;
		}
		else
		{
			markers.targetLockedHub = null;
		}
		
        MapBoundaries();
		CountFPS();
		fpsCounterTxt.text = "FPS: " + currentFPS;
    }
	
	
	int totalFrames;
	int currentFPS;
	void CountFPS()
	{
		totalFrames++;
		if(totalFrames % 10 == 0)
		{
			currentFPS = (int)(1f / Time.unscaledDeltaTime);
			totalFrames = 0;
		}
	}
	

    IEnumerator StartNewWave()
    {
        print("Starting new wave");
        currentWave++;
        newWaveText.enabled = true;
        newWaveText.text = "Wave " + currentWave + " Inbound!";
        newWaveText.gameObject.GetComponent<AudioSource>().PlayOneShot(newWaveText.gameObject.GetComponent<AudioSource>().clip);

		if (startWithAllies && currentWave == 1)
        {
			SpawnAllies();
        }
	
		if(enableWaveBonus)
		{
			if(currentWave != 1 || (startWithBonus && currentWave == 1))
			{
				newWaveBonusText.enabled = true;
				newWaveBonusText.text = GiveReward();
			}
		}
		
        bonusWaveObjectiveText.enabled = true;
		bonusWaveObjectiveText.text = SpawnEnemies();

        yield return new WaitForSeconds(5f);
        newWaveText.enabled = false;
        newWaveBonusText.enabled = false;
        bonusWaveObjectiveText.enabled = false;
        startingwave = false;
        yield return null;
    }

    bool SetRetry = false;
    void MissionEnd()
    {
        if(bgm != null)
        {
            bgm.enabled = false; bgm = null;
        }
        BattleUI.SetActive(false);
        markers.gameObject.SetActive(false);
        MissionSuccess.SetActive(true);
        KillCounter.Points = 0;
        missionEnd = true;
        if (SetRetry == false)
        {
            timerToMenu += Time.deltaTime;
            if (timerToMenu > 1f)
            {
                CalculateFinalScore();
                Retry.SetActive(true);
                if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetAxis("FireCannon") != 0)
                {
                    SetRetry = true;
                    Retry.SetActive(false);
                    timerToMenu = 0f;
                }
            }
            if (timerToMenu >= 8f)
            {
                Retry.SetActive(false);
                Fade(false);
            }
            if (timerToMenu >= 10f)
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("SurvivalMenu");
            }
        }
    }

    void MissionRetry()
    {
        Fade(false);
        timerToMenu += Time.unscaledDeltaTime;
        if (timerToMenu >= 2f)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("SurvivalMission");
        }
    }

    int timeBonus;
    int finalScore;
    bool finalScoreCalculated;
    void CalculateFinalScore()
    {
        if (!finalScoreCalculated)
        {
            timeBonus = (int)MissionTimer * 10;
            mEnd_TimeBonus.text = "Time bonus: " + (int)MissionTimer + "s = " + timeBonus + " pts.";
            mEnd_PointScore.text = "Kills: " + points + " pts.";
            finalScore = points + timeBonus;
            mEnd_FinalScore.text = "Final Score: " + finalScore + " pts.";
            print(finalScore);
            SaveScore();
            finalScoreCalculated = true;
        }
    }

    void SaveScore()
    {
        int highestScore = PlayerPrefs.GetInt("Survival High Score");
        if(finalScore >  highestScore)
        {
            PlayerPrefs.SetInt("Survival High Score", finalScore);
        }

        int highestKills = PlayerPrefs.GetInt("Survival Highest Kills");
        if (kills > highestKills)
        {
            PlayerPrefs.SetInt("Survival Highest Kills", kills);
        }

        int highestRound = PlayerPrefs.GetInt("Survival Highest Round");
        if(currentWave > highestRound)
        {
            PlayerPrefs.SetInt("Survival Highest Round", currentWave);
        }

        int longestAlive = PlayerPrefs.GetInt("Survival Longest Alive");
        if((int)MissionTimer > longestAlive)
        {
            PlayerPrefs.SetInt("Survival Longest Alive", (int)MissionTimer);
        }
        PlayerPrefs.SetInt("Survival Mission Score", finalScore);
        RegisterKillStats();
        PlayerPrefs.Save();
    }

    void Fade(bool fadeInOrOut)
    {
        if (fadeInOrOut)
        {
            BlackBG.fillOrigin = 1;
            BlackBG.fillAmount -= Time.unscaledDeltaTime * 2f;
        }

        if (!fadeInOrOut)
        {
            BlackBG.fillOrigin = 2;
            BlackBG.fillAmount += Time.unscaledDeltaTime * 2f;
        }
    }

	private IEnumerator FadeOut()
	{
		float t = 0f;
		while (t < 1f)
		{
			t += Time.unscaledDeltaTime * 2f;
            BlackBG.fillOrigin = 2;
            BlackBG.fillAmount = Mathf.Lerp(0f, 1f, t);
			yield return null;
		}
		BlackBG.fillAmount = 1f;
	}

	private IEnumerator FadeIn()
	{
		float t = 0f;
		while (t < 1f)
		{
			t += Time.unscaledDeltaTime * 2f;
			BlackBG.fillOrigin = 1;
            BlackBG.fillAmount = Mathf.Lerp(1f, 0f, t);
			yield return null;
		}
        BlackBG.fillAmount = 0f;
		//fadeCanvasGroup.alpha = 0f;
	}

    bool startingwave = false;
    public List<AircraftHub> enemyFighters;
    void CheckForRemainingFighters()
    {
        for (int i = 0; i < enemyFighters.Count; i++)
        {
            if(enemyFighters[i] == null)
            {
                enemyFighters.RemoveAt(i);
                return;
            }
        }

        if(enemyFighters.Count == 0)
        {
            if(!startingwave)
            {
                StartCoroutine("StartNewWave");
                startingwave = true;
            }
        }
    }

	public bool isPlayerOutOfBounds;
    void MapBoundaries()
    {
        float giveWarningDistance = 12500f;
        float destroyDistance = 16000f;

        if(playerAcHub == null)
        {
            mapBoundaryWarning.enabled = false;
            mapBoundaryWarningAS.enabled = false;
            return;
        }

			float distance = Vector3.Distance(playerAcHub.transform.position, new Vector3(0f, playerAcHub.transform.position.y, 0f));
			if(distance > giveWarningDistance)
			{
				mapBoundaryWarning.enabled = true;
				mapBoundaryWarning.color = Color.white;
				mapBoundaryWarningAS.enabled = true;
				mapBoundaryWarningAS.clip = mapBoundaryWarningLight;
				if(mapBoundaryWarningAS.isPlaying == false && isPaused == false)
				{
					mapBoundaryWarningAS.Play();
				}
			}
	        else
			{
				mapBoundaryWarning.enabled = false;
				mapBoundaryWarningAS.enabled = false;
			}
			
			if (distance > destroyDistance)
			{
				if(!isPlayerOutOfBounds)
				{
					StartCoroutine(ResetPlayerOutOfBounds(playerAcHub, giveWarningDistance));
					isPlayerOutOfBounds = true;
					print("Reposition Algorythm called");
				}
			}
		
		foreach (AircraftHub aircraft in enemyFighters)
		{
			if (aircraft == null) continue;
	
			float distanceAI = Vector3.Distance(aircraft.transform.position, new Vector3(0f, aircraft.transform.position.y, 0f));
			if (distanceAI > destroyDistance)
			{
				TurnAircraftToCenter(aircraft, giveWarningDistance);
			}
		}
    }

	public void TurnAircraftToCenter(AircraftHub aircraft, float distanceToReposition)
	{
		Rigidbody rb = aircraft.rb;
		if (rb == null) return;

		// Reset physics
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;

		// Point toward world center
		Vector3 directionToCenter = (new Vector3(0f, aircraft.transform.position.y, 0f) - aircraft.transform.position).normalized;
		aircraft.transform.rotation = Quaternion.LookRotation(directionToCenter, Vector3.up);
		
		Vector3 directionFromCenter = (aircraft.transform.position - Vector3.zero).normalized;
		aircraft.transform.position = directionFromCenter * distanceToReposition;

		// Apply forward "spawn" speed
		float spawnSpeed = aircraft.fm.SpawnSpeed; // You can make this configurable per aircraft
		rb.velocity = aircraft.transform.forward * spawnSpeed;
		print("Aircraft Repositioned");
	}

	public IEnumerator ResetPlayerOutOfBounds(AircraftHub player, float distanceToReposition)
	{
		print("Starting reposition Algorythm");
        StartCoroutine(FadeOut());
		yield return new WaitForSeconds(0.5f);
		TurnAircraftToCenter(player, distanceToReposition);
		isPlayerOutOfBounds = false;
	    StartCoroutine(FadeIn());
		print("Finishing reposition Algorythm");
		yield return null;
	}

    void Pause()
    {
        PauseUI.SetActive(isPaused);
        bgm.gameObject.SetActive(!isPaused);
        AudioListener.pause = isPaused;
        
        if (Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                Time.timeScale = 0f;
                BattleUI.SetActive(false);
                isPaused = true;
            }
            else if (isPaused)
            {
                Time.timeScale = 1f;
                BattleUI.SetActive(true);
                bgm.UnPause();
                PlayerPrefs.SetFloat("BGM Volume", bgmVolume.value);
                isPaused = false;
            }
        }

        if (isPaused)
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton3))
            {
                buttonRetrying();
            }

            if (Input.GetKeyDown(KeyCode.Joystick1Button4))
            {
                buttonReturnToMenu();
            }
        }
    }

    public void UnPause()
    {
        isPaused = false;
        Time.timeScale = 1f;
        PlayerPrefs.SetFloat("BGM Volume", bgmVolume.value);
        bgm.UnPause();
    }

    public void buttonReturnToMenu()
    {
        UnPause();
        missionEnd = true;
        returningToMenu = true;
    }

    public void buttonRetrying()
    {
        print("Retrying");
        UnPause();
        missionEnd = true;
        reloadingMission = true;
    }

    void ReturnToMenu()
    {
        Fade(false);
        timerToMenu += Time.unscaledDeltaTime;
        if (timerToMenu >= 2f)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }
    }

    void RegisterKillStats()
    {
        PlayerPrefs.SetInt(aircraftName + " Total Kill Count", PlayerPrefs.GetInt(aircraftName + " Highest Kill Count") + kills);
        if(kills > PlayerPrefs.GetInt(aircraftName + " Highest Kill Count"))
        {
            PlayerPrefs.SetInt(aircraftName + " Highest Kill Count", kills);
        }

        PlayerPrefs.SetInt(aircraftName + " Total Time Alive", PlayerPrefs.GetInt(aircraftName + " Total Time Alive") + (int)MissionTimer);
        if (MissionTimer > PlayerPrefs.GetInt(aircraftName + " Highest Time Alive"))
        {
            PlayerPrefs.SetInt(aircraftName + " Highest Time Alive", (int)MissionTimer);
        }

        PlayerPrefs.SetInt(aircraftName + " Total Score", PlayerPrefs.GetInt(aircraftName + " Total Score") + finalScore);
        if (finalScore > PlayerPrefs.GetInt(aircraftName + " Highest Score"))
        {
            PlayerPrefs.SetInt(aircraftName + " Highest Score", finalScore);
        }

        PlayerPrefs.SetInt("General Total Score", PlayerPrefs.GetInt("General Total Score") + finalScore);
        PlayerPrefs.SetInt("General Total Kills", PlayerPrefs.GetInt("General Total Kills") + kills);
        PlayerPrefs.SetInt("General Total Fly Time", PlayerPrefs.GetInt("General Total Fly Time") + (int)MissionTimer);
    }
	
	
	void SpawnAllies()
	{
		switch (aircraftType)
		{
			case AircraftType.Trainer:
				waveSpawner.PropAlliedSpawnWave();
				break;
				
			case AircraftType.Prop:
				waveSpawner.PropAlliedSpawnWave();
				break;
			
			case AircraftType.JetTier1:
				waveSpawner.Jet1AlliedSpawnWave();
				break;
				
			case AircraftType.JetTier2:
				waveSpawner.Jet2AlliedSpawnWave();
				break;
			
		}
	}
	
	string SpawnEnemies()
	{
		string text = "";
		
		switch (aircraftType)
            {
                case AircraftType.Trainer:
                    if (currentWave % 5 == 0)
                    {
                        waveSpawner.PropBonusSpawnWave();
                        text = "Bonus Wave: Intercept the Enemy!";
                    }

                    else
                    {
						text = "";
                        if (currentWave <= 3)
                        {
                            waveSpawner.spawnEnemyWave(1);
                        }
                        else if (currentWave > 3 && currentWave <= 6)
                        {
                            waveSpawner.spawnEnemyWave(2);
                        }
                        else if (currentWave > 6 && currentWave <= 9)
                        {
                            waveSpawner.spawnEnemyWave(3);
                        }
                        else if (currentWave > 9 && currentWave <= 12)
                        {
                            waveSpawner.spawnEnemyWave(4);
                        }
                        else
                        {
                            waveSpawner.spawnEnemyWave(6);
                        }
                    }

                    break;

                case AircraftType.Prop:
                    if (currentWave % 5 == 0)
                    {
                        waveSpawner.PropBonusSpawnWave();
                        text = "Bonus Wave: Intercept the Enemy!";
                    }

                    else
                    {
						text = "";
                        if (currentWave <= 2)
                        {
                            waveSpawner.spawnEnemyWave(1);
                        }
                        else if (currentWave > 2 && currentWave < 5)
                        {
                            waveSpawner.spawnEnemyWave(2);
                        }
                        else if (currentWave > 5 && currentWave <= 8)
                        {
                            waveSpawner.spawnEnemyWave(3);
                        }
                        else if (currentWave > 8 && currentWave < 10)
                        {
                            waveSpawner.spawnEnemyWave(4);
                        }
                        else
                        {
                            waveSpawner.spawnEnemyWave(6);
                        }
                    }
                    break;

                case AircraftType.JetTier1:
                    if (currentWave % 5 == 0)
                    {
                        waveSpawner.JetTier1BonusSpawnWave();
                        text = "Bonus Wave: Intercept the Enemy!";
                    }

                    else
                    {
						text = "";
                        if (currentWave <= 3)
                        {
                            waveSpawner.spawnEnemyWave(1);
                        }
                        else if (currentWave > 3 && currentWave <= 6)
                        {
                            waveSpawner.spawnEnemyWave(2);
                        }
                        else if (currentWave > 6 && currentWave <= 9)
                        {
                            waveSpawner.spawnEnemyWave(3);
                        }
                        else if (currentWave > 9 && currentWave <= 12)
                        {
                            waveSpawner.spawnEnemyWave(4);
                        }
                        else
                        {
                            waveSpawner.spawnEnemyWave(6);
                        }
                    }

                    break;

                case AircraftType.JetTier2:
                    if (currentWave % 5 == 0)
                    {
                        waveSpawner.JetTier2BonusSpawnWave();
                        text = "Bonus Wave: Intercept the Enemy!";
                    }

                    else
                    {
						text = "";
                        if (currentWave <= 3)
                        {
                            waveSpawner.spawnEnemyWave(1);
                        }
                        else if (currentWave > 3 && currentWave <= 6)
                        {
                            waveSpawner.spawnEnemyWave(2);
                        }
                        else if (currentWave > 6 && currentWave <= 9)
                        {
                            waveSpawner.spawnEnemyWave(3);
                        }
                        else if (currentWave > 9 && currentWave <= 12)
                        {
                            waveSpawner.spawnEnemyWave(4);
                        }
                        else
                        {
                            waveSpawner.spawnEnemyWave(6);
                        }
                    }
                    break;
            }
			
		if (currentWave % 5 == 1 && currentWave != 1)
        {
            playerAcHub.hp.GrantExtraLife();
            text = "Continue granted!";
        }
		return text;
	}
	
	string GiveReward()
	{
		int randomReward = UnityEngine.Random.Range(1, 10);
		string text = "";
        switch (randomReward)
            {
                                case 1:
                                    playerAcHub.hp.HealMaxHP();
                                    text = "Reward: Full Health Restored!";
                                    break;
                                case 2:
                                    playerAcHub.hp.HealHPAmmount(UnityEngine.Random.Range(70, 150));
                                    text = "Reward: Health Restored!";
                                    break;
                                case 3:
                                    playerAcHub.hp.EnableInvulerability();
                                    text = "Reward: Invulnerable for 3 minutes!";
                                    break;
                                case 4:
                                    if (playerAcHub.irControl != null)
                                    {
                                        playerAcHub.irControl.MissileAmmo = playerAcHub.irControl.MaxMissileAmmo;
                                        text = "Reward: IR Missiles enabled";
                                    }
                                    else
                                    {
                                        playerAcHub.hp.HealMaxHP();
                                        text = "Reward: Full Health Restored!";
                                    }
                                    break;
                                case 5:
									if(playerAcHub.gunsControl.additionalGuns.Length != 0)
									{
										playerAcHub.gunsControl.EnableAG();
										text = "Reward: Gunpods enabled for 4 minutes!";
									}
									else
									{	
										playerAcHub.hp.EnableInvulerability();
										text = "Reward: Invulnerable for 3 minutes!";
									}
                                    break;
                                case 6:
                                    playerAcHub.rocketLauncherControl.EnableRockets();
                                    text = "Reward: Rocketpods enabled!";
                                    break;
                                case 7:
									SpawnAllies();
                                    text = "Reward: Allied Reinforcements!";
                                    break;
                                case 8:
                                    if (playerAcHub.radarMissileControl != null)
                                    {
                                        playerAcHub.radarMissileControl.MissileAmmo = playerAcHub.radarMissileControl.MaxMissileAmmo;
                                        text = "Reward: Radar Missiles enabled";
                                    }
                                    else
                                    {
                                        playerAcHub.hp.HealMaxHP();
                                        text = "Reward: Full Health Restored!";
                                    }
                                    break;
								case 9:
								    if (playerAcHub.sagControl != null)
                                    {
                                        playerAcHub.sagControl.MissileAmmo = playerAcHub.sagControl.MaxMissileAmmo;
                                        text = "Reward: SAG Missiles enabled";
                                    }
                                    else
                                    {
                                        playerAcHub.hp.HealMaxHP();
                                        text = "Reward: Full Health Restored!";
                                    }
									break;
                            }
		return text;
	}
}
