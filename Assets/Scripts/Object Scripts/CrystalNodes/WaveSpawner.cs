using UnityEngine;

//[ExecuteInEditMode]
public class WaveSpawner : MonoBehaviour
{
  [SerializeField]
  private EnemySpawner[] enemySpawners = null;

  private GameObject target = null;

  private void Awake()
  {
    
  }

  private void FixedUpdate()
  {
  }

  private void OnValidate()
  {
    
  }
}
