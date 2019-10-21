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
  private GameObject attackNode;

  private int currentPhaseCycle = 1;
  public static PHASES CurrentPhase { get; private set; }

  /////////////////////////////////////////////////////////
  // Preparation phase
  private bool nodeSelected;
  private bool phaseCycleSetup = false;

  private void Awake()
  {
    resourceManager = GetComponent<ResourceManager>();

    conqueredNodes = new List<GameObject>
    {
      fortress
    };

    CurrentPhase = PHASES.PREPARATION;
  }

  private void FixedUpdate()
  {
    phaseText.text = CurrentPhase.ToString() + " PHASE";
    nodeText.text = "Node " + currentPhaseCycle.ToString();
  }

  private void Update()
  {
    switch (CurrentPhase)
    {
      case PHASES.PREPARATION:
        PreparationPhase();
        break;

      case PHASES.ESCORT:
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
      Vector3 camPos = lastConqueredNode.transform.position;
      camPos.y = playerCamera.transform.position.y;
      playerCamera.transform.position = camPos;

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

    GameObject attackingFromNode = conqueredNodes[conqueredNodes.Count - 1];

    // Set the camera to point at the assembly space
    Vector3 setupPos = attackingFromNode.GetComponent<ConqueredNode>().AssemblySpace.transform.position;
    // Retains the camera height
    setupPos.y = playerCamera.transform.position.y;

    playerCamera.transform.position = setupPos;
    playerCamera.GetComponent<Camera>().orthographicSize = 25f;
    // Disable the camera controls
    playerCamera.GetComponent<CameraControls>().enabled = false;
    playerCamera.GetComponent<CameraIssueOrdering>().enabled = false;
    // Set the unit manager assembly space reference
    uiInterface.UnitManager.SetAssemblySpace(attackingFromNode.GetComponent<ConqueredNode>().AssemblySpace);

    // Update the reference to the node we are going to attack
    attackNode = attackingFromNode.GetComponent<CrystalSeekerSpawner>().CrystalTarget;
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
    // Update phase, UI and camera
    CurrentPhase = PHASES.ESCORT;

    uiInterface.PreparationPhaseDisableUI();

    playerCamera.GetComponent<CameraControls>().enabled = true;
    playerCamera.GetComponent<CameraIssueOrdering>().enabled = true;
    playerCamera.GetComponent<CameraManager>().SetNormalView();

    // Disabled the crystal nodes functionalities and spawns a crystal seeker
    GameObject attackingFromNode = conqueredNodes[conqueredNodes.Count - 1];
    GameObject spawnedCrystalSeeker = attackingFromNode.GetComponent<CrystalSeekerSpawner>().SpawnCrystalSeeker();
    attackingFromNode.GetComponent<CrystalSeekerSpawner>().enabled = false;
    attackingFromNode.GetComponent<CrystalOrder>().enabled = false;

    // Change unit panel buttons to combat buttons so clicking on them selects units instead of deleting them
    uiInterface.UnitManager.SetUnitButtonsToCombat();

    // Start wave spawners of attack node
    attackNode.GetComponent<CrystalNode>().SetWaveSpawnersActive(true, spawnedCrystalSeeker);
    // Disables the tree wall so units can pass through
    attackNode.GetComponent<CrystalNode>().DisableTreeWall();

    // Turn off tempFOVMeshes so we can't see the enemies attacking from the path
    attackingFromNode.GetComponent<ConqueredNode>().DisablePreparationFOV();
  }

  public void EscortWin()
  {
    // Advance to the next Preparation Phase
    ++currentPhaseCycle;

    CurrentPhase = PHASES.PREPARATION;

    // Remove all units on the playing field, friendly units are contained in Unit Manager, enemy units are contained in Hideable Manager
    uiInterface.EscortPhaseRemoveAllUnits();
    GetComponent<HideableManager>().RemoveAllUnits();

    // Add the conquered node to the list
    conqueredNodes.Add(attackNode);

    GameObject conqueredNode = conqueredNodes[conqueredNodes.Count - 1];

    // Enable the crystal nodes functionalities and spawns a crystal seeker
    conqueredNode.GetComponent<CrystalSeekerSpawner>().enabled = true;
    conqueredNode.GetComponent<CrystalOrder>().enabled = true;

    // Disable wave spawners of the conquered node
    conqueredNode.GetComponent<CrystalNode>().SetWaveSpawnersActive(false, null);

    // Turn on all the tempFOVMeshes of every conquered nodes 
    for (int i = 0; i < conqueredNodes.Count; ++i)
    {
      conqueredNodes[i].GetComponent<ConqueredNode>().EnablePreparationFOV();
    }

    // Turn the conquered node into your faction
    conqueredNode.GetComponent<Faction>().faction = Faction.FACTIONS.GOBLINS;

    // Update the camera bounds
    playerCamera.GetComponent<CameraControls>().AddCameraBounds(conqueredNode.GetComponent<ConqueredNode>().CameraBound);

    phaseCycleSetup = false;
    nodeSelected = false;
  }

  public void EscortLose()
  {

  }
}
