using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombControl : MonoBehaviour
{
    [SerializeField] int maxBombs;
    public int bombAmmo;
    float bombReload = 15f;
    float reloadTime;
    public GameObject bomb;
    public GameObject[] bombPos;
    public int bmbIndex = 0;

    [SerializeField] KillCounter killCounter;

    private void Start()
    {
        bombAmmo = maxBombs;
        reloadTime = bombReload;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.LeftAlt) && bombAmmo != 0)
        {
            DropBomb();
        }

        if (bombAmmo == 0)
        {
            Reload();
        }
    }

    void DropBomb()
    {
        GameObject bombGo = Instantiate(bomb, bombPos[bmbIndex].transform.position, bombPos[bmbIndex].transform.rotation);
        Rigidbody bombRb = bombGo.GetComponent<Rigidbody>();
        bombRb.AddForce(gameObject.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
        bombGo.GetComponent<BombScript>().SetKillEnemyDelegate(EnemyKilled);

        bombAmmo--;
        bombPos[bmbIndex].SetActive(false);

        if (bmbIndex < bombPos.Length - 1)
        {
            bmbIndex++;
        }
        else
        {
            bmbIndex = 0;
        }
    }

    void Reload()
    {
        reloadTime -= Time.deltaTime;
        if (reloadTime < 0)
        {
            bombAmmo = maxBombs;
            reloadTime = bombReload;
            for (int i = 0; i < bombPos.Length; i++)
            {
                bombPos[i].SetActive(true);
            }
        }
    }

    public void EnemyKilled(bool countsAsKill, int points)
    {
        if (countsAsKill)
        {
            killCounter.Kills++;
        }
        killCounter.Points += points;
        print("Got a kill!");
    }
}
