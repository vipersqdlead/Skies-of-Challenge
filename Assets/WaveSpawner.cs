using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public FlightModel player;
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

    public void JetTier1SpawnWave(int numberOfEnemies)
    {
        List<Transform> auxSpawnPositions = SpawnPositions;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            int spawnRand = Random.Range(0, auxSpawnPositions.Count);
            GameObject newWave = Instantiate(jetTier1WavePrefabs[Random.Range(0, jetTier1WavePrefabs.Count)], auxSpawnPositions[spawnRand].position, auxSpawnPositions[spawnRand].rotation);
            //auxSpawnPositions.Remove(auxSpawnPositions[spawnRand]);
            newWave.GetComponent<Wave>().AddRenderersToMarker(markers, status, player);
        }
    }
    public void JetTier1BonusSpawnWave()
    {
        List<Transform> auxSpawnPositions = SpawnPositions;

        int spawnRand = Random.Range(0, auxSpawnPositions.Count);
        int randomWave = Random.Range(0, jetTier1BonusWavePrefabs.Count);
        print(randomWave);
        GameObject newWave = Instantiate(jetTier1BonusWavePrefabs[randomWave], auxSpawnPositions[spawnRand].position, auxSpawnPositions[spawnRand].rotation);
        //auxSpawnPositions.Remove(auxSpawnPositions[spawnRand]);
        newWave.GetComponent<Wave>().AddRenderersToMarker(markers, status, player);
        Destroy(newWave, 150f);
    }

    public void JetTier2SpawnWave(int numberOfEnemies)
    {
        List<Transform> auxSpawnPositions = SpawnPositions;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            int spawnRand = Random.Range(0, auxSpawnPositions.Count);
            GameObject newWave = Instantiate(jetTier2WavePrefabs[Random.Range(0, jetTier2WavePrefabs.Count)], auxSpawnPositions[spawnRand].position, auxSpawnPositions[spawnRand].rotation);
            //auxSpawnPositions.Remove(auxSpawnPositions[spawnRand]);
            newWave.GetComponent<Wave>().AddRenderersToMarker(markers, status, player);
        }
    }

    public void JetTier2BonusSpawnWave()
    {
        List<Transform> auxSpawnPositions = SpawnPositions;

        int spawnRand = Random.Range(0, auxSpawnPositions.Count);
        int randomWave = Random.Range(0, jetTier2BonusWavePrefabs.Count);
        print(randomWave);
        GameObject newWave = Instantiate(jetTier2BonusWavePrefabs[randomWave], auxSpawnPositions[spawnRand].position, auxSpawnPositions[spawnRand].rotation);
        newWave.GetComponent<Wave>().AddRenderersToMarker(markers, status, player);
        Destroy(newWave, 150f);
    }

    public void PropSpawnWave(int numberOfEnemies)
    {
        List<Transform> auxSpawnPositions = SpawnPositions;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            int spawnRand = Random.Range(0, auxSpawnPositions.Count);
            GameObject newWave = Instantiate(propWavePrefabs[Random.Range(0, propWavePrefabs.Count)], auxSpawnPositions[spawnRand].position, auxSpawnPositions[spawnRand].rotation);
            //auxSpawnPositions.Remove(auxSpawnPositions[spawnRand]);
            newWave.GetComponent<Wave>().AddRenderersToMarker(markers, status, player);
        }
    }

    public void PropBonusSpawnWave()
    {
        List<Transform> auxSpawnPositions = SpawnPositions;

        int spawnRand = Random.Range(0, auxSpawnPositions.Count);
        int randomWave = Random.Range(0, propBonusWavePrefabs.Count);
        print("Bonus wave ID is: " + randomWave);
        GameObject newWave = Instantiate(propBonusWavePrefabs[randomWave], auxSpawnPositions[spawnRand].position, auxSpawnPositions[spawnRand].rotation);
        newWave.GetComponent<Wave>().AddRenderersToMarker(markers, status, player);
        Destroy(newWave, 150f);
        print("Name of wave is: " + newWave.name);
    }
    public void TrainerSpawnWave(int numberOfEnemies)
    {
        List<Transform> auxSpawnPositions = SpawnPositions;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            int spawnRand = Random.Range(0, auxSpawnPositions.Count);
            GameObject newWave = Instantiate(trainerWavePrefabs[Random.Range(0, trainerWavePrefabs.Count)], auxSpawnPositions[spawnRand].position, auxSpawnPositions[spawnRand].rotation);
            //auxSpawnPositions.Remove(auxSpawnPositions[spawnRand]);
            newWave.GetComponent<Wave>().AddRenderersToMarker(markers, status, player);
        }
    }

    public void PropAlliedSpawnWave()
    {
        GameObject newWave = Instantiate(propWavePrefabs[Random.Range(0, propWavePrefabs.Count)], new Vector3(0, 4000f, 0), transform.rotation);
        //auxSpawnPositions.Remove(auxSpawnPositions[spawnRand]);
        newWave.GetComponent<Wave>().AddAllyRenderersToMarker(markers, status);
    }

    public void Jet1AlliedSpawnWave()
    {
        GameObject newWave = Instantiate(jetTier1WavePrefabs[Random.Range(0, jetTier1WavePrefabs.Count)], new Vector3(0, 4000f, 0), transform.rotation);
        //auxSpawnPositions.Remove(auxSpawnPositions[spawnRand]);
        newWave.GetComponent<Wave>().AddAllyRenderersToMarker(markers, status);
    }
    public void Jet2AlliedSpawnWave()
    {
        GameObject newWave = Instantiate(jetTier2WavePrefabs[Random.Range(0, jetTier2WavePrefabs.Count)], new Vector3(0, 4000f, 0), transform.rotation);
        //auxSpawnPositions.Remove(auxSpawnPositions[spawnRand]);
        newWave.GetComponent<Wave>().AddAllyRenderersToMarker(markers, status);
    }
}
