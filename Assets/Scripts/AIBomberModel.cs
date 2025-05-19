using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBomberModel : BasicAIModel
{
    public SquadPosType sqdPosition;

    public BomberType type;

    public float MaxTurn; // How much does the plane turn at current speed?
    public float Agility; // Multiplier for agility.

    [SerializeField] float currentSpeed;

    public List<GameObject> Targets = new List<GameObject>(); // List of possible Targets the AI could aquire.
    public GameObject Target; // Current selected Target
    Rigidbody AIRigidbody; // Literally the plane's rb

    public float Power, Drag; // Estimated values for the AI's engine power and Drag. They should be similar to those of the same Player-controlledd plane (That means, an AI Bf 109 should have Power and Drag values that kinda match the MaxEnginePower and Drag of a playable Bf 109)



    // Start is called before the first frame update
    void Start()
    {
        AIRigidbody = GetComponent<Rigidbody>();
        AIRigidbody.velocity = new Vector3(83, 0, 0);

        Target = Targets[0];
    }

    // Update is called once per frame
    void Update()
    {
        AIRigidbody.drag = Drag / 1000f;

        MaxTurn = (AIRigidbody.velocity.magnitude / Agility);

        AIRigidbody.AddForce(transform.forward * Power, ForceMode.Force);
        AIRigidbody.velocity = transform.forward * AIRigidbody.velocity.magnitude;

        currentSpeed = AIRigidbody.velocity.magnitude * 3.6f;
    }




    public enum SquadPosType
    {
        Leader,
        Wingman
    }
    public enum BomberType
    {
        LevelBomber,
        DiveBomber
    }

}
