using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCounter : MonoBehaviour
{
    public int Kills = 0;
    public int Points = 0;
	public int hits = 0;

    float comboBonusTimer = 10f;
    public bool comboCounting;
    public int currentCombo;

    private void Update()
    {
        if (comboCounting)
        {
            comboBonusTimer -= Time.deltaTime;
        }

        if(comboBonusTimer < 0)
        {
            comboBonusTimer = 10f;
            currentCombo = 0;
            comboCounting = false;
        }
    }

    public void GiveKill(bool countsAsKill, int points)
    {
        if(comboCounting == false)
        {
            if (countsAsKill)
            {
                Kills += 1;
            }
            Points += points;
            comboCounting = true;
            currentCombo = 1;
            return;
        }
        if (comboCounting == true)
        {
            if (countsAsKill)
            {
                Kills += 1;
            }
            comboBonusTimer = 10f;
            currentCombo += 1;
            Points += (points * currentCombo);
            return;
        }
    }

    public void GivePoints(int points)
    {
        if (comboCounting == false)
        {
            Points += points;
			hits++;
            return;
        }
        if (comboCounting == true)
        {
            Points += (points * currentCombo);
			hits++;
            return;
        }
    }
}
