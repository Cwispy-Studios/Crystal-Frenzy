using UnityEngine;

public class CrystalNode : MonoBehaviour
{
  [SerializeField]
  private ConnectedNodeData[] connectedNodesData;

  [SerializeField]
  private GameObject[] connectedNodes = null;
  [SerializeField]
  private GameObject[] pathSplines = null;
  [SerializeField]
  private GameObject[] spawnPoints = null;
  [SerializeField]
  private GameObject treeWall = null;
  [HideInInspector]
  public GameObject conqueredNode = null;

  // To add disable component in inspector
  private void Start() { }

  public void CheckCrystalIsValid(GameObject checkObject, ref GameObject setTarget, ref GameObject crystalPath)
  {
    for (int i = 0; i < connectedNodes.Length; ++i)
    {
      // Check if the crystal selected is connected to this node, and if the crystal does not already belong to the player, 
      // AND the crystal node number must be greater than or equal to this node
      if (connectedNodes[i] == checkObject && checkObject.GetComponent<Faction>().faction != GetComponent<Faction>().faction)
      {
        setTarget = checkObject;
        crystalPath = pathSplines[i];

        for (int notSelected = 0; notSelected < connectedNodes.Length; ++notSelected)
        {
          if (notSelected != i)
          {
            ParticleSystem particleSystem = pathSplines[notSelected].GetComponentInChildren<ParticleSystem>();
            var emittor = particleSystem.emission;

            emittor.enabled = false;
            particleSystem.Clear();
          }
        }

        break;
      }
    }
  }

  public void SetWaveSpawnersActive(bool active, GameObject target)
  {
    foreach (GameObject spawnPoint in spawnPoints)
    {
      spawnPoint.GetComponent<WaveSpawner>().enabled = active;
      spawnPoint.GetComponent<WaveSpawner>().target = target;
    }
  }

  public void DisableTreeWall()
  {
    if (treeWall != null)
    {
      treeWall.SetActive(false);
    }
  }

  public void InitialiseRewards()
  {
    for (int i = 0; i < connectedNodes.Length; ++i)
    {
      CrystalRewards crystalRewards = connectedNodes[i].GetComponent<CrystalRewards>();

      if (crystalRewards && crystalRewards.enabled == false)
      {
        crystalRewards.enabled = true;
      }
    }
  }

  public void AdjustSpawners(float waveSpawnerDifficultyMultiplier)
  {
    for (int i = 0; i < spawnPoints.Length; ++i)
    {
      spawnPoints[i].GetComponent<WaveSpawner>().AdjustDifficulty(waveSpawnerDifficultyMultiplier);
    }
  }
}
