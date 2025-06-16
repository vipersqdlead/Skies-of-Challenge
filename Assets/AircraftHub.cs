using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftHub : MonoBehaviour
{
    public FlightModel fm;
    public PlayerInputs playerInputs;
    public EngineControl engineControl;
    public Rigidbody rb;
    public GunsControl gunsControl;
    public PlaneToUI planeToUI;
    public HealthPoints hp;
    public KillCounter killcounter;
    public PlaneCamera planeCam;
    public PlaneHUD planeHUD;

    public SpWeaponManager weaponManager;
    public IRMissileControl irControl;
    public SAGMissileControl sagControl;
    public RadarMissileControl radarMissileControl;
    public RocketLauncherControl rocketLauncherControl;
    public BombControl bombControl;
    public FlareDispenser flareDispenser;

    public MeshRenderer meshRenderer;


    [Header("Aircraft Stats")]
    public string aircraftName;
	public string nameShort, nameLong;
    public int speed_maxSpeed;
    public float power_maxPower;
    public bool isJet;
public int engineNumber;
    public float powerToWeight;
    public float attack_totalBurstMass;
    public float agility_maxTurnDegS;
    public float agility_wingLoading;
    public int agility_maxTurnSpeed;
    public float agility_weight;
    public float health_maxHP;
    public float defense_def;
    [TextArea] public string aircraftDescription;
    public GameObject aircraftModel;

    public void Awake()
    {
        fm = GetComponent<FlightModel>();
        playerInputs = GetComponent<PlayerInputs>();
        engineControl = GetComponent<EngineControl>();
        rb = GetComponent<Rigidbody>();
        gunsControl = GetComponent<GunsControl>();
        planeToUI = GetComponent<PlaneToUI>();
        hp = GetComponent<HealthPoints>();
        killcounter = GetComponent<KillCounter>();
        planeCam = GetComponent<PlaneCamera>();
        planeHUD = GetComponent<PlaneHUD>();

        weaponManager = GetComponent<SpWeaponManager>();
        irControl = GetComponent<IRMissileControl>();
        sagControl = GetComponent<SAGMissileControl>();
        radarMissileControl = GetComponent<RadarMissileControl>();
        rocketLauncherControl = GetComponent<RocketLauncherControl>();
        bombControl = GetComponent<BombControl>();
        flareDispenser = GetComponent<FlareDispenser>();
		
		CalculateSomeStats();

        if(meshRenderer == false)
        {
            print("Mesh Renderer hasn't been assigned!");
        }
    }

    public void CalculateSomeStats()
    {
        agility_weight = rb.mass;
	    agility_wingLoading = (int)(agility_weight / fm.wingArea);

        if (engineControl.enginePropellers.Length == 0)
        {
            power_maxPower = (int)((engineControl.engineStaticThrust + engineControl.afterburnerThrust) * 0.101972f);
            isJet = true;
	    engineNumber = engineControl.engines.Length;
            powerToWeight = (power_maxPower * engineNumber) / agility_weight;
        {
            float x = powerToWeight;
            x *= 100;
            x = Mathf.Floor(x);
            x /= 100;
            powerToWeight = x;
        }
        }
        else if (engineControl.enginePropellers[0] != null)
        {
            power_maxPower = (int)(engineControl.engineStaticThrust + engineControl.afterburnerThrust) / 5;
	    engineNumber = engineControl.engines.Length;
            powerToWeight = agility_weight / (power_maxPower * engineNumber);
        {
            float x = powerToWeight;
            x *= 100;
            x = Mathf.Floor(x);
            x /= 100;
            powerToWeight = x;
        }
        }

        agility_maxTurnDegS = 0f;
        for (int i = 0; i < 1236; i += 5)
        {
            if(fm.PitchForce.Evaluate(i / 1234f) > agility_maxTurnDegS)
            {
                agility_maxTurnDegS = fm.PitchForce.Evaluate(i / 1234f);
                agility_maxTurnSpeed = i;
            }
        }
                {
                    float x = agility_maxTurnDegS;

                    x *= 100f;
                    x = Mathf.Floor(x);
                    x /= 100f;
		    x *= 100f;
                    agility_maxTurnDegS = x;
                }

        float totalBurstMass = 0f;
        foreach (Gun gun in gunsControl.guns)
        {
			if(gun == null)
				continue;
			
			if(gun.shells[0] == null)
				continue;
			
            float gunBurstMass = (gun.shells[0].GetComponent<Rigidbody>().mass) * gun.rateOfFireRPM / 60f;
            totalBurstMass += gunBurstMass;
        }
        {
            float x = totalBurstMass;
            x *= 100f;
            x = Mathf.Floor(x);
            x /= 100f;
            totalBurstMass = x;
        }
        attack_totalBurstMass = totalBurstMass;





        health_maxHP = hp.HP;
        defense_def = hp.Defense;
    }
}
