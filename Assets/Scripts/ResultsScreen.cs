using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultsScreen : MonoBehaviour
{
    [SerializeField] TMP_Text M1Score, M2Score, M3Score, M4Score, M5Score, M6Score, M7Score, M8Score, M9Score, M10Score, M11Score, M12Score, TotalScore, HighScore;

    int finalScore;

    // Start is called before the first frame update
    void Start()
    {
        M1Score.text = "Mission 1: " + PlayerPrefs.GetInt("Mission1Score") + " pts.";
        M2Score.text = "Mission 2: " + PlayerPrefs.GetInt("Mission2Score") + " pts.";
        M3Score.text = "Mission 3: " + PlayerPrefs.GetInt("Mission3Score") + " pts.";
        M4Score.text = "Mission 4: " + PlayerPrefs.GetInt("Mission4Score") + " pts.";
        M5Score.text = "Mission 5: " + PlayerPrefs.GetInt("Mission5Score") + " pts.";
        M6Score.text = "Mission 6: " + PlayerPrefs.GetInt("Mission6Score") + " pts.";
        M7Score.text = "Mission 7: " + PlayerPrefs.GetInt("Mission7Score") + " pts.";
        M8Score.text = "Mission 8: " + PlayerPrefs.GetInt("Mission8Score") + " pts.";
        M9Score.text = "Mission 9: " + PlayerPrefs.GetInt("Mission9Score") + " pts.";
        M10Score.text = "Mission 10: " + PlayerPrefs.GetInt("Mission10Score") + " pts.";
        M11Score.text = "Mission 11: " + PlayerPrefs.GetInt("Mission11Score") + " pts.";

        finalScore = CalculateTotalScore();
        PlayerPrefs.SetInt("LastScore", finalScore);

        if(PlayerPrefs.GetInt("MaxScore") < finalScore)
        {
            HighScore.enabled = true;
            PlayerPrefs.SetInt("MaxScore", finalScore);
        }

        TotalScore.text = "Total: " + finalScore + " pts.";
    }

    int CalculateTotalScore()
    {
        return PlayerPrefs.GetInt("Mission1Score") + PlayerPrefs.GetInt("Mission2Score") + PlayerPrefs.GetInt("Mission3Score") + PlayerPrefs.GetInt("Mission4Score") + PlayerPrefs.GetInt("Mission5Score") + PlayerPrefs.GetInt("Mission6Score") + PlayerPrefs.GetInt("Mission7Score") + PlayerPrefs.GetInt("Mission8Score") + PlayerPrefs.GetInt("Mission9Score") + PlayerPrefs.GetInt("Mission10Score") + PlayerPrefs.GetInt("Mission11Score");
    }
}
