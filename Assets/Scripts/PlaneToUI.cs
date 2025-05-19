using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class PlaneToUI : MonoBehaviour
{
    [SerializeField] HealthPoints playerHp;
    public IRMissileControl IRControl;
    [SerializeField] SAGMissileControl SAGControl;
    [SerializeField] RadarMissileControl SARHControl;
    public RocketLauncherControl RktControl;
    [SerializeField] BombControl bmbControl;
    [SerializeField] LaserGuidedBombController lgbController;

    public GameObject UI;
    public TMP_Text Health, MissilesIR, MissilesSAG, MissilesRadar, Rockets1, Bombs, flaresTxt, guidedBombs, killsCombo;
    public RawImage healthIcon;
    public GameObject AcquireCircle, LockCircle, SARHPovCircle, SARHLockCircle;

    public bool IRMissiles, SAGMissiles, SARHMissiles, Rockets, bomb, guidedBomb, flares;

    Vector3 screenCenter;



    [Header("General Settings")]
    [SerializeField] AircraftHub hub;
    [SerializeField] Rigidbody planeRb;
    [SerializeField] FlightModel playerControls;
    [SerializeField] EngineControl engineControl;
    [SerializeField] Transform planeTransform;

    [Header("Speedometer")]
    public float maxSpeed = 900f; // The maximum speed of the target ** IN KM/H **
    float minSpeedArrowAngle = 125;
    float maxSpeedArrowAngle = -125;
    public TMP_Text speedLabel; // The label that displays the speed;
    public RectTransform speedArrow; // The arrow in the speedometer
    private float speed = 0.0f;
    public AudioSource stallAlarm;

    [Header("Tachometer")]
    public TMP_Text rpmLabel;
    private float rpm;
    public RectTransform rpmArrow; // The arrow in the speedometer

    [Header("Altitude")]
    public RectTransform smallAltArrow;
    public RectTransform altBigArrow;

    [Header("Climb")]
    public float climbRate;
    public float clampedClimbRate;
    public RectTransform climbArrow;
    private const float maxClimbRate = 30f;
    private const float minClimbRate = -30f;






    // Start is called before the first frame update
    void Awake()
    {
        hub = GetComponent<AircraftHub>();
        engineControl = hub.engineControl;
        screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
    }

    // Update is called once per frame
    void Update()
    {

        ShowDials();
        ShowFlares();
        InfraredMissiles();
        RadarMissiles();
        SemiAutomaticMissiles();
        ShowHealth();
        Rocket();
        //Bomb();
        //GuidedBomb();
        CountKillsCombo();
    }

    void ShowHealth()
    {
        if(playerHp.extraLives == 0)
        {
            Health.text = (int)playerHp.hpPercent + "%";
        }
        else
        {
            Health.text = (int)playerHp.hpPercent + "% (" + playerHp.extraLives + ")";
        }

        if(playerHp.invulnerable == true)
        {
            Health.color = Color.green;
        }
        else
        {
            Color newColor = Color.Lerp(Color.red, Color.white, (Mathf.Clamp(playerHp.hpPercent, 0, 100)) / 100f);
            healthIcon.color = newColor;
            if (playerHp.hpPercent >= 40 && hub.planeCam.camShaking == false)
            {
                Health.color = Color.white;
            }
            if (playerHp.hpPercent < 40 && hub.planeCam.camShaking == false)
            {
                Health.color = Color.yellow;
            }
            if (playerHp.hpPercent < 25 || hub.planeCam.camShaking == true)
            {
                Health.color = Color.red;
            }
        }
    }

    void ShowSpeed()
    {
        // 3.6f to convert in kilometers
        // ** The speed must be clamped by the car controller **
        speed = hub.fm.currentSpeed;

        if (speedLabel != null)
        {
            if (playerControls.IAS_Speed < playerControls.stallSpeed + 30f || playerControls.stalling == true|| hub.planeCam.camShaking == true || playerControls.currentSpeed > playerControls.neverExceedSpeed - 60f)
            {
                speedLabel.color = Color.red;
            }
            if (playerControls.stalling)
            {
                stallAlarm.enabled = true;
            }
            else
            {
                speedLabel.color = Color.white;
                stallAlarm.enabled = false;
            }
            speedLabel.text = ((int)speed).ToString();

            // stallAlarm.enabled = (-playerControls.maxAngleOfAttack / 2 - 1f) < playerControls.angleOfAttack && playerControls.angleOfAttack < playerControls.maxAngleOfAttack + 3f;

        }
        if (speedArrow != null)
            speedArrow.localEulerAngles =
                new Vector3(0, 0, Mathf.Lerp(minSpeedArrowAngle, maxSpeedArrowAngle, speed / maxSpeed));
    }

    void ShowPower()
    {
        rpm = hub.engineControl.ThrottleInput;

        if (rpmLabel != null)
        {
            if (engineControl.afterBurner)
            {
                rpmLabel.text = "AB";
                rpmLabel.color = Color.red;
            }
            else
            {
                rpmLabel.text = ((int)(rpm * 100)).ToString();
                rpmLabel.color = Color.white;
            }
        }
        
        if (rpmArrow != null)
            rpmArrow.localEulerAngles =
                new Vector3(0, 0, Mathf.Lerp(minSpeedArrowAngle, maxSpeedArrowAngle, rpm / 1));
    }

    void ShowAltitude()
    {
        float alt = hub.rb.position.y / 1000;
        float hundredMeters = hub.rb.position.y % 1000f;

        // Calculate arrow rotation (360 degrees for full rotation)
        // For the hundred-meter arrow: 100 meters -> 36 degrees (360 degrees / 10 increments)
        float hundredMeterRotation = (hundredMeters / 1000f) * 360f;

        // For the kilometer arrow: 1 kilometer -> 36 degrees (10 kilometers -> full rotation)
        float kilometerRotation = (alt / 10) * 360f;

        // Rotate the arrows
        smallAltArrow.localRotation = Quaternion.Euler(0, 0, -hundredMeterRotation);
        altBigArrow.localRotation = Quaternion.Euler(0, 0, -kilometerRotation);
    }

    void ShowClimbRate()
    {
        climbRate = hub.rb.velocity.y;
        // Clamp the climb rate to the range [-30, 30]
        clampedClimbRate = Mathf.Clamp(climbRate, minClimbRate, maxClimbRate);

        // Calculate rotation based on the climb rate
        // Neutral (0 m/s) is at 9 o'clock (180 degrees)
        // Max positive climb rate (+30 m/s) is at 3 o'clock (0 degrees)
        // Max negative climb rate (-30 m/s) is at 6 o'clock (270 degrees)

        float rotation = 90f - ((clampedClimbRate / maxClimbRate) * 180f); // Map climb rate to angle range [-90, 90]

        // Rotate the arrow
        climbArrow.localRotation = Quaternion.Euler(0, 0, rotation);
    }

    void ShowDials()
    {
        ShowSpeed();
        ShowPower();
        ShowAltitude();
        ShowClimbRate();
    }

    void ShowFlares()
    {
        if(flares == true)
        {
            flaresTxt.text = "Flares    " + hub.flareDispenser.flareCount;
            
            if (hub.flareDispenser.flareCount == 0 || hub.planeCam.camShaking == true)
            {
                flaresTxt.color = Color.red;
            }
            else
            {

                flaresTxt.color = Color.white;
            }
        }
        else
        {
            if(flaresTxt != null)
            {
                flaresTxt.enabled = false;
            }
        }
    }

    void RadarMissiles()
    {
        if (SARHMissiles == true)
        {
            if (SARHControl.isPlayer)
            {
                MissilesRadar.text = ">" + SARHControl.weaponName + "    " + SARHControl.MissileAmmo;
            }
            else
            {
                MissilesRadar.text = SARHControl.weaponName + "    " + SARHControl.MissileAmmo;
            }
            SARHPovCircle.SetActive(false);
            SARHLockCircle.SetActive(false);

            if(hub.planeCam.camShaking == false)
            {
                if (SARHControl.MissileAmmo == 0 || SARHControl.Guiding)
                {
                    MissilesRadar.color = Color.red;
                }
                else
                {
                    MissilesRadar.color = Color.white;
                }
            }

            else if(hub.planeCam.camShaking == true)
            {
                MissilesRadar.color = Color.red;
            }
            if (SARHControl.Acquiring)
            {
                SARHPovCircle.SetActive(true);
                if (SARHControl.Target)
                {
                    SARHLockCircle.SetActive(true);
                    SARHLockCircle.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, SARHControl.Target.transform.TransformPoint(Vector3.zero));
                    //AcquireCircle.SetActive(false);
                }
                else
                {
                    SARHLockCircle.SetActive(false);
                }
            }
        }
        else
        {
            MissilesRadar.enabled = false;
        }
    }

    void InfraredMissiles()
    {
        if (IRMissiles == true)
        {
            if (IRControl != null)
            {
                AcquireCircle.transform.localScale = new Vector3(IRControl.missileOuterFoV / 5, IRControl.missileOuterFoV / 5, 1);
                if (!IRControl.Acquiring && !IRControl.Locked)
                {
                    AcquireCircle.SetActive(false);
                    LockCircle.SetActive(false);
                }
                if (IRControl.Acquiring || IRControl.Locked)
                {
		    if(IRControl.isCagedSeeker == false) { AcquireCircle.SetActive(true); }
                    LockCircle.SetActive(true);
                }
                if (IRControl.Locked)
                {
                    if(IRControl.Target != null)
                    {
                        Vector3 targetScreenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, IRControl.Target.transform.position); 
                        LockCircle.transform.position = Vector3.Lerp(LockCircle.transform.position, targetScreenPosition, Time.deltaTime * 20f);
                        LockCircle.GetComponent<UnityEngine.UI.Image>().color = Color.red;
                        //LockCircle.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, IRControl.Target.transform.TransformPoint(Vector3.zero));
                    }
                }
                if(!IRControl.Locked)
                {
                    LockCircle.transform.position = Vector3.Lerp(LockCircle.transform.position, AcquireCircle.transform.position, Time.deltaTime * 20f);
                    LockCircle.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                }
                if (IRControl.isPlayer)
                {
                    MissilesIR.text = ">" + IRControl.weaponName + "    " + IRControl.MissileAmmo;
                }
                else
                {
                    MissilesIR.text = IRControl.weaponName + "    " + IRControl.MissileAmmo;
                }

                if (hub.planeCam.camShaking == false)
                {
                    if (IRControl.MissileAmmo == 0)
                    {
                        MissilesIR.color = Color.red;
                    }
                    else
                    {
                        MissilesIR.color = Color.white;
                    }
                }
                else
                {
                    MissilesIR.color = Color.red;
                }
            }
        }
        else
        {
            MissilesIR.enabled = false;
            AcquireCircle.SetActive(false);
            LockCircle.SetActive(false);
        }
    }

    void SemiAutomaticMissiles()
    {
        if (SAGMissiles == true)
        {
            if (SAGControl.isPlayer)
            {
                MissilesSAG.text = ">" + SAGControl.weaponName + "    " + SAGControl.MissileAmmo;
            }
            else
            {
                MissilesSAG.text = SAGControl.weaponName + "    " + SAGControl.MissileAmmo;
            }
            if (hub.planeCam.camShaking == false)
            {
                if (SAGControl.MissileAmmo == 0)
                {
                    MissilesSAG.color = Color.red;
                }
                else
                {
                    MissilesSAG.color = Color.white;
                }
            }
            else
            {
                MissilesSAG.color = Color.red;
            }
        }
        else
        {
            MissilesSAG.enabled = false;
        }
    }

    void Rocket()
    {
        if (Rockets == true)
        {
            if (RktControl.isPlayer)
            {
                Rockets1.text = ">" + RktControl.weaponName +"    " + RktControl.rocketAmmo;
            }
            else
            {
                Rockets1.text = RktControl.weaponName + "    " + RktControl.rocketAmmo;
            }
            if (RktControl.rocketAmmo == 0 || hub.planeCam.camShaking == true)
            {
                Rockets1.color = Color.red;
            }
            else
            {

                Rockets1.color = Color.white;
            }
        }
        else
        {
            Rockets1.enabled = false;   
        }
    }

    void Bomb()
    {
        if (bomb == true)
        {
            Bombs.text = "Bombs    " + bmbControl.bombAmmo;
            if (bmbControl.bombAmmo == 0 || hub.planeCam.camShaking == true)
            {
                Bombs.color = Color.red;
            }
            else
            {

                Bombs.color = Color.white;
            }
        }
        else
        {
            Bombs.enabled = false;
        }
    }

    void GuidedBomb()
    {
        if (guidedBomb == true)
        {
            guidedBombs.text = "Guided Bombs    " + lgbController.bombAmmo;
            if (lgbController.bombAmmo == 0 || hub.planeCam.camShaking == true)
            {
                guidedBombs.color = Color.red;
            }
            else
            {

                guidedBombs.color = Color.white;
            }
        }
        else
        {
            guidedBombs.enabled= false;
        }
    }

    void CountKillsCombo()
    {
        if (hub.killcounter.comboCounting)
        {

            if(hub.killcounter.currentCombo == 1)
            {
                killsCombo.enabled = true;
                killsCombo.text = "Splash 1!";
            }
            else if(hub.killcounter.currentCombo  > 1)
            {
                killsCombo.text = "Splash " + hub.killcounter.currentCombo + "!";
            }
            if (hub.planeCam.camShaking || hub.killcounter.currentCombo > 1)
            {
                killsCombo.color = Color.red;
            }
            else if(hub.planeCam.camShaking == false)
            {
                killsCombo.color = Color.white;
            }
        }
        else
        {
            killsCombo.text = "Splash 1!";
            killsCombo.enabled = false;
        }
    }
}
