using System.Collections;
using UnityEngine;

public class CrystalNode : MonoBehaviour
{
  private Color normalColor = new Color(93f / 255f, 191f / 255f, 0);
  private Color corruptedColor = new Color(191f / 255f, 0, 173f / 255f);
  private const float NORMAL_EMISSION = 1.55f;
  private const float CORRUPTED_EMISSION = 2.6f;

  [SerializeField]
  private bool isFortress = false;
  public bool IsFortress { get { return isFortress; } }

  [SerializeField]
  private bool isLifeCrystal = false;
  public bool IsLifeCrystal { get { return isLifeCrystal; } }

  [SerializeField]
  private ConnectedNodeData[] connectedNodesData = null;
  public ConnectedNodeData[] ConnectedNodesData { get { return connectedNodesData; } }

  [SerializeField]
  private GameObject[] spawnPoints = null;
  [HideInInspector]
  public GameObject conqueredNode = null;
  [HideInInspector]
  public bool explored = false,   // Unexplored nodes appear gray
    active = false,               // Active node is the node that you are currently selecting from
    conquerable = false,          // Conquerable are the nodes that are connected to your active node
    targeted = false;             // Targeted nodes are the nodes we are attacking

  // To add disable component in inspector
  private void Start() { }

  private void Update()
  {
    if (GameManager.CurrentPhase == PHASES.PREPARATION && !GameManager.NodeSelected)
    {
      if (CameraObjectSelection.IsObjectSelected(gameObject))
      {
        GameManager.GetActiveNode().GetComponent<CrystalSeekerSpawner>().SetCrystalTarget(gameObject);
      }
    }
  }

  public bool CheckCrystalIsValid(GameObject checkObject, ref GameObject setTarget, ref GameObject crystalPath)
  {
    for (int i = 0; i < connectedNodesData.Length; ++i)
    {
      // Check if the crystal selected is connected to this node, and if the crystal does not already belong to the player, 
      // AND the crystal node number must be greater than or equal to this node
      if (connectedNodesData[i].connectedNode == checkObject && checkObject.GetComponent<Faction>().faction != GetComponent<Faction>().faction)
      {
        // Set the previous node as not targetted if it exists and make the new one the targeted one
        if (setTarget != null)
        {
          setTarget.GetComponent<CrystalNode>().targeted = false;
        }
        
        setTarget = checkObject;
        setTarget.GetComponent<CrystalNode>().targeted = true;
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

  public void SetPathVisibilityMeshes(bool setActive, GameObject onlySetThisNodeVisible = null)
  {
    active = true;
    explored = true;

    for (int connectedNodeIndex = 0; connectedNodeIndex < connectedNodesData.Length; ++connectedNodeIndex)
    {
      if (onlySetThisNodeVisible == null)
      {
        connectedNodesData[connectedNodeIndex].connectedNode.GetComponent<CrystalNode>().explored = true;
        connectedNodesData[connectedNodeIndex].connectedNode.GetComponent<CrystalNode>().conquerable = true;

        for (int visibilityMeshIndex = 0; visibilityMeshIndex < connectedNodesData[connectedNodeIndex].pathVisibilityMeshes.Length; ++visibilityMeshIndex)
        {
          connectedNodesData[connectedNodeIndex].pathVisibilityMeshes[visibilityMeshIndex].SetActive(setActive);
        }
      }

      else if (connectedNodesData[connectedNodeIndex].connectedNode = onlySetThisNodeVisible)
      {
        connectedNodesData[connectedNodeIndex].connectedNode.GetComponent<CrystalNode>().explored = true;
        connectedNodesData[connectedNodeIndex].connectedNode.GetComponent<CrystalNode>().conquerable = true;

        for (int visibilityMeshIndex = 0; visibilityMeshIndex < connectedNodesData[connectedNodeIndex].pathVisibilityMeshes.Length; ++visibilityMeshIndex)
        {
          connectedNodesData[connectedNodeIndex].pathVisibilityMeshes[visibilityMeshIndex].SetActive(setActive);
        }

        return;
      }
    }
  }

  public void SetConqueredPathVisibilityMeshes(GameObject conqueredNode, bool setActive)
  {
    for (int connectedNodeIndex = 0; connectedNodeIndex < connectedNodesData.Length; ++connectedNodeIndex)
    {
      // Find the node we conquered and set those visibility paths as active
      if (conqueredNode == connectedNodesData[connectedNodeIndex].connectedNode)
      {
        for (int visibilityMeshIndex = 0; visibilityMeshIndex < connectedNodesData[connectedNodeIndex].pathVisibilityMeshes.Length; ++visibilityMeshIndex)
        {
          connectedNodesData[connectedNodeIndex].pathVisibilityMeshes[visibilityMeshIndex].SetActive(setActive);
        }

        return;
      }
    }
  }

  public GameObject RetrieveCameraBound(GameObject attackingNode)
  {
    for (int connectedNodeIndex = 0; connectedNodeIndex < connectedNodesData.Length; ++connectedNodeIndex)
    {
      // Find the node we conquered and set those visibility paths as active
      if (attackingNode == connectedNodesData[connectedNodeIndex].connectedNode)
      {
        return connectedNodesData[connectedNodeIndex].cameraBound;
      }
    }

    return null;
  }

  public void SetWaveSpawnersActive(bool active, GameObject target)
  {
    foreach (GameObject spawnPoint in spawnPoints)
    {
      spawnPoint.GetComponent<WaveSpawner>().enabled = active;
      spawnPoint.GetComponent<WaveSpawner>().target = target;
    }
  }

  public void DisableTreeWall(GameObject attackedNode)
  {
    for (int connectedNodeIndex = 0; connectedNodeIndex < connectedNodesData.Length; ++connectedNodeIndex)
    {
      // Find the node we are attacking and cut down their tree
      if (attackedNode == connectedNodesData[connectedNodeIndex].connectedNode)
      {
        if (connectedNodesData[connectedNodeIndex].treeWall != null)
        {
          connectedNodesData[connectedNodeIndex].treeWall.SetActive(false);
        }
      }
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

  public void SetConnectedNodesUnconquerable(GameObject conqueredNode)
  {
    active = false;
    targeted = false;
    conqueredNode.GetComponent<CrystalNode>().active = true;

    for (int i = 0; i < ConnectedNodesData.Length; ++i)
    {
      if (conqueredNode != connectedNodesData[i].connectedNode)
      {
        connectedNodesData[i].connectedNode.GetComponent<CrystalNode>().conquerable = false;
      }
    }
  }

  public void SetCrystalColour(bool corrupted)
  {
    if (!corrupted)
    {
      StartLerpColour(corruptedColor, NORMAL_EMISSION, CORRUPTED_EMISSION);
    }

    else
    {
      StartLerpColour(normalColor, CORRUPTED_EMISSION, NORMAL_EMISSION);
    }
  }

  private IEnumerator LerpColour(Renderer fromRenderer, Color fromColour, Color toColour, float fromEmission, float toEmission)
  {
    float startTime = Time.time;
    float duration = 2f;

    while (Time.time - startTime < duration)
    {
      fromRenderer.material.SetColor("_EmissionColor", Color.Lerp(fromColour * fromEmission, toColour * toEmission, (Time.time - startTime) / duration));

      yield return 1;
    }

    fromRenderer.material.SetColor("_EmissionColor", toColour * toEmission);
  }

  private Coroutine StartLerpColour(Color toColour, float fromEmission, float toEmission)
  {
    return StartCoroutine(LerpColour(GetComponent<Renderer>(), GetComponent<Renderer>().material.color, toColour, fromEmission, toEmission));
  }
}
