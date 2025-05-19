using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMarkers : MonoBehaviour
{
    public List<GameObject> enemiesToBeMarked = new List<GameObject>();
    [SerializeField]List<Renderer> enemyRenderer;
    public List<GameObject> alliesToBeMarked;
    [SerializeField] List<Renderer> alliesRenderers;
    [SerializeField] List<GameObject> enemyMarkers, alliedMarkers;
    [SerializeField] GameObject markerPrefab, alliedMarkerPrefab, selectedTargetPrefab;
    [SerializeField] GameObject targetLockedMarker;
    public Renderer targetLocked;
    Vector3 screenPos;

    public RadarMinimap minimap;

    void Start()
    {
        enemyRenderer = new List<Renderer>(enemiesToBeMarked.Count);
        alliesRenderers = new List<Renderer>(alliesToBeMarked.Count);
        enemyMarkers = new List<GameObject>(enemiesToBeMarked.Count);
        alliedMarkers = new List<GameObject>(alliesToBeMarked.Count);
        targetLockedMarker = new GameObject();

        for (int i = 0; i < enemiesToBeMarked.Count; i++)
        {
            enemyRenderer.Add(enemiesToBeMarked[i].GetComponent<Renderer>());
        }
        for (int i = 0; i < alliesToBeMarked.Count; i++)
        {
            alliesRenderers.Add(alliesToBeMarked[i].GetComponent<Renderer>());
        }
        for (int i = 0;i < enemiesToBeMarked.Count; i++)
        {
            enemyMarkers.Add(Instantiate(markerPrefab, enemiesToBeMarked[i].transform.position, transform.rotation, this.transform));
        }
        for (int i = 0; i < alliesToBeMarked.Count; i++)
        {
            alliedMarkers.Add(Instantiate(alliedMarkerPrefab, alliesToBeMarked[i].transform.position, transform.rotation, this.transform));
        }
        targetLockedMarker = Instantiate(selectedTargetPrefab, transform.position, transform.rotation, this.transform);
    }


    void LateUpdate()
    {
        for (int i = 0; i < enemyRenderer.Count; i++)
        {
            if (Camera.main != null && enemyRenderer[i] != null)
            {
                screenPos = Camera.main.WorldToScreenPoint(transform.position);
                if (enemyRenderer[i].isVisible)
                {
                    enemyMarkers[i].SetActive(true);
                    enemyMarkers[i].transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, enemyRenderer[i].transform.TransformPoint(Vector3.zero));
                }

                else
                {
                    enemyMarkers[i].SetActive(false);
                }
            }
            else if (enemyRenderer[i] == null)
            {
                enemyMarkers[i].SetActive(false);
            }
        }

        for (int i = 0; i < alliesRenderers.Count; i++)
        {
            if (Camera.main != null && alliesRenderers[i] != null)
            {
                screenPos = Camera.main.WorldToScreenPoint(transform.position);
                if (alliesRenderers[i].isVisible)
                {
                    alliedMarkers[i].SetActive(true);
                    alliedMarkers[i].transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, alliesRenderers[i].transform.TransformPoint(Vector3.zero));
                }

                else
                {
                    alliedMarkers[i].SetActive(false);
                }
            }
            else if (alliesRenderers[i] == null)
            {
                alliedMarkers[i].SetActive(false);
            }
        }

        if (Camera.main == null || targetLocked == null)
        {
            targetLockedMarker.SetActive(false);
        }
        else if (Camera.main != null && targetLocked != null)
        {
            screenPos = Camera.main.WorldToScreenPoint(transform.position);
            if (targetLocked.isVisible)
            {
                targetLockedMarker.SetActive(true);
                targetLockedMarker.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, targetLocked.transform.TransformPoint(Vector3.zero));
            }

            else
            {
                targetLockedMarker.SetActive(false);
            }
        }

    }

    public void AddMarker(GameObject objWithRenderer)
    {
        enemiesToBeMarked.Add(objWithRenderer);
        enemyRenderer.Add(objWithRenderer.GetComponent<Renderer>());
        enemyMarkers.Add(Instantiate(markerPrefab, objWithRenderer.transform.position, transform.rotation, this.transform));

        if(minimap != null)
        {
            minimap.AddAllyBlip(objWithRenderer.transform, 0);
        }
    }

    public void AddMarker(AircraftHub aircraftToAdd)
    {
        enemiesToBeMarked.Add(aircraftToAdd.gameObject);
        enemyRenderer.Add(aircraftToAdd.meshRenderer);
        enemyMarkers.Add(Instantiate(markerPrefab, aircraftToAdd.transform.position, transform.rotation, this.transform));

        if (minimap != null)
        {
            minimap.AddAllyBlip(aircraftToAdd.transform, 0);
        }
    }

    public void AddAllyMarker(AircraftHub aircraftToAdd)
    {
        alliesToBeMarked.Add(aircraftToAdd.gameObject);
        alliesRenderers.Add(aircraftToAdd.meshRenderer);
        alliedMarkers.Add(Instantiate(alliedMarkerPrefab, aircraftToAdd.transform.position, transform.rotation, this.transform));

        if (minimap != null)
        {
            minimap.AddAllyBlip(aircraftToAdd.transform, 1);
        }
    }
}
