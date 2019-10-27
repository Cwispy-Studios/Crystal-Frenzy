using UnityEngine;

public class CrystalNode : MonoBehaviour
{
  [SerializeField]
  private ConnectedNodeData[] connectedNodesData = null;

  [SerializeField]
  private GameObject[] spawnPoints = null;
  [SerializeField]
  private GameObject treeWall = null;
  [HideInInspector]
  public GameObject conqueredNode = null;

  // To add disable component in inspector
  private void Start() { }

  public bool CheckCrystalIsValid(GameObject checkObject, ref GameObject setTarget, ref GameObject crystalPath)
  {
    for (int i = 0; i < connectedNodesData.Length; ++i)
    {
      // Check if the crystal selected is connected to this node, and if the crystal does not already belong to the player, 
      // AND the crystal node number must be greater than or equal to this node
      if (connectedNodesData[i].connectedNode == checkObject && checkObject.GetComponent<Faction>().faction != GetComponent<Faction>().faction)
      {
        setTarget = checkObject;
        crystalPath = connectedNodesData[i].pathSpline;

        for (int notSelected = 0; notSelected < connectedNodesData.Length; ++notSelected)
        {
          if (notSelected != i)
          {
            ParticleSystem particleSystem = connectedNodesData[notSelected].pathSpline.GetComponentInChildren<ParticleSystem>();
            var emittor = particleSystem.emission;

            emittor.enabled = false;
            particleSystem.Clear();
          }
        }

        return true;
      }
    }

    return false;
  }

  public GameObject RetrieveSpline(GameObject checkObject)
  {
    for (int i = 0; i < connectedNodesData.Length; ++i)
    {
      // Find the crystal that we were attacking from, and retrieve the spline for that crystal
      if (connectedNodesData[i].connectedNode == checkObject)
      {
        return connectedNodesData[i].pathSpline;
      }
    }

    return null;
  }

  public void SetPathVisibilityMeshes(bool active)
  {
    for (int connectedNodeIndex = 0; connectedNodeIndex < connectedNodesData.Length; ++connectedNodeIndex)
    {
      for (int visibilityMeshIndex = 0; visibilityMeshIndex < connectedNodesData[connectedNodeIndex].pathVisibilityMeshes.Length; ++visibilityMeshIndex)
      {
        connectedNodesData[connectedNodeIndex].pathVisibilityMeshes[visibilityMeshIndex].SetActive(active);
      }
    }
  }

  public void SetConqueredPathVisibilityMeshes(GameObject conqueredNode, bool active)
  {
    for (int connectedNodeIndex = 0; connectedNodeIndex < connectedNodesData.Length; ++connectedNodeIndex)
    {
      // Find the node we conquered and set those visibility paths as active
      if (conqueredNode == connectedNodesData[connectedNodeIndex].connectedNode)
      {
        for (int visibilityMeshIndex = 0; visibilityMeshIndex < connectedNodesData[connectedNodeIndex].pathVisibilityMeshes.Length; ++visibilityMeshIndex)
        {
          connectedNodesData[connectedNodeIndex].pathVisibilityMeshes[visibilityMeshIndex].SetActive(active);
        }

        return;
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
    for (int i = 0; i < connectedNodesData.Length; ++i)
    {
      CrystalRewards crystalRewards = connectedNodesData[i].connectedNode.GetComponent<CrystalRewards>();

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
