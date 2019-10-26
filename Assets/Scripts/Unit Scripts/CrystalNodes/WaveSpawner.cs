﻿using UnityEngine;
using UnityEngine.AI;

//[ExecuteInEditMode]
public class WaveSpawner : MonoBehaviour
{
  [SerializeField]
  private EnemySpawner[] enemySpawners = null;

  public GameObject target = null;

  private float[] cumulativeSumOfWeights = null;
  private float sumOfWeights = 0;

  private GameObject enemyToSpawn = null;
  private float spawnCountdown = 0;

  private void Awake()
  {
    CalculateSumOfWeights();
    // Automatically spawns the first enemy
    RandomiseSpawn();
    spawnCountdown = 0;
  }

  // Implementation from: https://softwareengineering.stackexchange.com/questions/150616/get-weighted-random-item
  private void CalculateSumOfWeights()
  {
    cumulativeSumOfWeights = new float[enemySpawners.Length];

    float cumulatedSumOfWeights = 0;

    // Stores the culmulative sum of weights of each subsequent spawn weight in an array. When choosing which enemySpawner to spawn, we randomise
    // a number from 0 to the total sum of weights, the corresponding array index of the number which is lower than the randomised number will be
    // the chosen spawned enemy
    for (int i = 0; i < enemySpawners.Length; ++i)
    {
      cumulatedSumOfWeights += enemySpawners[i].weight;

      cumulativeSumOfWeights[i] = cumulatedSumOfWeights;
    }

    sumOfWeights = cumulatedSumOfWeights;
  }

  private void Update()
  {
    // Every frame, we check if we have already randomised an enemy type to spawn
    if (enemyToSpawn == null)
    {
      RandomiseSpawn();
    }

    // Here we countdown to spawn the enemy type chosen
    // We check the countdown timer before we decrement it
    if (spawnCountdown > 0)
    {
      spawnCountdown -= Time.deltaTime;
    }

    // Spawn the unit
    else
    {
      GameObject spawnedEnemy = Instantiate(enemyToSpawn, transform.position, transform.rotation);
      spawnedEnemy.GetComponent<NavMeshAgent>().Warp(transform.position);

      // Set it to attack move towards the crystal seeker position instead of at the crystal seeker
      spawnedEnemy.GetComponent<EnemyAI>().target = target;

      enemyToSpawn = null;
    }
  }

  private void RandomiseSpawn()
  {
    // Here we randomise which type of enemy to spawn as specified in CalculateSumOfWeights()
    float randomNumber = Random.Range(0, sumOfWeights);

    // Find the largest cumulative sum that is smaller than this random number, go from the lowest to highest
    for (int i = 0; i < enemySpawners.Length; ++i)
    {
      if (cumulativeSumOfWeights[i] > randomNumber)
      {
        enemyToSpawn = enemySpawners[i].enemyType;
        spawnCountdown = RandomFromDistribution.RandomNormalDistribution(enemySpawners[i].meanSpawnInterval, enemySpawners[i].sdSpawnInterval);

        break;
      }
    }
  }

  public void AdjustDifficulty(float waveSpawnerDifficultyMultiplier)
  {
    for (int i = 0; i < enemySpawners.Length; ++i)
    {
      enemySpawners[i].meanSpawnInterval /= waveSpawnerDifficultyMultiplier;
      enemySpawners[i].sdSpawnInterval /= waveSpawnerDifficultyMultiplier;
      // Scale the weight as well?
    }
  }
}
