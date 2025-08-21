using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveSpawner : MonoBehaviour
{
    public FlightModel player;
	[SerializeField] List<GameObject> allAircraftPrefabs;
	[SerializeField] List<GameObject> sortedAircraft;
	[SerializeField] List<GameObject> eligibleAircraft;
    [SerializeField] List<GameObject> jetTier1WavePrefabs;
    [SerializeField] List<GameObject> jetTier1BonusWavePrefabs;
    [SerializeField] List<GameObject> propWavePrefabs;
    [SerializeField] List<GameObject> propBonusWavePrefabs;
    [SerializeField] List<GameObject> trainerWavePrefabs;
    [SerializeField] List<GameObject> jetTier2WavePrefabs;
    [SerializeField] List<GameObject> jetTier2BonusWavePrefabs;
	[SerializeField] List<GameObject> jetTier3WavePrefabs;
    [SerializeField] List<GameObject> jetTier3BonusWavePrefabs;
	[SerializeField] List<GameObject> jetTier4WavePrefabs;
    [SerializeField] List<GameObject> jetTier4BonusWavePrefabs;
    [SerializeField] List<Transform> SpawnPositions;
    public EnemyMarkers markers;
    public SurvivalMissionStatus status;
	public bool enableBlackouts;

	public int currentDifficulty = 400;

	public void OrderLists()
	{	
		sortedAircraft = allAircraftPrefabs.OrderBy(p => p.GetComponent<Wave>().aircraft[0].gameObject.GetComponent<HealthPoints>().pointsWorth).ToList();
		eligibleAircraft = GetAircraftByDifficultyRange();
		print("Current Rating is: " + currentDifficulty + ". Number of possible enemies at this rating is " + eligibleAircraft.Count);
	}
	
	public List<GameObject> GetAircraftByDifficultyRange(int range = 100)
	{
		int minPoints = currentDifficulty - range;
		int maxPoints = currentDifficulty + range;

		return sortedAircraft
			.Where(p =>
			{
				var hp = p.GetComponent<Wave>().aircraft[0].gameObject.GetComponent<HealthPoints>();
				return hp != null && hp.pointsWorth >= minPoints && hp.pointsWorth <= maxPoints;
			})
			.OrderBy(p => p.GetComponent<Wave>().aircraft[0].gameObject.GetComponent<HealthPoints>().pointsWorth)
			.ToList();
	}

	public void spawnEnemyWave(int numberOfEnemies)
	{
		List<Transform> auxSpawnPositions = SpawnPositions;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            int spawnRand = Random.Range(0, auxSpawnPositions.Count);
            GameObject newWave = Instantiate(eligibleAircraft[Random.Range(0, eligibleAircraft.Count)], Utilities.GetSafeSpawnAltitude(auxSpawnPositions[spawnRand].position, player.transform.position.y), auxSpawnPositions[spawnRand].rotation);
            Vector3 pos = GetSpawnPosition(player.transform.position.y, 8000f);
			//GameObject newWave = Instantiate(eligibleAircraft[Random.Range(0, eligibleAircraft.Count)], pos, pos.forward);
			//auxSpawnPositions.Remove(auxSpawnPositions[spawnRand]);
            Wave wave = newWave.GetComponent<Wave>();
            wave.AddRenderersToMarker(markers, status, player);
            foreach (AircraftHub hub in wave.aircraft)
            {
				hub.fm.experiencesG = enableBlackouts;
                if (hub.transform.position.y < 0f)
                {
                    hub.transform.position = new Vector3(hub.transform.position.x, Mathf.Abs(hub.transform.position.y), hub.transform.position.z);
                }
				int agGuns = Random.Range(1, 10);
				if(agGuns <= 1)
				{
					hub.gunsControl.EnableAG();
				}
            }
        }
	}
	
	public void SpawnAllyWave(int numberOfAllies)
	{
		List<Transform> auxSpawnPositions = SpawnPositions;

        for (int i = 0; i < numberOfAllies; i++)
        {
			GameObject newWave = Instantiate(eligibleAircraft[Random.Range(0, eligibleAircraft.Count)], Utilities.GetSafeSpawnAltitude(new Vector3(0, 4250, 0), player.transform.position.y), transform.rotation);
			//auxSpawnPositions.Remove(auxSpawnPositions[spawnRand]);
			Wave wave = newWave.GetComponent<Wave>();
			wave.AddAllyRenderersToMarker(markers, status);
			foreach (AircraftHub hub in wave.aircraft)
			{
				hub.fm.experiencesG = enableBlackouts;
								
				hub.hp.extraLives = 1;
				if(hub.irControl != null)
				{
					hub.irControl.canReload = true;
				}
				int agGuns = Random.Range(1, 10);
				if(agGuns <= 3)
				{
					hub.gunsControl.EnableAG();
				}
				
				if (hub.transform.position.y < 0f)
				{
					hub.transform.position = new Vector3(hub.transform.position.x, Mathf.Abs(hub.transform.position.y), hub.transform.position.z);
				}
			}
        }
	}

    public void JetTier1SpawnWave(int numberOfEnemies)
    {
        List<Transform> auxSpawnPositions = SpawnPositions;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            int spawnRand = Random.Range(0, auxSpawnPositions.Count);
            GameObject newWave = Instantiate(jetTier1WavePrefabs[Random.Range(0, jetTier1WavePrefabs.Count)], GetSafeSpawnAltitude(auxSpawnPositions[spawnRand].position, player.transform.position.y), auxSpawnPositions[spawnRand].rotation);
            //auxSpawnPositions.Remove(auxSpawnPositions[spawnRand]);
            Wave wave = newWave.GetComponent<Wave>();
            wave.AddRenderersToMarker(markers, status, player);
            foreach (AircraftHub hub in wave.aircraft)
            {				
				hub.fm.experiencesG = enableBlackouts;
                if (hub.transform.position.y < 0f)
                {
                    hub.transform.position = new Vector3(hub.transform.position.x, Mathf.Abs(hub.transform.position.y), hub.transform.position.z);
                }
            }
        }
    }
	
    public void JetTier1BonusSpawnWave()
    {
        List<Transform> auxSpawnPositions = SpawnPositions;

        int spawnRand = Random.Range(0, auxSpawnPositions.Count);
        int randomWave = Random.Range(0, jetTier1BonusWavePrefabs.Count);
        print(randomWave);
        GameObject newWave = Instantiate(jetTier1BonusWavePrefabs[randomWave], GetSafeSpawnAltitude(auxSpawnPositions[spawnRand].position, player.transform.position.y), auxSpawnPositions[spawnRand].rotation);
        //auxSpawnPositions.Remove(auxSpawnPositions[spawnRand]);
        Wave wave = newWave.GetComponent<Wave>();
        wave.AddRenderersToMarker(markers, status, player);
    }

    public void JetTier2SpawnWave(int numberOfEnemies)
    {
        List<Transform> auxSpawnPositions = SpawnPositions;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            int spawnRand = Random.Range(0, auxSpawnPositions.Count);
            GameObject newWave = Instantiate(jetTier2WavePrefabs[Random.Range(0, jetTier2WavePrefabs.Count)], GetSafeSpawnAltitude(auxSpawnPositions[spawnRand].position, player.transform.position.y), auxSpawnPositions[spawnRand].rotation);
            //auxSpawnPositions.Remove(auxSpawnPositions[spawnRand]);
            Wave wave = newWave.GetComponent<Wave>();
            wave.AddRenderersToMarker(markers, status, player);
            foreach (AircraftHub hub in wave.aircraft)
            {
				hub.fm.experiencesG = enableBlackouts;
                if (hub.transform.position.y < 0f)
                {
                    hub.transform.position = new Vector3(hub.transform.position.x, Mathf.Abs(hub.transform.position.y), hub.transform.position.z);
                }
            }
        }
    }

    public void JetTier2BonusSpawnWave()
    {
        List<Transform> auxSpawnPositions = SpawnPositions;

        int spawnRand = Random.Range(0, auxSpawnPositions.Count);
        int randomWave = Random.Range(0, jetTier2BonusWavePrefabs.Count);
        print(randomWave);
        GameObject newWave = Instantiate(jetTier2BonusWavePrefabs[randomWave], GetSafeSpawnAltitude(auxSpawnPositions[spawnRand].position, player.transform.position.y), auxSpawnPositions[spawnRand].rotation);
        Wave wave = newWave.GetComponent<Wave>();
        wave.AddRenderersToMarker(markers, status, player);
    }

    public void PropSpawnWave(int numberOfEnemies)
    {
        List<Transform> auxSpawnPositions = SpawnPositions;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            int spawnRand = Random.Range(0, auxSpawnPositions.Count);
            GameObject newWave = Instantiate(propWavePrefabs[Random.Range(0, propWavePrefabs.Count)], GetSafeSpawnAltitude(auxSpawnPositions[spawnRand].position, player.transform.position.y), auxSpawnPositions[spawnRand].rotation);
            //auxSpawnPositions.Remove(auxSpawnPositions[spawnRand]);
            Wave wave = newWave.GetComponent<Wave>();
            wave.AddRenderersToMarker(markers, status, player);
            foreach (AircraftHub hub in wave.aircraft)
            {
				hub.fm.experiencesG = enableBlackouts;
                if (hub.transform.position.y < 0f)
                {
                    hub.transform.position = new Vector3(hub.transform.position.x, Mathf.Abs(hub.transform.position.y), hub.transform.position.z);
                }
            }
        }
    }

    public void PropBonusSpawnWave()
    {
        List<Transform> auxSpawnPositions = SpawnPositions;

        int spawnRand = Random.Range(0, auxSpawnPositions.Count);
        int randomWave = Random.Range(0, propBonusWavePrefabs.Count);
        print("Bonus wave ID is: " + randomWave + ": " + propBonusWavePrefabs[randomWave].name);
        GameObject newWave = Instantiate(propBonusWavePrefabs[randomWave], GetSafeSpawnAltitude(auxSpawnPositions[spawnRand].position, player.transform.position.y), auxSpawnPositions[spawnRand].rotation);
        Wave wave = newWave.GetComponent<Wave>();
        wave.AddRenderersToMarker(markers, status, player);
    }
    public void TrainerSpawnWave(int numberOfEnemies)
    {
        List<Transform> auxSpawnPositions = SpawnPositions;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            int spawnRand = Random.Range(0, auxSpawnPositions.Count);
            GameObject newWave = Instantiate(trainerWavePrefabs[Random.Range(0, trainerWavePrefabs.Count)], GetSafeSpawnAltitude(auxSpawnPositions[spawnRand].position, player.transform.position.y), auxSpawnPositions[spawnRand].rotation);
            //auxSpawnPositions.Remove(auxSpawnPositions[spawnRand]);
            Wave wave = newWave.GetComponent<Wave>();
            wave.AddRenderersToMarker(markers, status, player);
            foreach (AircraftHub hub in wave.aircraft)
            {
				hub.fm.experiencesG = enableBlackouts;
                if (hub.transform.position.y < 0f)
                {
                    hub.transform.position = new Vector3(hub.transform.position.x, Mathf.Abs(hub.transform.position.y), hub.transform.position.z);
                }
            }
        }
    }

    public void PropAlliedSpawnWave()
    {
        GameObject newWave = Instantiate(propWavePrefabs[Random.Range(0, propWavePrefabs.Count)], GetSafeSpawnAltitude(new Vector3(0, 4000f, 0), player.transform.position.y), transform.rotation);
        //auxSpawnPositions.Remove(auxSpawnPositions[spawnRand]);
        Wave wave = newWave.GetComponent<Wave>();
        wave.AddAllyRenderersToMarker(markers, status);
        foreach(AircraftHub hub in wave.aircraft)
        {
			hub.fm.experiencesG = enableBlackouts;
            hub.hp.extraLives = 1;
            if(hub.transform.position.y < 0f)
            {
                hub.transform.position = new Vector3(hub.transform.position.x, Mathf.Abs(hub.transform.position.y), hub.transform.position.z);
            }
        }
    }

    public void Jet1AlliedSpawnWave()
    {
        GameObject newWave = Instantiate(jetTier1WavePrefabs[Random.Range(0, jetTier1WavePrefabs.Count)], GetSafeSpawnAltitude(new Vector3(0, 4000f, 0), player.transform.position.y), transform.rotation);
        //auxSpawnPositions.Remove(auxSpawnPositions[spawnRand]);
        Wave wave = newWave.GetComponent<Wave>();
        wave.AddAllyRenderersToMarker(markers, status);
        foreach (AircraftHub hub in wave.aircraft)
        {
			hub.fm.experiencesG = enableBlackouts;
            hub.hp.extraLives = 1;
			if(hub.irControl != null)
			{
				hub.irControl.canReload = true;
			}
            if (hub.transform.position.y < 0f)
            {
                hub.transform.position = new Vector3(hub.transform.position.x, Mathf.Abs(hub.transform.position.y), hub.transform.position.z);
            }
        }
    }
    public void Jet2AlliedSpawnWave()
    {
        GameObject newWave = Instantiate(jetTier2WavePrefabs[Random.Range(0, jetTier2WavePrefabs.Count)], GetSafeSpawnAltitude(new Vector3(0, 4000f, 0), player.transform.position.y), transform.rotation);
        //auxSpawnPositions.Remove(auxSpawnPositions[spawnRand]);
        Wave wave = newWave.GetComponent<Wave>();
        wave.AddAllyRenderersToMarker(markers, status);
        foreach (AircraftHub hub in wave.aircraft)
        {
			hub.fm.experiencesG = enableBlackouts;			
            hub.hp.extraLives = 1;
			if(hub.irControl != null)
			{
				hub.irControl.canReload = true;
			}
            if (hub.transform.position.y < 0f)
            {
                hub.transform.position = new Vector3(hub.transform.position.x, Mathf.Abs(hub.transform.position.y), hub.transform.position.z);
            }
        }
    }
	
	Vector3 GetSpawnPosition(float alt, float spawnDistance)
	{
		// Point toward world center
		
		Vector3 randPosition = new Vector3(Random.Range(-100f, 100f), 0f, Random.Range(-100f, 100f));
		
		Vector3 directionToCenter = (new Vector3(0f, alt, 0f) - randPosition).normalized;
		
		Vector3 directionFromCenter = directionToCenter * -1f;
		Vector3 spawnPosition = directionFromCenter * spawnDistance;
		spawnPosition = Utilities.GetSafeSpawnAltitude(spawnPosition, alt);
		
		Quaternion rotation = Quaternion.LookRotation(directionToCenter, Vector3.up);

		return spawnPosition;
	
	}

    Vector3 GetSafeSpawnAltitude(Vector3 spawnPoint, float playerAltitude)
    {
        Vector3 rayStart = new Vector3(spawnPoint.x, 10000f, spawnPoint.z);
        Ray ray = new Ray(rayStart, Vector3.down);
        RaycastHit hit;

        float terrainY = 0f;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            terrainY = hit.point.y;
        }
        else
        {
            terrainY = 50f;
        }

        float safeAltitude = terrainY + 500f;

        float finalAltitude = Mathf.Max(safeAltitude, playerAltitude);

        return new Vector3(spawnPoint.x, finalAltitude, spawnPoint.z);
    }
	
	public void CleanLists(int tier)
	{
		// -1 is new matchmaker, 0 is props, 1 is subsonics, 2 is gen2, 3 is gen3, 4 is gen4.
		
		if(tier == (-1))
		{
			allAircraftPrefabs.Clear();
			sortedAircraft.Clear();
			jetTier1WavePrefabs.Clear();
			jetTier2WavePrefabs.Clear();
			jetTier3WavePrefabs.Clear();
			jetTier4WavePrefabs.Clear();
			propWavePrefabs.Clear();
		}
		if(tier == 0)
		{
			jetTier1WavePrefabs.Clear();
			jetTier2WavePrefabs.Clear();
			jetTier3WavePrefabs.Clear();
			jetTier4WavePrefabs.Clear();
			allAircraftPrefabs.Clear();
			sortedAircraft.Clear();
			eligibleAircraft.Clear();
			jetTier1BonusWavePrefabs.Clear();
			jetTier2BonusWavePrefabs.Clear();
			jetTier3BonusWavePrefabs.Clear();
			jetTier4BonusWavePrefabs.Clear();
		}
		
		if(tier == 1)
		{
			
			jetTier2WavePrefabs.Clear();
			jetTier3WavePrefabs.Clear();
			jetTier4WavePrefabs.Clear();
			propWavePrefabs.Clear();
			allAircraftPrefabs.Clear();
			sortedAircraft.Clear();
			eligibleAircraft.Clear();
			propBonusWavePrefabs.Clear();
			
			jetTier2BonusWavePrefabs.Clear();
			jetTier3BonusWavePrefabs.Clear();
			jetTier4BonusWavePrefabs.Clear();
		}
		
		if(tier == 2)
		{
			jetTier1WavePrefabs.Clear();
			jetTier3WavePrefabs.Clear();
			jetTier4WavePrefabs.Clear();
			propWavePrefabs.Clear();
			allAircraftPrefabs.Clear();
			sortedAircraft.Clear();
			eligibleAircraft.Clear();
			propBonusWavePrefabs.Clear();
			jetTier1BonusWavePrefabs.Clear();
			jetTier3BonusWavePrefabs.Clear();
			jetTier4BonusWavePrefabs.Clear();
		}
	
	
		/*
		if(tier == 1)
		{
			jetTier1WavePrefabs.Clear();
			jetTier2WavePrefabs.Clear();
			propWavePrefabs.Clear();
			allAircraftPrefabs.Clear();
			sortedAircraft.Clear();
			eligibleAircraft.Clear();
			propBonusWavePrefabs.Clear();
			jetTier1BonusWavePrefabs.Clear();
			jetTier2BonusWavePrefabs.Clear();
		}
		*/
		
	}
}
