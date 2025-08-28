using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPS_Counter : MonoBehaviour
{
	public static FPS_Counter Instance;
	
    void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		
		else
		{
			Destroy(gameObject);
			return;
		}
		
	}
	
	public TMP_Text fpsCounterTxt;
	void Update()
	{
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
}
