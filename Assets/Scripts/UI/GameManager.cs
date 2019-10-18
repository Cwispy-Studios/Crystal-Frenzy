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

  [Header("UI Interfaces")]
  [SerializeField]
  private UIInterface uiInterface = null;

  private static ResourceManager resourceManager;

  private List<GameObject> conqueredNodes; 

  private int currentPhaseCycle = 0;
  public static PHASES currentPhase { get; private set; }

  /////////////////////////////////////////////////////////
  // Preparation phase
  private bool nodeSelected;
  private bool phaseCycleSetup = false;

  private void Awake()
  {
    resourceManager = GetComponent<ResourceManager>();
    conqueredNodes = new List<GameObject>();
    conqueredNodes.Add(fortress);
    currentPhase = PHASES.PREPARATION;
  }

  private void FixedUpdate()
  {
    phaseText.text = currentPhase.ToString() + " PHASE";
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

  ///////////////////////////////////////////////////////////////
  // PREPARATION FUNCTIONS

  private void PreparationPhase()
  {
    GameObject lastConqueredNode = conqueredNodes[conqueredNodes.Count - 1];

    if (phaseCycleSetup == false)
    {
      // Force the camera into a bird's eye view
      playerCamera.GetComponent<CameraManager>().SetBirdsEyeView();

      // Get the latest conquered node and set all the temp FOV mesh to true so we can see the paths and crystal nodes 
      lastConqueredNode.GetComponent<ConqueredNode>().EnablePreparationFOV();

      // Update the camera bounds
      playerCamera.GetComponent<CameraControls>().AddCameraBounds(lastConqueredNode.GetComponent<ConqueredNode>().CameraBound);

      // Set the UI Interfaces to invisible and show the button to select army roster
      uiInterface.PreparationPhaseSelectNodeUI();

      playerCamera.GetComponent<CameraControls>().enabled = true;
      playerCamera.GetComponent<CameraIssueOrdering>().enabled = true;

      phaseCycleSetup = true;
    }

    if (nodeSelected == false)
    {
      // While it is false, keep the button not interactable
      if (lastConqueredNode.GetComponent<CrystalSeekerSpawner>().crystalSelected == false)
      {
        uiInterface.PreparationPhaseSetSelectArmyButtonInteractable(false);
      }

      // Once crystal is selected, button is selectable
      else
      {
        uiInterface.PreparationPhaseSetSelectArmyButtonInteractable(true);
      }
    }

    // Army selection roster
    else
    {
      // While it is false, keep the button not interactable
      if (resourceManager.ArmySize == 0)
      {
        uiInterface.PreparationPhaseSetAttackButtonInteractable(false);
      }

      // Once crystal is selected, button is selectable
      else
      {
        uiInterface.PreparationPhaseSetAttackButtonInteractable(true);
      }
    }
  }

  public void NodeSelected()
  {
    nodeSelected = true;

    uiInterface.PreparationPhaseSelectArmyUI();

    // Set the camera to point at the assembly space
    Vector3 setupPos = conqueredNodes[conqueredNodes.Count - 1].GetComponent<ConqueredNode>().AssemblySpace.transform.position;
    // Retains the camera height
    setupPos.y = playerCamera.transform.position.y;

    playerCamera.transform.position = setupPos;
    playerCamera.GetComponent<Camera>().orthographicSize = 25f;
    // Disable the camera controls
    playerCamera.GetComponent<CameraControls>().enabled = false;
    playerCamera.GetComponent<CameraIssueOrdering>().enabled = false;
    // Set the unit manager assembly space reference
    uiInterface.SetUnitManagerAssemblySpace(conqueredNodes[conqueredNodes.Count - 1].GetComponent<ConqueredNode>().AssemblySpace);
  }

  public void ReturnToNodeSelection()
  {
    phaseCycleSetup = false;
    nodeSelected = false;
  }

  ///////////////////////////////////////////////////////////////
  // ESCORT FUNCTIONS
  public void BeginEscort()
  {
    currentPhase = PHASES.ESCORT;

    uiInterface.PreparationPhaseDisableUI();

    playerCamera.GetComponent<CameraControls>().enabled = true;
    playerCamera.GetComponent<CameraIssueOrdering>().enabled = true;
    playerCamera.GetComponent<CameraManager>().SetNormalView();

    // TODO: Change unit panel buttons to combat buttons
    // TODO: Start wave spawner (also create wave spawner)
    // TODO: Turn off tempFOVMeshes
    // TODO: Spawn crystal seeker
  }
}
