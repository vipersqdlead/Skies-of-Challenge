using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DeathCamera : MonoBehaviour
{

    public GameObject Player;
    public bool isDeath = false;
    public new Camera camera; public AudioListener audioListener;
    [SerializeField] float camSpeed;
    // Start is called before the first frame update
    void Awake()
    {
        camera = gameObject.GetComponent<Camera>();
        audioListener = gameObject.GetComponent<AudioListener>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Player != null)
        {
            gameObject.transform.position = Player.transform.position + new Vector3(70, 70, 70);
            transform.LookAt(Player.transform.position, Vector3.up);
        }

        if(Player == null)
        {
            isDeath = true;
            camera.enabled = true;
            audioListener.enabled = true;
            transform.position -= transform.forward * camSpeed * Time.deltaTime;
        }
    }
}
