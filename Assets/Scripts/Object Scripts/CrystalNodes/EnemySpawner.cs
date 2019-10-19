using UnityEngine;

[System.Serializable]
public class EnemySpawner
{
  public GameObject enemyType;
  public float meanSpawnInterval;
  public float sdSpawnInterval;
  public float weight;
}
