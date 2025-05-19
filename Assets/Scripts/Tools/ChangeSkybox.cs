using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSkybox : MonoBehaviour
{

    public Material skybox;
    public float fog;

    // Use this for initialization
    void Start()
    {
        if(skybox != null)
        {
            RenderSettings.skybox = skybox;
            RenderSettings.fogDensity = fog;
        }
    }
}