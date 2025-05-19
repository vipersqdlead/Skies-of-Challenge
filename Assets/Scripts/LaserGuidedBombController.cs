using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGuidedBombController : MonoBehaviour
{
    [SerializeField] int maxBombs;
    public int bombAmmo;
    float bombReload = 15f;
    float reloadTime;
    public GameObject bomb;
    public GameObject[] bombPos;
    public int bmbIndex = 0;
    [SerializeField] Camera cam;
    float rayDistance = 8000f;
    Vector2 pos;

    [SerializeField] KillCounter killCounter;

    // Start is called before the first frame update
    void Start()
    {
        bombAmmo = maxBombs;
        reloadTime = bombReload;
    }

    // Update is called once per frame
    void Update()
    {
        pos = Input.mousePosition;
        cam = Camera.main;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = cam.ScreenPointToRay(pos);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, rayDistance) && bombAmmo != 0)
            {
                Debug.Log("Choqué");
                DropLaserBomb(hitInfo.point);
            }
        }

        if(bombAmmo == 0)
        {
            Reload();
        }
    }

    void DropLaserBomb(Vector3 targetPos)
    {
        GameObject bombGo = Instantiate(bomb, bombPos[bmbIndex].transform.position, bombPos[bmbIndex].transform.rotation);
        Rigidbody bombRb = bombGo.GetComponent<Rigidbody>();
        bombRb.AddForce(gameObject.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
        bombGo.GetComponent<LaserGuidedBomb>().SetKillEnemyDelegate(EnemyKilled);

        LaserGuidedBomb lgb = bombGo.GetComponent<LaserGuidedBomb>();
        lgb.target = targetPos;

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
