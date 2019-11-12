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

  [SerializeField]
  private Material highlightedMaterial = null;
  private Material normalMaterial;
  private bool isBlinking = false;
  private bool isHighlighted = false;

  [FMODUnity.EventRef]
  public string validCrystalSelectonSound = "";

  private GameManager gameManager;
  private Camera playerCamera;

  private void Awake()
  {
    gameManager = FindObjectOfType<GameManager>();
    playerCamera = Camera.main;
  }

  // To add disable component in inspector
  private void Start() { }

  private void Update()
  {
    if (gameManager.CurrentPhase == PHASES.PREPARATION && !gameManager.NodeSelected)
    {
      if (!targeted && playerCamera.GetComponent<CameraObjectSelection>().IsObjectSelected(gameObject))
      {
        gameManager.GetActiveNode().GetComponent<CrystalSeekerSpawner>().SetCrystalTarget(gameObject);
      }

      if (conquerable && !GetComponent<ConqueredNode>().conquered)
      {
        if (!isBlinking && !isHighlighted)
        {
          StartBlinking();
        }
      }

      else if (isBlinking)
      {
        StopBlinking();
      }
    }

    else
    {
      if (isHighlighted)
      {
        HighlightCrystalNode(false);
      }

      if (isBlinking)
      {
        StopBlinking();
      }      
    }
  }

  public bool CheckCrystalIsValid(GameObject checkObject, ref GameObject setTarget, ref GameObject crystalPath)
  {
    for (int i = 0; i < connectedNodesData.Length; ++i)
    {
      // Check if the crystal selected is connected to this node, and if the crystal does not already belong to the player
      if (connectedNodesData[i].connectedNode == checkObject && checkObject.GetComponent<Faction>().faction != GetComponent<Faction>().faction)
      {
        // Set the previous node as not targeted if it exists and make the new one the targeted one
        if (setTarget != null)
        {
          CrystalNode previousCrystalNode = setTarget.GetComponent<CrystalNode>();

          if (previousCrystalNode.isHighlighted)
          {
            previousCrystalNode.HighlightCrystalNode(false);
          }

          if (!previousCrystalNode.isBlinking)
          {
            previousCrystalNode.StartBlinking();
          }

          previousCrystalNode.targeted = false;
        }
        
        // Update the crystal node target
        setTarget = checkObject;

        CrystalNode targetedCrystalNode = setTarget.GetComponent<CrystalNode>();
        targetedCrystalNode.targeted = true;

        if (targetedCrystalNode.isBlinking)
        {
          targetedCrystalNode.StopBlinking();
        }

        if (!targetedCrystalNode.isHighlighted)
        {
          targetedCrystalNode.HighlightCrystalNode(true);
        }

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

        FMODUnity.RuntimeManager.PlayOneShot(validCrystalSelectonSound);

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

  public void SetWaveSpawnersActive(bool active, GameObject target, CrystalPath path)
  {
    foreach (GameObject spawnPoint in spawnPoints)
    {
      spawnPoint.GetComponent<WaveSpawner>().enabled = active;
      spawnPoint.GetComponent<WaveSpawner>().target = target;
      spawnPoint.GetComponent<WaveSpawner>().crystalPath = path;
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
    if (corrupted)
    {
      StartLerpColour(corruptedColor, NORMAL_EMISSION, CORRUPTED_EMISSION);
    }

    else
    {
      StartLerpColour(normalColor, CORRUPTED_EMISSION, NORMAL_EMISSION);
    }
  }

  private IEnumerator LerpColour(Material changeMaterial, Color fromColour, Color toColour, float fromEmission, float toEmission)
  {
    float startTime = Time.time;
    float duration = 2f;
    
    while (Time.time - startTime < duration)
    {
      changeMaterial.SetColor("_EmissionColor", Color.Lerp(fromColour * fromEmission, toColour * toEmission, (Time.time - startTime) / duration));

      yield return 1;
    }

    changeMaterial.SetColor("_EmissionColor", toColour * toEmission);
  }

  private Coroutine StartLerpColour(Color toColour, float fromEmission, float toEmission)
  {
    Material crystalMaterial = GetComponent<Renderer>().material;

    return StartCoroutine(LerpColour(crystalMaterial, GetComponent<Renderer>().material.GetColor("_EmissionColor"), toColour, fromEmission, toEmission));
  }

  private void StartBlinking()
  {
    if (isBlinking)
    {
      StopBlinking();
    }

    isBlinking = true;
    normalMaterial = GetComponent<Renderer>().material;
    StartCoroutine("TargetBlink");
  }

  private void StopBlinking()
  {
    isBlinking = false;
    GetComponent<Renderer>().material = normalMaterial;
    StopAllCoroutines();
  }

  IEnumerator TargetBlink()
  {
    bool highlighted = false;

    while (true)
    {
      switch (highlighted)
      {
        case false:
          GetComponent<Renderer>().material = highlightedMaterial;
          highlighted = true;

          yield return new WaitForSeconds(0.25f);
          break;

        case true:
          GetComponent<Renderer>().material = normalMaterial;
          highlighted = false;

          yield return new WaitForSeconds(0.25f);
          break;
      }
    }
  }

  private void HighlightCrystalNode(bool highlight)
  {
    isHighlighted = highlight;
    GetComponent<Renderer>().material = highlight ? highlightedMaterial : normalMaterial;
  }
}
