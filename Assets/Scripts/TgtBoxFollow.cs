using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TgtBoxFollow : MonoBehaviour
{
    [SerializeField] public GameObject lookAt;

    [SerializeField] TMP_Text textName;
    [SerializeField] string enemyName;
    Vector3 screenPoint;
    bool onScreen;
    HealthPoints HP;
    float maxHP;
    [SerializeField] float healthPercentage;
    [SerializeField] Image hpBar;

    private void Start()
    {
        lookAt = GameObject.FindWithTag("Player");
        textName.text = enemyName;
        HP = gameObject.GetComponentInParent<HealthPoints>();
        maxHP = HP.HP;
    }

    void Update()
    {

        screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        if (onScreen)
        {
            UpdateBox();
        }
    }
    
    void UpdateBox()
    {
        transform.LookAt(lookAt.transform.position);
        if (lookAt == null)
        {
            Destroy(gameObject);
        }
        hpBar.fillAmount = (HP.HP * 100 / maxHP) / 100;
    }
}
