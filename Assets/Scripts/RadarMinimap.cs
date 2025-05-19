using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Reflection;
using static UnityEngine.EventSystems.EventTrigger;

public class RadarMinimap : MonoBehaviour
{
    [SerializeField] EnemyMarkers markers;

    public RectTransform minimapContainer; // UI Panel for the minimap
    public RectTransform playerIcon; // UI Element for player
    public RectTransform enemyBlipPrefab, allyBlipPrefab; // Prefab for ally/enemy icons

    public Transform player; // Player's transform
    public float radarSize = 100f; // Size of the radar in UI space
    public float worldScale = 50f; // How much world space is mapped to minimap space

    public List<Transform> allies;
    public List<Transform> enemies;

    private List<RectTransform> enemyBlips = new List<RectTransform>();
    private List<RectTransform> allyBlips = new List<RectTransform>();

    void Start()
    {
        foreach(GameObject obj in markers.alliesToBeMarked)
        {
            allies.Add(obj.transform);
        }

        foreach(GameObject obj in markers.enemiesToBeMarked)
        {
            enemies.Add(obj.transform);
        }
        // Create blips for all allies and enemies
        foreach (var unit in allies) AddAllyBlip(unit);
        foreach (var unit in enemies) AddEnemyBlip(unit);
    }

    void LateUpdate()
    {
        UpdateBlips();
    }

    void AddEnemyBlip(Transform unit)
    {
        RectTransform newBlip = Instantiate(enemyBlipPrefab, minimapContainer);
        enemyBlips.Add(newBlip);
    }

    void AddAllyBlip(Transform unit)
    {
        RectTransform newBlip = Instantiate(allyBlipPrefab, minimapContainer);
        allyBlips.Add(newBlip);
    }

    void UpdateBlips()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null)
            {
                if (enemyBlips[i] != null)
                {
                    enemyBlips[i].gameObject.SetActive(false);
                }
                enemyBlips.RemoveAt(i);
                enemies.RemoveAt(i);
            }
            else if(enemies[i] != null && enemyBlips[i] != null)
            {
                UpdateBlipPosition(enemyBlips[i], enemies[i].position);
            }
        }

        for (int i = 0; i < allies.Count; i++)
        {
            if (allies[i] == null)
            {
                if (allyBlips[i] != null)
                {
                    allyBlips[i].gameObject.SetActive(false);
                }
                allyBlips.RemoveAt(i);
                allies.RemoveAt(i);
            }
            else if (allies[i] != null && allyBlips[i] != null)
            {
                UpdateBlipPosition(allyBlips[i], allies[i].position);
            }

        }
    }

    void UpdateBlipPosition(RectTransform blip, Vector3 worldPos)
    {
        Vector3 offset = worldPos - player.position;
        offset.y = 0; // Ignore vertical difference

        // Rotate offset using player's rotation
        Vector3 rotatedOffset = Quaternion.Euler(0, -player.eulerAngles.y, 0) * offset;

        // Scale position for minimap
        Vector2 minimapPos = new Vector2(rotatedOffset.x, rotatedOffset.z) / worldScale * radarSize;

        // Clamp to minimap bounds
        minimapPos = Vector2.ClampMagnitude(minimapPos, radarSize * 0.5f);

        blip.anchoredPosition = minimapPos;
    }

    public void AddAllyBlip(Transform newBlip, int side)
    {
        if (side == 0)
        {
            AddEnemyBlip(newBlip);
            enemies.Add(newBlip);
        }
        else
        {
            AddAllyBlip(newBlip);
            allies.Add(newBlip);
        }
    }
}
