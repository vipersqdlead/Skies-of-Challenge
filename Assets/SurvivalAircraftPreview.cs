using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalAircraftPreview : MonoBehaviour
{
    SurvivalSettings settings;
    [SerializeField] string[] text;

    private void Awake()
    {
        settings = GetComponent<SurvivalSettings>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
