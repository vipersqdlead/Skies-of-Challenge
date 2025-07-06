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
    [SerializeField] List<Transform> SpawnPositions;
    public EnemyMarkers markers;
    public SurvivalMissionStatus status;

	public int currentDifficulty = 400;

	public void OrderLists()
	{	
		sortedAircraft = allAircraftPrefabs.OrderBy(p => p.GetComponent<Wave>().aircraft[0].gameObject.GetComponent<HealthPoints>().pointsWorth).ToList();
		eligibleAircraft = GetAircraftByDifficultyRange();
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
            GameObject newWave = Instantiate(eligibleAircraft[Random.Range(0, eligibleAircraft.Count)], GetSafeSpawnAltitude(auxSpawnPositions[spawnRand].position, player.transform.position.y), auxSpawnPositions[spawnRand].rotation);
            //auxSpawnPositions.Remove(auxSpawnPositions[spawnRand]);
            Wave wave = newWave.GetComponent<Wave>();
            wave.AddRenderersToMarker(markers, status, player);
            foreach (AircraftHub hub in wave.aircraft)
            {
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
        Destroy(newWave, 150f);
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
        Destroy(newWave, 150f);
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
}
