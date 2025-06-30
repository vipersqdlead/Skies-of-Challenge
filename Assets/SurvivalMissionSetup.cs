using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalMissionSetup : MonoBehaviour
{
    [Header("Mission Settings")]
    // Set in the same order as those in the Survival Menu settings!
    [SerializeField] GameObject[] mapPrefabs;
    [SerializeField] GameObject[] playerAircraftPrefabs;
    [SerializeField] GameObject player;
    AircraftHub hub;

    [SerializeField] SurvivalMissionStatus status;
    [SerializeField] WaveSpawner waveSpawner;

    private void Awake()
    {
        player = Instantiate(playerAircraftPrefabs[PlayerPrefs.GetInt("Survival Aircraft")], new Vector3(transform.position.x, 4500f, transform.position.z), transform.rotation);
        //status.aircraftName = playerAircraftPrefabs[PlayerPrefs.GetInt("Survival Aircraft")].name;
		status.aircraftName = playerAircraftPrefabs[PlayerPrefs.GetInt("Survival Aircraft")].GetComponent<AircraftHub>().aircraftName;
		
        if (PlayerPrefs.GetInt("Survival Aircraft") < 1)
        {
            status.aircraftType = SurvivalMissionStatus.AircraftType.Trainer;
        }
        else if (PlayerPrefs.GetInt("Survival Aircraft") >= 1 && PlayerPrefs.GetInt("Survival Aircraft") < 149)
        {
            status.aircraftType = SurvivalMissionStatus.AircraftType.Prop;
        }
        else if (PlayerPrefs.GetInt("Survival Aircraft") >= 148 && PlayerPrefs.GetInt("Survival Aircraft") < 211)
        {
            status.aircraftType = SurvivalMissionStatus.AircraftType.JetTier1;
        }
        else if (PlayerPrefs.GetInt("Survival Aircraft") >= 211)
        {
            status.aircraftType = SurvivalMissionStatus.AircraftType.JetTier2;
        }
        Instantiate(mapPrefabs[PlayerPrefs.GetInt("Survival Map")]);
        status.Player = player;
        hub = player.GetComponent<AircraftHub>();
        status.KillCounter = hub.killcounter;
        status.deathCam.Player = player;
        status.waveSpawner = waveSpawner;
        status.camListener = hub.planeCam.cameraTransform.GetComponent<AudioListener>();
        waveSpawner.player = hub.fm;
        UISetup();

        {
            if(PlayerPrefs.GetInt("Survival Enable Wave Bonus") == 1)
            {
                status.enableWaveBonus = true;
            }

            if (PlayerPrefs.GetInt("Survival Start With Allies") == 1)
            {
                status.startWithBonus = true;
            }

            if (PlayerPrefs.GetInt("Survival Start With Bonus") == 1)
            {
                status.startWithAllies = true;
            }

            if (PlayerPrefs.GetInt("Survival Always Reload Missiles") == 1)
            {
                status.missilesAllowed = true;
            }
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    [Header("UI Settings")]
    public TMP_Text UI_rpmLabel;
    public RectTransform UI_rpmArrow; // The arrow in the speedometer
    public TMP_Text UI_speedLabel; // The label that displays the speed;
    public RectTransform UI_speedArrow; // The arrow in the speedometer

    public RectTransform UI_smallAltArrow;
    public RectTransform UI_altBigArrow;

    public RectTransform UI_climbArrow;


    [SerializeField] TMP_Text UI_HP, UI_Missiles, UI_MissilesSAG, UI_MissilesRadar, UI_Rockets, UI_Combo, UI_Flares, UI_Target;
    [SerializeField] GameObject UI_Crosshair, UI_LeadMarker, UI_FPM, UI_MsslAcq, UI_MsslLock, UI_SARHAcq, UI_SARHLock, UI_Center;
    [SerializeField] AudioSource UI_stallWarning_SFX;
    [SerializeField] RawImage UI_healthIcon;
    [SerializeField] EnemyMarkers markers;
    [SerializeField] RadarMinimap minimap;
    void UISetup()
    {
        AircraftHub hub = player.GetComponent<AircraftHub>();
        hub.planeToUI.rpmLabel = UI_rpmLabel;
        hub.planeToUI.rpmArrow = UI_rpmArrow;
        hub.planeToUI.speedLabel = UI_speedLabel;
        hub.planeToUI.speedArrow = UI_speedArrow;
        hub.planeToUI.stallAlarm = UI_stallWarning_SFX;
        hub.planeToUI.smallAltArrow = UI_smallAltArrow;
        hub.planeToUI.altBigArrow = UI_altBigArrow;
        hub.planeToUI.climbArrow = UI_climbArrow;
        hub.planeToUI.Health = UI_HP;
        hub.planeToUI.healthIcon = UI_healthIcon;
        hub.planeToUI.killsCombo = UI_Combo;
        hub.planeToUI.Rockets = true;
        hub.planeToUI.Rockets1 = UI_Rockets;
        hub.planeToUI.MissilesIR = UI_Missiles;
        hub.planeToUI.MissilesSAG = UI_MissilesSAG;
        hub.planeToUI.MissilesRadar = UI_MissilesRadar;
        hub.planeToUI.flaresTxt = UI_Flares;
        hub.planeToUI.AcquireCircle = UI_MsslAcq;
        hub.planeToUI.LockCircle = UI_MsslLock;
        hub.planeToUI.SARHPovCircle = UI_SARHAcq;
        hub.planeToUI.SARHLockCircle = UI_SARHLock;
		//hub.planeToUI.currentTarget = UI_Target;


        hub.planeToUI.IRControl = hub.irControl;
        hub.planeToUI.RktControl = hub.rocketLauncherControl;
        if(hub.irControl != null)
        {
            hub.planeToUI.IRMissiles = true;
        }
        if(hub.radarMissileControl != null)
        {
            hub.planeToUI.SARHMissiles = true;
        }
        if (hub.sagControl != null)
        {
            hub.planeToUI.SAGMissiles = true;
        }
        if(hub.flareDispenser != null)
        {
            hub.planeToUI.flares = true;
        }

        hub.planeHUD.hudCenter = UI_Center.transform;
        hub.planeHUD.velocityMarker = UI_FPM.transform;
		markers.player = hub;
        markers.leadMarkerGO = UI_LeadMarker;

        waveSpawner.markers = markers;
        minimap.player = player.transform;
    }
}
