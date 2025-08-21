using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SurvivalSettings : MonoBehaviour
{
    public PlaneTypes playerPlane;
    public int playerPlaneint;
    public GameObject[] aircraftPrefabs;
    public AircraftHub selectedHub;
    public Sprite[] planesPNGs;
    public Image planeImg;
    public TMP_Text planeText;
    public MapTypes mapTypes;
    public Sprite[] mapPNGs;
    public Image mapImg;
    public TMP_Text mapText;
    public TMP_Text weaponText;
	public float totalAvailable;
    [SerializeField] TMP_Text speedText, powerText, climbText, weightText, agilityText, wingLoadingText, firePowerText, healthText, descriptionText, ratingText;
    public Transform modelDisplayPoint; // assign in Inspector
    private GameObject currentModel; // to keep track of the displayed model
    public float modelRotationSpeed = 20f;

    // Battle options
    public Toggle enableWaveBonusT, startWithBonusT, startWithAlliesT, alwaysReloadMissilesT, oldMatchmakerT, enableBlackoutRedoutT;
    public bool enableWaveBonus, startWithBonus, startWithAllies, alwaysReloadMissiles, oldMatchmaker, enableBlackoutRedout;

    [SerializeField] TMP_Text bestScore, bestRound, bestTime, lastScore, totalCompletion;
    [SerializeField] FadeInOut fadeEffect;
    [SerializeField] GameObject activeCanvas;
    [SerializeField] AudioSource bgm;

    [SerializeField] bool restrictSelection;
    [SerializeField] int maxSelection;

    bool loadMission;
    float timerToLoad;

    private void Start()
    {
        fadeEffect.ActivateFadeIn = true;

        if (PlayerPrefs.GetInt("Survival Enable Wave Bonus") == 1)
        {
            enableWaveBonus = true;
            enableWaveBonusT.isOn = true;
        }

        if (PlayerPrefs.GetInt("Survival Start With Allies") == 1)
        {
            startWithBonus = true;
            startWithBonusT.isOn = true;
        }

        if (PlayerPrefs.GetInt("Survival Start With Bonus") == 1)
        {
            startWithAllies = true;
            startWithAlliesT.isOn = true;
        }

        if (PlayerPrefs.GetInt("Survival Always Reload Missiles") == 1)
        {
            alwaysReloadMissiles = true;
            alwaysReloadMissilesT.isOn = true;
        }
		
		if (PlayerPrefs.GetInt("Survival Old Matchmaker") == 1)
        {
            oldMatchmaker = true;
            oldMatchmakerT.isOn = true;
        }
		
		if (PlayerPrefs.GetInt("Survival Enable Blackouts") == 1)
        {
            enableBlackoutRedout = true;
            enableBlackoutRedoutT.isOn = true;
        }

        bestScore.text = "Highest Score: " + PlayerPrefs.GetInt("Survival High Score") + " pts.";
        bestRound.text = "Highest Round: Wave " + PlayerPrefs.GetInt("Survival Highest Round");
        bestTime.text = "Longest Alive: " + PlayerPrefs.GetInt("Survival Longest Alive") + "s.";
        lastScore.text = "Last Score: " + PlayerPrefs.GetInt("Survival Mission Score") + " pts.";

        ChangeMap(PlayerPrefs.GetInt("Survival Map", 1));
        playerPlane = (PlaneTypes)PlayerPrefs.GetInt("Survival Aircraft", 5);
        selectedHub = aircraftPrefabs[(int)playerPlane].GetComponent<AircraftHub>();
        UpdateAircraftInfo();
		totalAvailable = CheckTotalAvailability();
		totalCompletion.text = "Completion: " + totalAvailable + "/" + aircraftPrefabs.Length;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F4))
        {
            PlayerPrefs.SetInt("Unlock All Aircraft", 1);
			totalAvailable = CheckTotalAvailability();
			totalCompletion.text = "Completion: " + totalAvailable + "/" + aircraftPrefabs.Length;
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            PlayerPrefs.SetInt("Unlock All Aircraft", 0);
			totalAvailable = CheckTotalAvailability();
			totalCompletion.text = "Completion: " + totalAvailable + "/" + aircraftPrefabs.Length;
        }
        if (loadMission == true)
        {
            bgm.volume -= Time.deltaTime;
            timerToLoad += Time.deltaTime;
        }
        if (currentModel != null)
        {
            currentModel.transform.Rotate(Vector3.up * modelRotationSpeed * Time.deltaTime);
        }

        enableWaveBonus = enableWaveBonusT.isOn;
        startWithBonus = startWithBonusT.isOn;
        startWithAllies = startWithAlliesT.isOn;
        alwaysReloadMissiles = alwaysReloadMissilesT.isOn;
		oldMatchmaker = oldMatchmakerT.isOn;
		enableBlackoutRedout = enableBlackoutRedoutT.isOn;
    }

    public void StartCoroutineChangeMenu(GameObject newMenu)
    {
        StartCoroutine(ChangeMenu(newMenu));
    }
    public IEnumerator ChangeMenu(GameObject newMenu)
    {
        fadeEffect.ActivateFadeOut = true;
        yield return new WaitForSeconds(0.5f);
        newMenu.SetActive(true); activeCanvas.SetActive(false);
        fadeEffect.ActivateFadeIn = true;
        yield return new WaitForSeconds(0.5f);
        activeCanvas = newMenu;
        yield return null;
    }

    public IEnumerator ToBattle()
    {
        fadeEffect.ActivateFadeOut = true;
        loadMission = true;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("SurvivalMission");
        yield return null;
    }

    public IEnumerator ToMainMenu()
    {
        fadeEffect.ActivateFadeOut = true;
        loadMission = true;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0);
        yield return null;
    }

    public void DisplayAircraft(AircraftHub aircraftHub)
    {
        if(aircraftHub.aircraftModel != null)
        {
	        if(currentModel == null) { currentModel = Instantiate(aircraftHub.aircraftModel, modelDisplayPoint.position, Quaternion.Euler(-30f, 0f, 0f), modelDisplayPoint); } 
		
	        else
	        {
                Quaternion currentRotation = currentModel.transform.rotation;
                Destroy(currentModel); // remove the old model
                                       // Instantiate the aircraft's model at the display point
                currentModel = Instantiate(aircraftHub.aircraftModel, modelDisplayPoint.position, currentRotation, modelDisplayPoint);
                // Optional: disable any flight scripts or extra colliders on this model
            }
        }
    }

    void UpdateAircraftInfo()
    {
        selectedHub.Awake();
        selectedHub.CalculateSomeStats();

        planeText.text = selectedHub.aircraftName;
		ratingText.text = "Rating: " + selectedHub.hp.pointsWorth + " pts.";

        speedText.text = "Speed: " + selectedHub.speed_maxSpeed + " km/h";
        weightText.text = "Weight: " + selectedHub.rb.mass + " Kg";
        if (selectedHub.isJet == false)
        {
			if(selectedHub.engineNumber == 1) 
			{ 
				if(selectedHub.engineControl.isAfterburningEngine)
				{
					powerText.text = "Power: " + selectedHub.power_maxPower + " PS Mil; " + selectedHub.power_WepPower + " PS WEP"; 
				}
				else
				{
					powerText.text = "Power: " + selectedHub.power_maxPower + " PS"; 
				}
			}
			else 
			{ 
				if(selectedHub.engineControl.isAfterburningEngine)
				{
					powerText.text = "Power: x" + selectedHub.engineNumber + " " + selectedHub.power_maxPower + " PS Mil; " + selectedHub.power_WepPower + " PS WEP"; 
				}
				else
				{
					powerText.text = "Power: x" + selectedHub.engineNumber + " " + selectedHub.power_maxPower + " PS"; 
				}
			}

			climbText.text = "Power-to-Weight: " + selectedHub.powerToWeight + " Kg/PS";
		}
        else
		{
			if(selectedHub.engineNumber == 1) 
			{ 
				if(selectedHub.engineControl.isAfterburningEngine)
				{
					powerText.text = "Thrust: " + selectedHub.power_maxPower + " Kgf Dry; " + selectedHub.power_WepPower + " Kgf Wet"; 
				}
				else
				{
					powerText.text = "Thrust: " + selectedHub.power_maxPower + " Kgf Dry"; 
				}
			}
			else 
			{ 
				if(selectedHub.engineControl.isAfterburningEngine)
				{
					powerText.text = "Thrust: x" + selectedHub.engineNumber + " " + selectedHub.power_maxPower + " Kgf Dry; " + selectedHub.power_WepPower + " Kgf Wet"; 
				}
				else
				{
					powerText.text = "Thrust: x" + selectedHub.engineNumber + " " + selectedHub.power_maxPower + " Kgf Dry"; 
				}
			}
			
			climbText.text = "Thrust-to-Weight: " + selectedHub.powerToWeight + " Kgf/Kg";
        }
	
	    wingLoadingText.text = "Wing Loading: " + selectedHub.agility_wingLoading + " kg/m2";
        agilityText.text = "Turn Rate: " + selectedHub.agility_maxTurnDegS + " deg/s at " + selectedHub.agility_maxTurnSpeed + " km/h";
        firePowerText.text = "Burst Mass: " + selectedHub.attack_totalBurstMass + " Kg/s";
        healthText.text = "Health: " + selectedHub.health_maxHP + " HP / " + selectedHub.defense_def + " DEF";
        descriptionText.text = selectedHub.aircraftDescription;

        DisplayAircraft(selectedHub);
    }

    public void ChangePlayerPlanesRight()
    {
        if (CheckPlaneAvailability(playerPlane + 1) && (int)playerPlane < aircraftPrefabs.Length - 1)
        {
            playerPlane++;
        }
        else if ((int)playerPlane > aircraftPrefabs.Length - 1)
        {
            playerPlane = 0;

            if (!CheckPlaneAvailability(0))
            {
                ChangePlayerPlanesRight();
            }
        }
        else
        {
            playerPlane++;
            ChangePlayerPlanesRight();
        }
        selectedHub = aircraftPrefabs[(int)playerPlane].GetComponent<AircraftHub>();
        // planeImg.sprite = planesPNGs[(int)playerPlane];
        UpdateAircraftInfo();
    }

    public void ChangePlayerPlanesLeft()
    {
        if (CheckPlaneAvailability(playerPlane - 1) && (int)playerPlane > 0)
        {
            playerPlane--;
        }
        else if ((int)playerPlane <= 0)
        {
            int _int = Enum.GetValues(typeof(PlaneTypes)).Length - 1;
            playerPlane = (PlaneTypes)_int;
            if (!CheckPlaneAvailability((PlaneTypes)_int))
            {
                ChangePlayerPlanesLeft();
            }
        }
        else
        {
            playerPlane--;
            ChangePlayerPlanesLeft();
        }
        selectedHub = aircraftPrefabs[(int)playerPlane].GetComponent<AircraftHub>();
        // planeText.text = playerPlane.ToString();
        UpdateAircraftInfo();
    }
	
	float CheckTotalAvailability()
	{	
		totalAvailable = 0;
		for(int i = 0; i < aircraftPrefabs.Length; i++)
		{
			if(CheckPlaneAvailability((PlaneTypes)i))
			{
				totalAvailable++;
			}
		}
		
		return totalAvailable;
	}

    bool CheckPlaneAvailability(PlaneTypes type)
    {

        if (PlayerPrefs.GetInt("Unlock All Aircraft") == 1)
        {
            return true;
        }

        switch (type)
        {
			case PlaneTypes.Pucara66:
			    if (PlayerPrefs.GetInt("IA 58D Pucará Total Kill Count") >= 150)
                { return true; }
                else
                { return false; }
			case PlaneTypes.PucaraC:
			    if (PlayerPrefs.GetInt("IA 58D Pucará Highest Kill Count") >= 35)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Emil:
			    if (PlayerPrefs.GetInt("Bf 109T Trager Total Kill Count") >= 3)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Fried:
                if (PlayerPrefs.GetInt("Bf 109E Emil Total Kill Count") >= 10)
                { return true; }
                else
                { return false; }
            case PlaneTypes.TragerLate:
                if (PlayerPrefs.GetInt("Bf 109T Trager Total Kill Count") >= 50 && PlayerPrefs.GetInt("Bf 109T Trager Highest Kill Count") >= 20)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Krahe:
                if (PlayerPrefs.GetInt("Bf 109T Trager (Late) Highest Kill Count") >= 25 && PlayerPrefs.GetInt("General Total Score") > 90000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.HispanoCobra:
                if (PlayerPrefs.GetInt("P-39N Airacobra Total Kill Count") >= 30)
                { return true; }
                else
                { return false; }
            case PlaneTypes.AiracobraL:
                if (PlayerPrefs.GetInt("P-39N Airacobra Highest Score") >= 30000 || PlayerPrefs.GetInt("P-400 Hispanocobra Highest Score") >= 30000)
                { return true; }
                else
                { return false; }
			case PlaneTypes.MiG3_Shvak:
                if (PlayerPrefs.GetInt("MiG-3 Highest Score") >= 60000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.VargA:
                if (PlayerPrefs.GetInt("J22B Varg Highest Time Alive") >= 300)
                { return true; }
                else
                { return false; }
            case PlaneTypes.ReisenIII:
                if (PlayerPrefs.GetInt("A6M5 Reisen Total Time Alive") >= 300)
                { return true; }
                else
                { return false; }
            case PlaneTypes.ReisenKai:
                if (PlayerPrefs.GetInt("A6M5 Reisen Total Time Alive") >= 600)
                { return true; }
                else
                { return false; }
            case PlaneTypes.HayabusaIII:
                if (PlayerPrefs.GetInt("A6M8 Reisen-Kai Total Time Alive") + PlayerPrefs.GetInt("A6M5 Reisen Total Time Alive") >= 1200 && (PlayerPrefs.GetInt("Ki-61-II Hien Total Score") + PlayerPrefs.GetInt("Ki-61-I Hien Total Score") + PlayerPrefs.GetInt("Ki-61-III Hien Total Score") >= 50000))
                { return true; }
                else
                { return false; }
            case PlaneTypes.NightHellcat:
                if (PlayerPrefs.GetInt("F6F-5 Hellcat Total Time Alive") >= 450 && PlayerPrefs.GetInt("Hellcat Total Kill Count") >= 30)
                { return true; }
                else
                { return false; }
            case PlaneTypes.SuperHellcat:
                if (PlayerPrefs.GetInt("F6F-5 Hellcat Total Kill Count") + PlayerPrefs.GetInt("F6F-5N Night Hellcat Total Kill Count") >= 80)
                { return true; }
                else
                { return false; }
            case PlaneTypes.CorsairB:
                if (PlayerPrefs.GetInt("F4U-4 Corsair Total Score") > 100000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.SuperCorsair:
                if (PlayerPrefs.GetInt("F4U-4 Corsair Total Kill Count") + PlayerPrefs.GetInt("F4U-4B Corsair Total Kill Count") >= 100)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Truehawk:
                if (PlayerPrefs.GetInt("General Total Fly Time") > 10000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.SuperWarhawk:
                if (PlayerPrefs.GetInt("P-40M Warhawk Total Score") + PlayerPrefs.GetInt("P-40M Warhawk (Mod) Total Score") > 100000 && PlayerPrefs.GetInt("General Total Fly Time") > 10000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Apache:
                if (PlayerPrefs.GetInt("P-51C Mustang Total Kill Count") >= 25)
                { return true; }
                else
                { return false; }
            case PlaneTypes.MustangA:
                if (PlayerPrefs.GetInt("P-51C Mustang Highest Score") >= 30000 || PlayerPrefs.GetInt("A-36A Apache Highest Score") >= 30000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.MustangD:
                if (PlayerPrefs.GetInt("P-51C Mustang Total Time Alive") + PlayerPrefs.GetInt("P-51A Mustang Total Time Alive") + PlayerPrefs.GetInt("A-36A Apache Total Time Alive") >= 1200)
                { return true; }
                else
                { return false; }
            case PlaneTypes.SkayatekaMustang:
                if (PlayerPrefs.GetInt("P-51A Mustang Highest Time Alive") >= 900 || PlayerPrefs.GetInt("P-51D Mustang Highest Time Alive") >= 900)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Mustang_II:
                if (PlayerPrefs.GetInt("A-36A Apache Total Kill Count") >= 80)
                { return true; }
                else
                { return false; }
            case PlaneTypes.MustangH:
                if (PlayerPrefs.GetInt("P-51D Mustang Total Score") + PlayerPrefs.GetInt("P-51A Mustang Total Score") + PlayerPrefs.GetInt("P-51C Mustang Total Score") + PlayerPrefs.GetInt("A-36A Apache Total Score") + PlayerPrefs.GetInt("P-51D Skayateka Mustang Total Score") > 500000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.YakovlevT:
                if (PlayerPrefs.GetInt("Yak-9U Total Time Alive") >= 300)
                { return true; }
                else
                { return false; }
            case PlaneTypes.YakovlevK:
                if (PlayerPrefs.GetInt("Yak-9T Highest Score") >= 30000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.YakovlevU:
                if (PlayerPrefs.GetInt("General Total Kills") >= 1000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.YakovlevJ:
                if (PlayerPrefs.GetInt("General Total Kills") >= 2000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.AntonA:
                if (PlayerPrefs.GetInt("Fw 190A Anton (Late) Total Kill Count") >= 30)
                { return true; }
                else
                { return false; }
            case PlaneTypes.AntonHotrod:
                if (PlayerPrefs.GetInt("General Total Fly Time") >= 10000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.HellWhisper:
                if (PlayerPrefs.GetInt("Fw 190A Anton (Late) Total Kill Count") + PlayerPrefs.GetInt("Fw 190A Anton (Early) Total Kill Count") >= 200 && PlayerPrefs.GetInt("Hell's Whisper Times Killed") >= 5)
                { return true; }
                else
                { return false; }
            case PlaneTypes.ZerstorerH:
                if (PlayerPrefs.GetInt("Bf 110G Zerstorer Highest Time Alive") >= 600)
                { return true; }
                else
                { return false; }
            case PlaneTypes.DinahKai:
                if (PlayerPrefs.GetInt("Ki-46-III Dinah Highest Score") >= 40000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Raiko:
                if (PlayerPrefs.GetInt("Ki-46-III Dinah Total Kill Count") + PlayerPrefs.GetInt("Ki-46-III Dinah-Kai Total Kill Count") >= 100)
                { return true; }
                else
                { return false; }
            case PlaneTypes.LightningK:
                if (PlayerPrefs.GetInt("P-38L Lightning Total Time Alive") >= 1000 && PlayerPrefs.GetInt("P-38L Lightning Highest Kill Count") >= 38)
                { return true; }
                else
                { return false; }
			case PlaneTypes.LightningI:
                if (PlayerPrefs.GetInt("P-38L Lightning Total Kill Count") >= 100 || PlayerPrefs.GetInt("P-40M Warhawk Total Kill Count") >= 100)
                { return true; }
                else
                { return false; }
            case PlaneTypes.SpitfireII:
                if (PlayerPrefs.GetInt("Survival High Score") >= 30000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.SpitfireXXIV:
                if (PlayerPrefs.GetInt("Survival Highest Kills") >= 70)
                { return true; }
                else
                { return false; }
            case PlaneTypes.BearcatB:
                if (PlayerPrefs.GetInt("F8F-1 Bearcat Total Kill Count") >= 80)
                { return true; }
                else
                { return false; }
            case PlaneTypes.FalkeB:
                if (PlayerPrefs.GetInt("Fw 187D Falke Total Time Alive") >= 1500)
                { return true; }
                else
                { return false; }
            case PlaneTypes.FalkeG:
                if (PlayerPrefs.GetInt("Fw 187B Falke Highest Kill Count") >= 70 || PlayerPrefs.GetInt("Fw 187D Falke Highest Kill Count") >= 70)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Moskito:
                if (PlayerPrefs.GetInt("Fw 187B Falke Highest Time Alive") >= 1500 || PlayerPrefs.GetInt("Fw 187G Falke Highest Time Alive") >= 1500 || PlayerPrefs.GetInt("Fw 187D Falke Highest Time Alive") >= 1500)
                { return true; }
                else
                { return false; }
			case PlaneTypes.ThunderboltM:
				if (PlayerPrefs.GetInt("P-47D Thunderbolt Total Kills") >= 75)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Superbolt:
                if (PlayerPrefs.GetInt("General Total Score") >= 1250000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Ruina:
                if (PlayerPrefs.GetInt("Ruina Times Killed") >= 5)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Sismo:
                if (PlayerPrefs.GetInt("Sismo Times Killed") >= 10)
                { return true; }
                else
                { return false; }
            case PlaneTypes.HienI:
                if (PlayerPrefs.GetInt("Ki-61-II Hien Highest Time Alive") >= 900)
                { return true; }
                else
                { return false; }
            case PlaneTypes.HienIIIOtsu:
                if ((PlayerPrefs.GetInt("Ki-61-II Hien Total Time Alive") + (PlayerPrefs.GetInt("Ki-61-I Hien Total Time Alive")) >= 1500) && PlayerPrefs.GetInt("Ki-84-Ia Hayate Total Time Alive") + PlayerPrefs.GetInt("Ki-84b Hayate Total Time Alive") + PlayerPrefs.GetInt("Ki-84c Hayate Total Time Alive") + PlayerPrefs.GetInt("Ki-84-II Hayate Total Time Alive") > 1500)
                { return true; }
                else
                { return false; }
            case PlaneTypes.TenraiKai:
                if (PlayerPrefs.GetInt("J5N1 Tenrai Highest Time Alive") >= 1000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.TenraiSuperKai:
                if (PlayerPrefs.GetInt("J5N1 Tenrai Highest Score") >= 55000 || PlayerPrefs.GetInt("J5N2 Tenrai-Kai Highest Kill Count") >= 80)
                { return true; }
                else
                { return false; }
            case PlaneTypes.LynxD:
                if (PlayerPrefs.GetInt("Survival Highest Round") >= 16)
                { return true; }
                else
                { return false; }
            case PlaneTypes.WhiteWolf:
                if (PlayerPrefs.GetInt("White Wolf Times Killed") >= 3)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Shoki:
                if (PlayerPrefs.GetInt("J2M5 Raiden Highest Time Alive") >= 600)
                { return true; }
                else
                { return false; }
            case PlaneTypes.CentauroA:
                if (PlayerPrefs.GetInt("G.55 Serie 1 Highest Kill Count") >= 50)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Centauro56:
                if (PlayerPrefs.GetInt("G.55 Serie 1 Highest Time Alive") >= 600)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Airacomet:
                if ((PlayerPrefs.GetInt("P-63A Kingcobra Total Kill Count") + PlayerPrefs.GetInt("P-39N Airacobra Total Kill Count")) >= 100)
                { return true; }
                else
                { return false; }
            case PlaneTypes.DolchA:
                if (PlayerPrefs.GetInt("Me 309C Dolch Total Score") + PlayerPrefs.GetInt("Bf 109K Kurfurst Total Score") >= 120000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.DolchD:
                if (PlayerPrefs.GetInt("Survival Longest Alive") >= 600)
                { return true; }
                else
                { return false; }
            case PlaneTypes.DolchE:
                if (PlayerPrefs.GetInt("Survival Longest Alive") >= 1200)
                { return true; }
                else
                { return false; }
            case PlaneTypes.XinyiYongnian:
                if (PlayerPrefs.GetInt("Xinyi Yongnian Times Killed") >= 3)
                { return true; }
                else
                { return false; }
            case PlaneTypes.MacchiC:
                if (PlayerPrefs.GetInt("C.205 S3 Veltro Highest Score") >= 50000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.MoonbatB:
                if (PlayerPrefs.GetInt("P-67A Moonbat Total Score") >= 70000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.MansyuuKai:
                if (PlayerPrefs.GetInt("Ki-98-I Mansyuu Total Time Alive") >= 1000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Senden:
                if (PlayerPrefs.GetInt("Ki-98-I Mansyuu Total Score") + PlayerPrefs.GetInt("Ki-98-I Mansyuu-Kai Total Score") >= 150000 && (PlayerPrefs.GetInt("A7M2 Reppu Total Kill Count") + PlayerPrefs.GetInt("A7M3 Reppu Total Kill Count") + PlayerPrefs.GetInt("A7M3-J Reppu-Kai Total Kill Count") >= 100))
                { return true; }
                else
                { return false; }
            case PlaneTypes.WhiteFootFox:
                if (PlayerPrefs.GetInt("Bf 109K Kurfurst Total Kill Count") >= 158 && PlayerPrefs.GetInt("White-Foot Fox Times Killed") >= 3)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Yak3_VK107:
                if (PlayerPrefs.GetInt("Yak-3 Highest Kill Count") >= 70)
                { return true; }
                else
                { return false; }
            case PlaneTypes.DoraLate:
                if (PlayerPrefs.GetInt("General Total Kills") >= 2500)
                { return true; }
                else
                { return false; }
            case PlaneTypes.KogarashiI:
                if (PlayerPrefs.GetInt("Ki-100-II Kogarashi Highest Score") >= 30000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.KogarashiOtsu:
                if (PlayerPrefs.GetInt("Ki-100-II Kogarashi Highest Kill Count") >= 60 || PlayerPrefs.GetInt("Ki-100-I Kogarashi Highest Kill Count") >= 60 && PlayerPrefs.GetInt("Ki-84-Ic Hayate Total Time Alive") + PlayerPrefs.GetInt("Ki-84-Ib Hayate Total Time Alive") + PlayerPrefs.GetInt("Ki-84-Ic Hayate Total Time Alive") + PlayerPrefs.GetInt("Ki-84-II Hayate Total Time Alive") >= 1800)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Lavochkin5:
                if (PlayerPrefs.GetInt("La-7 Total Time Alive") >= 1200)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Skyraider:
                if (PlayerPrefs.GetInt("Skyraider Times Killed") >= 30)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Skyshark:
                if (PlayerPrefs.GetInt("Skyraider Total Kills") >= 150)
                { return true; }
                else
                { return false; }
            case PlaneTypes.TankC:
                if (PlayerPrefs.GetInt("Ta 152H Tank Total Score") >= 120000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Kyofu:
                if (PlayerPrefs.GetInt("General Total Fly Time") >= 25000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.ShidenKaiIV:
                if (PlayerPrefs.GetInt("N1K2-J Shiden-Kai Highest Score") >= 50000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Pyorremyrsky:
                if (PlayerPrefs.GetInt("VL Pyorremyrsky (Early) Total Kill Count") >= 80)
                { return true; }
                else
                { return false; }
			case PlaneTypes.PyorremyrskyLate:
                if (PlayerPrefs.GetInt("VL Pyorremyrsky (Late) Highest Kill Count") >= 50)
                { return true; }
                else
                { return false; }
            case PlaneTypes.MathiasFleisher:
                if (PlayerPrefs.GetInt("Mathias Fleisher Times Killed") >= 3 && (PlayerPrefs.GetInt("VL Pyorremyrsky (Early) Highest Kill Count") >= 75 || PlayerPrefs.GetInt("VL Pyorremyrsky Highest Kill Count") >= 75 || PlayerPrefs.GetInt("VL Pyorremyrsky (Late) Highest Kill Count") >= 75))
                { return true; }
                else
                { return false; }
            case PlaneTypes.Ghost:
                if ((PlayerPrefs.GetInt("Ghost Times Killed") >= 20 && PlayerPrefs.GetInt("General Total Fly Time") > 10000000) || PlayerPrefs.GetInt("Survival High Score") > 500000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Assassin:
                if (PlayerPrefs.GetInt("Assassin Times Killed") >= 10 && PlayerPrefs.GetInt("General Total Kills") >= 10000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.ReppuC:
                if (PlayerPrefs.GetInt("A7M2 Reppu Highest Kill Count") >= 75)
                { return true; }
                else
                { return false; }
            case PlaneTypes.ReppuKai:
                if (PlayerPrefs.GetInt("A7M2 Reppu Total Score") + PlayerPrefs.GetInt("A7M3 Reppu Total Score") >= 200000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Crusader:
                if (PlayerPrefs.GetInt("Crusader Nagao Times Killed") >= 3 && PlayerPrefs.GetInt("Blue Angel Times Killed") >= 3)
                { return true; }
                else
                { return false; }
            case PlaneTypes.HayateOtsu:
                if (PlayerPrefs.GetInt("Ki-84-Ia Hayate Total Time Alive") >= 1000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.HayateHei:
                if (PlayerPrefs.GetInt("Ki-84-Ia Hayate Highest Score") >= 50000 || PlayerPrefs.GetInt("Ki-84-Ib Hayate Highest Score") >= 50000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.HayateII:
                if (PlayerPrefs.GetInt("Ki-84-Ia Hayate Total Time Alive") + PlayerPrefs.GetInt("Ki-84-Ib Hayate Total Time Alive") + PlayerPrefs.GetInt("Ki-84-Ic Hayate Total Time Alive") >= 3000 && PlayerPrefs.GetInt("Blue Angel Times Killed") >= 3 && PlayerPrefs.GetInt("Survival Highest Round") >= 15)
                { return true; }
                else
                { return false; }
            case PlaneTypes.BlueAngel:
                if (PlayerPrefs.GetInt("Blue Angel Times Killed") >= 10)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Wyvern:
                if (PlayerPrefs.GetInt("Skyshark Total Kills") >= 150)
                { return true; }
                else
                { return false; }
            case PlaneTypes.ShindenLate:
                if (PlayerPrefs.GetInt("General Total Score") >= 2500000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Reaper:
                if (PlayerPrefs.GetInt("The Reaper Times Killed") >= 10)
                { return true; }
                else
                { return false; }
            case PlaneTypes.SeiranEarly:
                if (PlayerPrefs.GetInt("D5A2 Seiran Total Score") >= 70000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.SeiranKai:
                if (PlayerPrefs.GetInt("D5A2 Seiran Total Score") + PlayerPrefs.GetInt("D5A1 Seiran Total Score") >= 200000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Cheetah:
                if (PlayerPrefs.GetInt("F7F-1 Tigercat Total Time Alive") >= 1200)
                { return true; }
                else
                { return false; }
            case PlaneTypes.TigercatC:
                if (PlayerPrefs.GetInt("F7F-1 Tigercat Total Kill Count") + PlayerPrefs.GetInt("P-65A Cheetah Total Kill Count") >= 120)
                { return true; }
                else
                { return false; }
            case PlaneTypes.ZwillingK:
                if (PlayerPrefs.GetInt("General Total Kills") >= 4000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.TakaKai:
                if (PlayerPrefs.GetInt("Survival Highest Kills") >= 150)
                { return true; }
                else
                { return false; }
            case PlaneTypes.SchwalbeIb:
                if (PlayerPrefs.GetInt("Me 262A Schwalbe Total Kill Count") >= 50)
                { return true; }
                else
                { return false; }
            case PlaneTypes.SchwalbeIc:
                if (PlayerPrefs.GetInt("Me 262A Schwalbe Highest Kill Count") >= 75)
                { return true; }
                else
                { return false; }
            case PlaneTypes.BlitzB:
                if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
                { return true; }
                else
                { return false; }
            case PlaneTypes.KikkaOtsu:
                if (PlayerPrefs.GetInt("A6M5 Reisen Total Score") + PlayerPrefs.GetInt("A6M8 Reisen-Kai Total Score") + PlayerPrefs.GetInt("J2M5 Raiden Total Score") + PlayerPrefs.GetInt("J5N1 Tenrai Total Score") + PlayerPrefs.GetInt("J5N2 Tenrai-Kai Total Score") + PlayerPrefs.GetInt("D5A1 Seiran Total Score") + PlayerPrefs.GetInt("J5N3 Tenrai-Super Kai Total Score") + PlayerPrefs.GetInt("N1K4-J Shiden-Kai Total Score") + PlayerPrefs.GetInt("A7M2 Reppu Total Score") + PlayerPrefs.GetInt("A7M3 Reppu Total Score") + PlayerPrefs.GetInt("A7M3-J Reppu-Kai Total Score") + PlayerPrefs.GetInt("D5A2 Seiran Total Score") + PlayerPrefs.GetInt("D5A1 Seiran Total Score") + PlayerPrefs.GetInt("D5A3 Seiran-Kai Total Score") + PlayerPrefs.GetInt("J9N1 Kikka Total Score") >= 1500000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.ShootingStarC:
                if (PlayerPrefs.GetInt("F-80A Shooting Star Total Kill Count") >= 70)
                { return true; }
                else
                { return false; }
            case PlaneTypes.ShootingStarT:
                if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Starfire:
                if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
                { return true; }
                else
                { return false; }
            case PlaneTypes.FargoLate:
                if (PlayerPrefs.GetInt("MiG-9FS Fargo Highest Time Alive") >= 900f)
                { return true; }
                else
                { return false; }
            case PlaneTypes.SeaVenom:
                if (PlayerPrefs.GetInt("Vampire FB 52 Fargo Total Time Alive") >= 1500f)
                { return true; }
                else
                { return false; }
            //case PlaneTypes.SeaVixen:
            //    if (PlayerPrefs.GetInt("Sea Venom FAW 20 Fargo Total Time Alive") >= 1500f)
            //    { return true; }
            //    else
            //    { return false; }
		    case PlaneTypes.HatsutakaII:
                if (PlayerPrefs.GetInt("Ki-215-I Hatsutaka Highest Score") >= 60000)
                { return true; }
                else
                { return false; }
		    case PlaneTypes.HatsutakaT:
                if ((PlayerPrefs.GetInt("Ki-215-I Hatsutaka Total Score") + PlayerPrefs.GetInt("Ki-215-II Hatsutaka Total Score")) >= 25000)
                { return true; }
                else
                { return false; }
			case PlaneTypes.PulquiLate:
                if (PlayerPrefs.GetInt("IAe 33 Pulqui II (Early) Total Score") >= 70000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.GinaPreserie:
                if (PlayerPrefs.GetInt("G.91 R/4 Gina Highest Kills") >= 30)
                { return true; }
                else
                { return false; }
            case PlaneTypes.KaryuKai:
                if (PlayerPrefs.GetInt("Campaign Clear") >= 1)
                { return true; }
                else
                { return false; }
            case PlaneTypes.DrachentoterB:
                if (PlayerPrefs.GetInt("Survival Highest Round") >= 15)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Gunval:
                if (PlayerPrefs.GetInt("F-86F Sabre Total Time Alive") >= 1500)
                { return true; }
                else
                { return false; }
            case PlaneTypes.FJ2Fury:
                if (PlayerPrefs.GetInt("F-86F-2 Gunval Sabre Highest Kill Count") >= 35)
                { return true; }
                else
                { return false; }
            case PlaneTypes.CACSabre:
                if (PlayerPrefs.GetInt("F-86F Sabre Total Score") + PlayerPrefs.GetInt("F-86F-2 Gunval Sabre Total Score") + PlayerPrefs.GetInt("FJ-3 Fury Total Score") >= 650000)
                { return true; }
                else
                { return false; }
            //case PlaneTypes.WhiteDaze:
            //    if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
            //    { return true; }
            //    else
            //    { return false; }
            case PlaneTypes.Fresco:
                if (PlayerPrefs.GetInt("Survival Highest Round") >= 20)
                { return true; }
                else
                { return false; }
            case PlaneTypes.TunnanD:
                if (PlayerPrefs.GetInt("Survival Highest Round") >= 20)
                { return true; }
                else
                { return false; }
            //case PlaneTypes.SchwalbeIIIb:
            //    if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
            //    { return true; }
            //    else
            //    { return false; }
            case PlaneTypes.Cougar:
                if (PlayerPrefs.GetInt("Survival Highest Round") >= 15)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Jaguar:
                if (PlayerPrefs.GetInt("Survival Highest Round") >= 20)
                { return true; }
                else
                { return false; }
            case PlaneTypes.SkyhawkM:
                if (PlayerPrefs.GetInt("A-4E Skyhawk Highest Kill Count") >= 25)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Ayit:
                if (PlayerPrefs.GetInt("A-4M Skyhawk Highest Time Alive") >= 900)
                { return true; }
                else
                { return false; }
            case PlaneTypes.FightingHawk:
                if (PlayerPrefs.GetInt("A-4M Skyhawk Highest Kill Count") >= 35)
                { return true; }
                else
                { return false; }
			case PlaneTypes.HunterF6:
                if (PlayerPrefs.GetInt("Hunter FGA.9 Total Kill Count") >= 75)
                { return true; }
                else
                { return false; }
			case PlaneTypes.HunterF58:
                if (PlayerPrefs.GetInt("Hunter FGA.9 Total Time Alive") + PlayerPrefs.GetInt("Hunter F.6 Total Time Alive") >= 3600f)
                { return true; }
                else
                { return false; }
            //case PlaneTypes.EnteB:
            //    if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
            //    { return true; }
            //    else
            //    { return false; }
            //case PlaneTypes.HarrierC:
            //    if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
            //    { return true; }
            //    else
            //    { return false; }
		    case PlaneTypes.StarfighterG:
                if (PlayerPrefs.GetInt("F-104A Starfighter Highest Round") >= 10)
                { return true; }
                else
                { return false; }
			case PlaneTypes.StarfighterASA:
                if (PlayerPrefs.GetInt("F-104A Starfighter Total Kill Count") + PlayerPrefs.GetInt("F-104G Starfighter Total Kill Count") >= 250)
                { return true; }
                else
                { return false; }
            case PlaneTypes.CrusaderE:
                if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
                { return true; }
                else
                { return false; }
            case PlaneTypes.KazeLate:
                if (PlayerPrefs.GetInt("General Total Kills") >= 2000)
                { return true; }
                else
                { return false; }
			case PlaneTypes.DrakenA:
                if (PlayerPrefs.GetInt("J35D Draken Total Kill Count") >= 85)
                { return true; }
                else
                { return false; }
            case PlaneTypes.DrakenJ:
                if (PlayerPrefs.GetInt("J35D Draken Highest Kill Count") >= 25)
                { return true; }
                else
                { return false; }
            //case PlaneTypes.Cipher:
            //    if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
            //    { return true; }
            //    else
            //    { return false; }
            case PlaneTypes.MirageV:
                if (PlayerPrefs.GetInt("Mirage IIIC Total Time Alive") + PlayerPrefs.GetInt("Mirage IIIE Total Time Alive") >= 1200)
                { return true; }
                else
                { return false; }
			case PlaneTypes.MiragePantera:
                if (PlayerPrefs.GetInt("Mirage 5F Total Score") >= 250000)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Kfir:
                if (PlayerPrefs.GetInt("Mirage 5F Total Kill Count") >= 100 && PlayerPrefs.GetInt("A-4N Ayit Total Kill Count") >= 150 && PlayerPrefs.GetInt("F-4E Phantom II Total Kill Count") >= 200)
                { return true; }
                else
                { return false; }
            /* case PlaneTypes.WhiteDazeII:
                if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
                { return true; }
                else
                { return false; } */
			case PlaneTypes.FishbedSMT:
			    if (PlayerPrefs.GetInt("MiG-21F Fishbed Total Kill Count") >= 150)
                { return true; }
                else
                { return false; }
			case PlaneTypes.FishbedBis:
			    if (PlayerPrefs.GetInt("MiG-21SMT Fishbed Highest Score") >= 60000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Bison:
                if (PlayerPrefs.GetInt("MiG-21bis Fishbed Highest Round") >= 20)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Fishcan:
                if (PlayerPrefs.GetInt("MiG-21F Fishbed Total Score") + PlayerPrefs.GetInt("MiG-21SMT Fishbed Total Score") + PlayerPrefs.GetInt("MiG-21bis Fishbed Total Score") >= 5000000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.FreedomFighter:
                if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
                { return true; }
                else
                { return false; }
			case PlaneTypes.TigerIIG:
                if (PlayerPrefs.GetInt("F-5E Tiger II Total Kill Count") >= 100)
                { return true; }
                else
                { return false; }
			case PlaneTypes.TigerIII:
                if (PlayerPrefs.GetInt("F-5E Tiger II Highest Kill Count") >= 50 && PlayerPrefs.GetInt("F-5G Tiger II Highest Kill Count") >= 50)
                { return true; }
                else
                { return false; }
            case PlaneTypes.PhantomS:
                if (PlayerPrefs.GetInt("F-4E Phantom II Highest Score") >= 30000)
                { return true; }
                else
                { return false; }
			case PlaneTypes.PhantomFGR:
                if (PlayerPrefs.GetInt("F-4S Phantom II Highest Score") >= 50000 && PlayerPrefs.GetInt("Tornado ADV Total Kill Count") >= 125)
                { return true; }
                else
                { return false; }
            case PlaneTypes.YuureiEarly:
                if (PlayerPrefs.GetInt("F-4E Phantom II Highest Score") >= 30000 && (PlayerPrefs.GetInt("T-2A Kaze Total Kill Count") + PlayerPrefs.GetInt("T-2A Kaze (Late) Total Kill Count") >= 30))
                { return true; }
                else
                { return false; }
            case PlaneTypes.Yuurei:
                if (PlayerPrefs.GetInt("F-4E Phantom II Total Kill Count") >= 100 && PlayerPrefs.GetInt("A9M5 Yuurei (Early) Total Kill Count") >= 100)
                { return true; }
                else
                { return false; }
            case PlaneTypes.PhantomICE:    
				if (PlayerPrefs.GetInt("F-4E Phantom II Highest Kill Count") >= 100 && PlayerPrefs.GetInt("F/A-18C Hornet Total Kill Count") >= 100)
                { return true; }
                else
                { return false; }
            case PlaneTypes.Fengren:
                if (PlayerPrefs.GetInt("MiG-23MLD Flogger Highest Round") >= 20)
                { return true; }
                else
                { return false; }
			case PlaneTypes.FloggerJ:
                if (PlayerPrefs.GetInt("MiG-23MLD Flogger Total Kill Count") >= 50)
                { return true; }
                else
                { return false; }
            //case PlaneTypes.ViggenAJS:
            //    if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
            //    { return true; }
            //    else
            //    { return false; }
            case PlaneTypes.SuperDeltaDart:
                if (PlayerPrefs.GetInt("F-106A Delta Dart Highest Score") >= 30000)
                { return true; }
                else
                { return false; }
            case PlaneTypes.MirageG8:
                if (PlayerPrefs.GetInt("Mirage F1 Highest Time Alive") >= 600)
                { return true; }
                else
                { return false; }
			case PlaneTypes.TomcatB:
                if (PlayerPrefs.GetInt("F-14A Tomcat Highest Score") >= 30000)
                { return true; }
                else
                { return false; }
			case PlaneTypes.PersianCat:
                if (PlayerPrefs.GetInt("F-14A Tomcat Highest Score") >= 50000)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Samurai:
                if (PlayerPrefs.GetInt("F-14A Tomcat Total Kill Count") + PlayerPrefs.GetInt("A9M5 Yuurei Total Kill Count") >= 250)
                { return true; }
                else
                { return false; }
			case PlaneTypes.TomcatD:
                if (PlayerPrefs.GetInt("F-14A Tomcat Highest Kill Count") + PlayerPrefs.GetInt("F-14B Tomcat Highest Kill Count") >= 300)
                { return true; }
                else
                { return false; }
			case PlaneTypes.FulcrumSMT:
                if (PlayerPrefs.GetInt("MiG-29 Fulcrum (9-13) Highest Kill Count") >= 104)
                { return true; }
                else
                { return false; }
			case PlaneTypes.FalconADF:
                if (PlayerPrefs.GetInt("F-16A-5 Fighting Falcon Total Kill Count") >= 50)
                { return true; }
                else
                { return false; }
			case PlaneTypes.FalconC50:
                if (PlayerPrefs.GetInt("F-16A-5 Fighting Falcon Total Kill Count") >= 100 && PlayerPrefs.GetInt("F-16A-15 ADF Fighting Falcon Total Kill Count") >= 100)
                { return true; }
                else
                { return false; }
			case PlaneTypes.FalconD50Plus:
                if (PlayerPrefs.GetInt("F-16C-50 Fighting Falcon Total Time Alive") >= 2400)
                { return true; }
                else
                { return false; }
			case PlaneTypes.FalconV:
                if (PlayerPrefs.GetInt("F-16C-50 Fighting Falcon Highest Score") >= 100000)
                { return true; }
                else
                { return false; }
			case PlaneTypes.FalconZero:
                if (PlayerPrefs.GetInt("F-16C-50 Fighting Falcon Highest Time Alive") >= 900 && PlayerPrefs.GetInt("T-2A Kaze (Late) Total Kill Count") >= 100)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Flanker27SM:
                if (PlayerPrefs.GetInt("Su-27S Flanker Total Time Alive") >= 900)
                { return true; }
                else
                { return false; }
			case PlaneTypes.FlankerJ11:
                if ((PlayerPrefs.GetInt("J-9 Fengren Total Kill Count") + PlayerPrefs.GetInt("J-7E Fishcan Total Kill Count") + PlayerPrefs.GetInt("Su-27SM Total Kill Count")) >= 300)
                { return true; }
                else
                { return false; }
			/*
			case PlaneTypes.Flanker30SM:
                if (PlayerPrefs.GetInt("Su-27S Flanker Total Time Alive") >= 900)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Flanker30MKI:
                if (PlayerPrefs.GetInt("Su-27S Flanker Total Time Alive") >= 900)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Flanker33Early:
                if (PlayerPrefs.GetInt("Su-27S Flanker Total Time Alive") >= 900)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Flanker33Late:
                if (PlayerPrefs.GetInt("Su-27S Flanker Total Time Alive") >= 900)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Flanker34:
                if (PlayerPrefs.GetInt("Su-27S Flanker Total Time Alive") >= 900)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Flanker35:
                if (PlayerPrefs.GetInt("Su-27S Flanker Total Time Alive") >= 900)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Flanker37:
                if (PlayerPrefs.GetInt("Su-27S Flanker Total Time Alive") + PlayerPrefs.GetInt("Su-27SM Flanker Total Time Alive") + PlayerPrefs.GetInt("Su-30SM Flanker Total Time Alive") + PlayerPrefs.GetInt("Su-33 Flanker (Early) Total Time Alive") + PlayerPrefs.GetInt("Su-33 Flanker (Late) Total Time Alive") + PlayerPrefs.GetInt("Su-34 Fullback Total Time Alive") + PlayerPrefs.GetInt("Su-35S Flanker Total Time Alive") >= 21000)
                { return true; }
                else
                { return false; }
			case PlaneTypes.FlankerJ15:
                if (PlayerPrefs.GetInt("J-11B Flanker Highest Kill Count") >= 100)
                { return true; }
                else
                { return false; }
			case PlaneTypes.FlankerJ16:
                if (PlayerPrefs.GetInt("J-15 Flanker Highest Kill Count") >= 100 && PlayerPrefs.GetInt("Su-33 Flanker (Early) Highest Time Alive") >= 600)
                { return true; }
                else
                { return false; }
			*/
			case PlaneTypes.EagleC:
                if (PlayerPrefs.GetInt("F-15A Eagle Highest Kill Count") >= 104)
                { return true; }
                else
                { return false; }
			case PlaneTypes.EagleE:
				if (PlayerPrefs.GetInt("F-15A Eagle Highest Time Alive") >= 1200 || PlayerPrefs.GetInt("F-15A Eagle Highest Time Alive") >= 1200)
                { return true; }
                else
                { return false; }
			case PlaneTypes.EagleEX:
				if (PlayerPrefs.GetInt("F-15E Strike Eagle Total Kill Count")  >= 500)
                { return true; }
                else
                { return false; }
			case PlaneTypes.EagleJ:
				if (PlayerPrefs.GetInt("F-15C Eagle Total Kill Count") + PlayerPrefs.GetInt("F-2A Zero Total Kill Count") >= 1000)
                { return true; }
                else
                { return false; }
			case PlaneTypes.EagleSE:
				if (PlayerPrefs.GetInt("F-15E Strike Eagle Highest Kill Count") >= 104)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Mirage20005:
                if (PlayerPrefs.GetInt("Mirage 2000C Highest Time Alive") >= 1200)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Mirage4000C:
                if (PlayerPrefs.GetInt("Mirage 4000A Total Kill Count") >= 150)
                { return true; }
                else
                { return false; }
			case PlaneTypes.HornetC:
                if (PlayerPrefs.GetInt("F/A-18A Hornet Total Time Alive") >= 3000)
                { return true; }
                else
                { return false; }
			case PlaneTypes.Raptor:
				if(totalAvailable >= aircraftPrefabs.Length - 2)
				{ return true; }
				else
				{ return false; }
			case PlaneTypes.RaptorLate:
				if(totalAvailable >= aircraftPrefabs.Length - 2 && PlayerPrefs.GetInt("F-22A Raptor Total Kill Count")  >= 500)
				{ return true; }
				else
				{ return false; }
        }
        return true;
    }

    public void ChangeMap(int toMap)
    {
        mapTypes = (MapTypes)toMap;
        mapImg.sprite = mapPNGs[(int)mapTypes];
        mapText.text = mapTypes.ToString();
    }

    public void ChangeMap()
    {
        mapTypes++;
        if((int)mapTypes > Enum.GetValues(typeof(MapTypes)).Length - 1)
        {
            mapTypes = 0;
        }
        mapImg.sprite = mapPNGs[(int)mapTypes];
        mapText.text = mapTypes.ToString();
    }

    public enum PlaneTypes
    {
        Fuji,
        Pucara,
		Pucara66,
		PucaraC,
        Trojan,
        Pilatus,
        Tucano,
        TragerEarly,
        Emil,
        Fried,
        TragerLate,
        Krahe,
        AiracobraN, 
        HispanoCobra,
        AiracobraL,
		MiG3,
		MiG3_Shvak,
        VargB,
        VargA,
        ReisenV,
		ReisenIII,
        ReisenKai,
        HayabusaIII,
        Hellcat,
        NightHellcat,
        SuperHellcat,
        CorsairA,
        CorsairB,
		//CorsairMkIV,
        SuperCorsair,
        Warhawk,
		//Hawk,
		//HawkMod,
		//WarhawkN,
        Truehawk,
        SuperWarhawk,
		// Ascender,
        MustangC,
        Apache,
        MustangA,
        MustangD,
        SkayatekaMustang,
		Mustang_II,
        MustangH,
        Yakovkev,
        YakovlevT,
        YakovlevK,
        YakovlevU,
        YakovlevJ,
        Anton,
        AntonA,
        AntonHotrod,
        HellWhisper,
        Zerstorer,
        ZerstorerH,
        Dinah,
        DinahKai,
        Raiko,
        LightningL,
        LightningK,
		// ChainLightning,
		LightningI,
        SpitfireVIII,
        SpitfireII,
        SpitfireXXIV,
		// MB157,
        BearcatA,
        BearcatB,
        FalkeD,
        FalkeB,
        FalkeG,
        Moskito,
        Thunderbolt,
		ThunderboltM,
        Superbolt,
        Ruina,
        Sismo,
        HienII,
        HienI,
        HienIIIOtsu,
        Tenrai,
        TenraiKai,
        TenraiSuperKai,
        LynxA,
        LynxD,
        WhiteWolf,
        Hornisse,
        Polikarpov,
        Raiden,
        Shoki,
        Centauro,
        CentauroA,
		Centauro56,
        Kingcobra,
        Airacomet,
        Dolch,
        DolchA,
        DolchD,
        DolchE,
		DolchV,
        XinyiYongnian,
        Macchi,
        MacchiC,
        Moonbat,
        MoonbatB,
		Chimere,
        Tvestjarten,
		// TvestjartenRB,
        Mansyuu,
        MansyuuKai,
        Senden,
        Kurfurst,
        WhiteFootFox,
		Yak3,
		Yak3_VK107,
        Dora,
        DoraLate,
        Kogarashi,
        KogarashiI,
        KogarashiOtsu,
        Lavochkin7,
		Lavochkin5,
        Tempest,
		Skyraider,
		Skyshark,
        Tank,
        TankC,
        ShidenKai,
        Kyofu,
        ShidenKaiIV,
        PyorremyrskyEarly,
        Pyorremyrsky,
		PyorremyrskyLate,
        MathiasFleisher,
        Ghost,
        Seafire,
        Assassin,
        Reppu,
        ReppuC,
        ReppuKai,
        Crusader_Nagao,
        Hayate,
        HayateOtsu,
        HayateHei,
        HayateII,
        BlueAngel,
        SeaFury,
		Wyvern,
        Shinden,
        ShindenLate,
        Reaper,
        Seiran,
        SeiranEarly,
        SeiranKai,
        Tigercat,
        Cheetah,
        TigercatC,
        Pfeil,
		Narval,
        TwinMustang,
        Zwilling,
        ZwillingK,
        Taka,
        TakaKai,
		//MosquitoFB,
        SeaHornet,
		// Volksjager,
        SchwalbeI,
        SchwalbeIb,
        SchwalbeIc,
        Blitz,
        BlitzB,
        Kikka,
		Kikka_20mm,
        KikkaOtsu,
		Su9,
		//Su11,
        Fargo,
        FargoLate,
        ShootingStar,
        ShootingStarC,
        ShootingStarT,
        Starfire,
        Thunderjet, 
		// Banshee,
        Meteor_Early,
		//Meteor_Late,
		KeiunKai_V1,
		// KeiunKai_V2
		// KeiunKai_Prod
        Vampire,
        SeaVenom,
        //SeaVixen,
		//SeaVixen_Mod,
        Shinden_Kai,
        Hatsutaka,
		HatsutakaII,
        HatsutakaT,
        Ente,
        //EnteB,
        Karyu,
        KaryuKai,
		//Scorpion,
        SchwalbeII,
        Huckebein,
		Pulqui,
		PulquiLate,
        Thunderstrike,
        Gina,
		// GinaY,
        GinaPreserie,
        Mystere,
        Drachentoter,
        DrachentoterB,
        Sabre,
        Gunval,
        FJ2Fury,
        CACSabre,
        Fagot,
		Fagot_Early,
        Fresco,
        //WhiteDaze,
		// Cutlass,
        Tunnan,
		TunnanF,
        TunnanD,
        SchwalbeIII,
        //SchwalbeIIIb,
        Panther,
        Cougar,
        Jaguar,
        Skyhawk,
        SkyhawkM,
        Ayit,
        FightingHawk,
        Lansen,
        HunterFGA9,
		HunterF6,
		HunterF58,
        SuperEtendard,
		//SuperEtendardModernise
        VautourIS,
		//VautourFR,
		//Sabredog,
        Kyokkou,
		// SeaDemon,
        Harrier,
		//HarrierGR3,
		//SeaHarrier,
        //HarrierC,
        Farmer,
        StarfighterA,
		StarfighterG,
		StarfighterASA,
        Skyray,
        DeltaDagger,
        Tiger,
		// Lightning_F6
        Thunderchief,
        Fantan,
        Crusader,
        CrusaderE,
		//Jaguar,
		//JaguarM,
        Kaze,
		KazeEarly,
        KazeLate,
        Draken,
		DrakenA,
        DrakenJ,
        //Cipher,
        MirageIIIC,
		MirageIIIE,
		MirageIIIS,
		MirageV,
		MiragePantera,
        Kfir,
        Fitter,
        //WhiteDazeII,
        Fishbed,
		FishbedSMT,
		FishbedBis,
        Bison,
		Fishcan,
        TigerII,
        FreedomFighter,
		TigerIIG,
		TigerIII,
		//Tora,
        Phantom,
		PhantomS,
		PhantomFGR,
		YuureiEarly,
        Yuurei,
		PhantomICE,
        Flogger,
        Fengren,
		FloggerJ,
		Flogger98,
        DeltaDart,
        SuperDeltaDart,
        Viggen,
        //ViggenAJS,
        MirageF1,
        MirageG8,
	    //ThunderboltII,
	    //Frogfoot,
        Tomcat,
		TomcatB,
        Samurai,
	    PersianCat,
		TomcatD,
        //TornadoGR3,
	    //TornadoADV,
		//TornadoF3,
        //Fencer,
        //AardvarkA,
		//AardvarkF,
        FulcrumA,
		FulcrumSMT,
		//FulcrumK,
		//SuperFulcrum,
        FalconA5,
		FalconADF,
		FalconC50,
		FalconD50Plus,
		//FalconF,
		FalconV,
		FalconZero,
		//FalconZeroKai,
        Flanker27,
		Flanker27SM,
		FlankerJ11,
		Flanker33Early,
		//Flanker33Late,
		//FlankerJ16,
		//Flanker30SM,
		//Flanker30MKI,
		//Flanker34,
		//Flanker35,
		//FlankerJ15,
		//Flanker37,
		//J10A,
		//J10C,
        EagleA,
		EagleC,
		EagleE,
		EagleEX,
		EagleJ,
		EagleSE,
        Mirage2000C,
		Mirage20005,
	    Mirage4000A,
		Mirage4000C,
		HornetA,
		HornetC,
        //SuperHornet,
        Eurofighter,
        RafaleC,
		//RafaleM,
        //GripenA,
		//GripenC,
		//GripenE,
		// BlackWidowII,
		// Felon,
		J20A,
		//J20A_Late,
		//LightningIIa,
		//LightningIIc
		Raptor,
		RaptorLate
    }

    public enum MapTypes
    {
        Harbor,
        Factories,
        SunnyDesert,
        GreenValleyDay,
        PureMountain,
        HiddenAirfield,
        ChaoticSea,
        DeathCanyon,
        GreenValleyNight,
        HarborNight
    }

    public void SaveChanges()
    {
        PlayerPrefs.SetInt("Survival Aircraft", (int)playerPlane);
        PlayerPrefs.SetInt("Survival Map", (int)mapTypes);
        PlayerPrefs.SetInt("Survival Enable Wave Bonus", enableWaveBonus ? 1 : 0);
        PlayerPrefs.SetInt("Survival Start With Allies", startWithAllies ? 1  :0);
        PlayerPrefs.SetInt("Survival Start With Bonus", startWithBonus ? 1 : 0);
        PlayerPrefs.SetInt("Survival Always Reload Missiles", alwaysReloadMissiles ? 1 : 0);
		PlayerPrefs.SetInt("Survival Old Matchmaker", oldMatchmaker ? 1 : 0);
		PlayerPrefs.SetInt("Survival Enable Blackouts", enableBlackoutRedout ? 1 : 0);
    }

    public void SaveChanges(int manualPlaneType)
    {
        PlayerPrefs.SetInt("Survival Aircraft", manualPlaneType);
        PlayerPrefs.SetInt("Survival Map", (int)mapTypes);
    }

    public void StartBattle()
    {
        SaveChanges();
        StartCoroutine("ToBattle");
    }

    public void ReturnToMenu()
    {
        SaveChanges();
        StartCoroutine("ToMainMenu");
    }

}
