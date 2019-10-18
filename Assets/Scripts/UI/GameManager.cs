using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
  [Header("Text")]
  [SerializeField]
  private Text phaseText = null, nodeText = null;

  [Header("Scene Objects")]
  [SerializeField]
  private GameObject playerCamera = null, fortress = null;

  private static ResourceManager resourceManager;

  private List<GameObject> conqueredNodes; 

  private int currentPhaseCycle = 0;
  public static PHASES currentPhase { get; private set; }

  // Preparation phase
  private bool nodeSelected;

  private void Awake()
  {
    resourceManager = GetComponent<ResourceManager>();
    conqueredNodes = new List<GameObject>();
    conqueredNodes.Add(fortress);
    currentPhase = PHASES.PREPARATION;
  }

  private void Update()
  {
    switch (currentPhase)
    {
      case PHASES.PREPARATION:
        PreparationPhase();
        break;

      case PHASES.ESCORT:
        break;

      case PHASES.FORTIFICATION:
        break;

      case PHASES.DEFENSE:
        break;
    }
  }

  private void FixedUpdate()
  {
    phaseText.text = currentPhase.ToString() + " PHASE";
  }

  private void PreparationPhase()
  {
    if (nodeSelected == false)
    {
      // Force the camera into a bird's eye view
      playerCamera.GetComponent<CameraManager>().SetBirdsEyeView();

      // Get the latest conquered node and set all the temp FOV mesh to true so we can see the paths and crystal nodes 
      conqueredNodes[conqueredNodes.Count - 1].GetComponent<ConqueredNode>().EnablePreparationFOV();
    }
    // Force the camera to look at the unit spawn area
    
  }
}
