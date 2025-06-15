using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyMarkers : MonoBehaviour
{
	public AircraftHub player;
    public List<GameObject> enemiesToBeMarked = new List<GameObject>();
    [SerializeField]List<Renderer> enemyRenderer;
    public List<GameObject> alliesToBeMarked;
    [SerializeField] List<Renderer> alliesRenderers;
    [SerializeField] List<GameObject> enemyMarkers, alliedMarkers;
    [SerializeField] GameObject markerPrefab, alliedMarkerPrefab, selectedTargetPrefab;
    [SerializeField] GameObject targetLockedMarker;
	public AircraftHub targetLockedHub;
    public GameObject leadMarkerGO;
	Transform leadMarker;
	public TMP_Text distanceMarker;
	public TMP_Text targetName;
	public float distanceToLeadMarker = 800f;
    Vector3 screenPos;

    public RadarMinimap minimap;

    void Start()
    {
        enemyRenderer = new List<Renderer>(enemiesToBeMarked.Count);
        alliesRenderers = new List<Renderer>(alliesToBeMarked.Count);
        enemyMarkers = new List<GameObject>(enemiesToBeMarked.Count);
        alliedMarkers = new List<GameObject>(alliesToBeMarked.Count);
        targetLockedMarker = new GameObject();
		leadMarkerGO.SetActive(false);
		distanceMarker.gameObject.SetActive(false);

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
		
		leadMarker = leadMarkerGO.transform;
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

        if (Camera.main == null || targetLockedHub == null)
        {
            targetLockedMarker.SetActive(false);
			leadMarkerGO.SetActive(false);
			distanceMarker.gameObject.SetActive(false);
			targetName.gameObject.SetActive(false);
        }
        else if (Camera.main != null && targetLockedHub != null)
        {
			if(targetLockedHub.fm.side == 1)
			{
				targetLockedMarker.SetActive(false);
				leadMarkerGO.SetActive(false);
				distanceMarker.gameObject.SetActive(false);
				targetName.gameObject.SetActive(false);
				return;
			}
            screenPos = Camera.main.WorldToScreenPoint(transform.position);
            if (targetLockedHub.meshRenderer.isVisible)
            {
                targetLockedMarker.SetActive(true);
                targetLockedMarker.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, targetLockedHub.transform.TransformPoint(Vector3.zero));
				ShowLeadMarker();
				ShowTargetInfo();
            }

            else
            {
                targetLockedMarker.SetActive(false);
				leadMarkerGO.SetActive(false);
				distanceMarker.gameObject.SetActive(false);
				targetName.gameObject.SetActive(false);
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
	
    void ShowLeadMarker()
    {    
		if(targetLockedHub == null)
        {
            leadMarkerGO.SetActive(false);
			return;
        }
        else if(targetLockedHub != null)
        {
            float distToTarget = Vector3.Distance(player.transform.position, targetLockedHub.transform.position);
            if(distToTarget < distanceToLeadMarker)
            {
                Vector3 leadPos = Utilities.FirstOrderIntercept(player.transform.position, player.rb.velocity, player.gunsControl.guns[0].muzzleVelocity, targetLockedHub.transform.position, targetLockedHub.rb.velocity);

                var hudPos = Utilities.TransformToHUDSpace(leadPos);

                if (hudPos.z > 0)
                {
                    leadMarkerGO.SetActive(true);
                    leadMarker.localPosition = new Vector3(hudPos.x, hudPos.y, 0);
                }
                else
                {
                    leadMarkerGO.SetActive(false);
                }
            }
            else
            {
                leadMarkerGO.SetActive(false);
            }
        }
    }
	
	void ShowTargetInfo()
	{
		if(targetLockedHub == null)
        {
            distanceMarker.gameObject.SetActive(false);
			targetName.gameObject.SetActive(false);
			return;
        }
        else if(targetLockedHub != null)
        {
            float distToTarget = Vector3.Distance(player.transform.position, targetLockedHub.transform.position);
			screenPos = Camera.main.WorldToScreenPoint(transform.position);
			distanceMarker.gameObject.SetActive(true);
			targetName.gameObject.SetActive(true);
            distanceMarker.gameObject.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, targetLockedHub.transform.TransformPoint(Vector3.zero));
			targetName.gameObject.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, targetLockedHub.transform.TransformPoint(Vector3.zero));
			{
				float x = distToTarget / 1000f;
				x *= 100;
				x = Mathf.Floor(x);
				x /= 100;
				distToTarget = x;
			}
			distanceMarker.text = distToTarget + " km";
			
			if(distToTarget >= 1.5f)
			{
				targetName.text = targetLockedHub.nameShort;
			}
			else if(distToTarget < 1.5f && distToTarget >= 0.4f)
			{
				targetName.text = targetLockedHub.nameLong;
			}
			else if(distToTarget < 0.4f)
			{
				targetName.text = targetLockedHub.aircraftName;
			}
        }
	}
	
	void OnDisable()
	{
	    targetLockedMarker.SetActive(false);
		leadMarkerGO.SetActive(false);
		distanceMarker.gameObject.SetActive(false);
		targetName.gameObject.SetActive(false);
	}
}
