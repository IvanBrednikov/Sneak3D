using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject fishPrefab;
    GameObject fish;
    [SerializeField]
    bool fishSpawns;
    [SerializeField]
    float spawnDistance = 1f;

    bool delayTimerIsActive = false;
    float maxSpawnDelay = 1f;
    float minSpawnDelay = 0.2f;
    float spawnDelayTimer;

    bool waitTimerActive = false;
    [SerializeField]
    float waitTime = 5f;
    float fishDestroyWaitTimer;

    private void Start()
    {
        if (fishSpawns)
            SpawnFish();
    }

    float FishSpawnDelay
    {
        get 
        { 
            return Random.Range(minSpawnDelay, maxSpawnDelay); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == fish)
        {
            Destroy(fish);
            delayTimerIsActive = true;
            spawnDelayTimer = FishSpawnDelay;
            waitTimerActive = false;
        }
    }

    private void Update()
    {
        if (delayTimerIsActive)
        {
            spawnDelayTimer -= Time.deltaTime;

            if (spawnDelayTimer <= 0)
            {
                delayTimerIsActive = false;
                SpawnFish();
            }
        }
        
        if(waitTimerActive)
        {
            fishDestroyWaitTimer -= Time.deltaTime;

            if(fishDestroyWaitTimer <= 0)
            {
                fishSpawns = false;
                gameObject.SetActive(false);
            }
        }
    }

    void SpawnFish()
    {
        fish = Instantiate(fishPrefab);
        float angle = Random.Range(0, 360);
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
        Vector3 localPosition = rotation * Vector3.forward * spawnDistance;
        Vector3 globalPosition = transform.TransformPoint(localPosition);
        globalPosition.y += 1f;
        fish.transform.position = globalPosition;
        fish.GetComponent<FishAI>().fishSpawner = transform;

        waitTimerActive = true;
        fishDestroyWaitTimer = waitTime;
    }
}
