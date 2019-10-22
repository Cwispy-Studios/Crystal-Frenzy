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

  public static ResourceManager resourceManager;

  private List<GameObject> conqueredNodes;
  private GameObject attackNode;

  private int currentPhaseCycle = 1;
  public static PHASES CurrentPhase { get; private set; }

  /////////////////////////////////////////////////////////
  // Preparation phase
  private bool nodeSelected;

  private void Awake()
  {
    resourceManager = GetComponent<ResourceManager>();

    conqueredNodes = new List<GameObject>
    {
      fortress
    };

    CurrentPhase = PHASES.PREPARATION;
  }

  private void Start()
  {
    BeginPreparationPhase();
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

      case PHASES.PREPARATION_DEFENSE:
        PreparationDefensePhase();
        break;
    }
  }

  ///////////////////////////////////////////////////////////////
  // PREPARATION FUNCTIONS

  private void PreparationPhase()
  {
    GameObject lastConqueredNode = conqueredNodes[conqueredNodes.Count - 1];

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

  public void BeginPreparationPhase()
  {
    GameObject lastConqueredNode = conqueredNodes[conqueredNodes.Count - 1];

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
  }

  public void NodeSelected()
  {
    nodeSelected = true;

    uiInterface.PreparationPhaseSelectArmyUI(false);

    GameObject attackingFromNode = conqueredNodes[conqueredNodes.Count - 1];

    // Set the camera to point at the assembly space
    Vector3 setupPos = attackingFromNode.GetComponent<ConqueredNode>().AssemblySpace.transform.position;
    // Retains the camera height
    setupPos.y = playerCamera.transform.position.y;

    playerCamera.transform.position = setupPos;
    playerCamera.GetComponent<CameraManager>().SetAssemblyOrthographicSize();
    // Disable the camera controls
    playerCamera.GetComponent<CameraControls>().enabled = false;
    playerCamera.GetComponent<CameraIssueOrdering>().enabled = false;
    // Set the unit manager assembly space reference
    uiInterface.UnitManager.SetAssemblySpace(attackingFromNode.GetComponent<ConqueredNode>().AssemblySpace);

    // Update the reference to the node we are going to attack
    attackNode = attackingFromNode.GetComponent<CrystalSeekerSpawner>().CrystalTarget;

    // Update the loot target panel
    uiInterface.UpdateLootTargetPanel(attackNode.GetComponent<CrystalRewards>().goldLoot, attackNode.GetComponent<CrystalRewards>().crystalIncomeReward);
  }

  public void ReturnToNodeSelection()
  {
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

    GameObject attackingFromNode = conqueredNodes[conqueredNodes.Count - 1];
    attackingFromNode.GetComponent<CrystalNode>().conqueredNode = attackNode;

    // Add the conquered node to the list
    conqueredNodes.Add(attackNode);

    GameObject conqueredNode = conqueredNodes[conqueredNodes.Count - 1];

    // Enable the crystal nodes functionalities
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

    // Collect loot
    resourceManager.CollectLoot(conqueredNode.GetComponent<CrystalRewards>().goldLoot, conqueredNode.GetComponent<CrystalRewards>().crystalIncomeReward);
    uiInterface.UpdateLootTargetPanel(0, 0);

    nodeSelected = false;

    BeginPreparationPhase();
  }

  public void EscortLose()
  {
    CurrentPhase = PHASES.PREPARATION_DEFENSE;

    // Remove all units on the playing field, friendly units are contained in Unit Manager, enemy units are contained in Hideable Manager
    uiInterface.EscortPhaseRemoveAllUnits();
    GetComponent<HideableManager>().RemoveAllUnits();

    // Enable the crystal nodes functionalities
    attackNode.GetComponent<CrystalSeekerSpawner>().enabled = true;
    attackNode.GetComponent<CrystalSeekerSpawner>().SetCrystalTarget(conqueredNodes[conqueredNodes.Count - 1]);
    attackNode.GetComponent<CrystalOrder>().enabled = true;

    // Disable wave spawners of the conquered node
    attackNode.GetComponent<CrystalNode>().SetWaveSpawnersActive(false, null);

    // Turn on all the tempFOVMeshes of every conquered nodes 
    for (int i = 0; i < conqueredNodes.Count; ++i)
    {
      conqueredNodes[i].GetComponent<ConqueredNode>().EnablePreparationFOV();
    }

    BeginPreparationDefensePhase();
  }

  public void BeginPreparationDefensePhase()
  {
    GameObject lastConqueredNode = conqueredNodes[conqueredNodes.Count - 1];

    // Force the camera into a bird's eye view
    playerCamera.GetComponent<CameraManager>().SetBirdsEyeView();
    Vector3 camPos = lastConqueredNode.transform.position;
    camPos.y = playerCamera.transform.position.y;
    playerCamera.transform.position = camPos;

    // Get the latest conquered node and set all the temp FOV mesh to true so we can see the paths and crystal nodes 
    lastConqueredNode.GetComponent<ConqueredNode>().EnablePreparationFOV();

    playerCamera.GetComponent<CameraControls>().enabled = true;
    playerCamera.GetComponent<CameraIssueOrdering>().enabled = true;

    uiInterface.PreparationDefensePhaseSelectArmyUI();

    GameObject attackingFromNode = conqueredNodes[conqueredNodes.Count - 1];

    // Set the camera to point at the assembly space
    Vector3 setupPos = attackingFromNode.GetComponent<ConqueredNode>().AssemblySpace.transform.position;
    // Retains the camera height
    setupPos.y = playerCamera.transform.position.y;

    playerCamera.transform.position = setupPos;
    // Disable the camera controls
    playerCamera.GetComponent<CameraControls>().enabled = false;
    playerCamera.GetComponent<CameraIssueOrdering>().enabled = false;
    // Set the unit manager assembly space reference
    uiInterface.UnitManager.SetAssemblySpace(attackingFromNode.GetComponent<ConqueredNode>().AssemblySpace);

    // Update the loot target panel
    uiInterface.UpdateLootTargetPanel(0, 0);
  }

  private void PreparationDefensePhase()
  {
    GameObject lastConqueredNode = conqueredNodes[conqueredNodes.Count - 1];

    // Army selection roster
    // While it is false, keep the button not interactable
    if (resourceManager.ArmySize == 0)
    {
      uiInterface.PreparationDefensePhaseSetDefendButtonInteractable(false);
    }

    // Once army is selected, button is selectable
    else
    {
      uiInterface.PreparationDefensePhaseSetDefendButtonInteractable(true);
    }
  }

  public void BeginDefense()
  {
    // Update phase, UI and camera
    CurrentPhase = PHASES.DEFENSE;

    uiInterface.PreparationDefensePhaseDisableUI();

    playerCamera.GetComponent<CameraControls>().enabled = true;
    playerCamera.GetComponent<CameraIssueOrdering>().enabled = true;
    playerCamera.GetComponent<CameraManager>().SetNormalView();

    // Disable the crystal nodes functionalities and spawns a crystal seeker
    GameObject spawnedCrystalSeeker = attackNode.GetComponent<CrystalSeekerSpawner>().SpawnCrystalSeeker();
    attackNode.GetComponent<CrystalSeekerSpawner>().enabled = false;

    // Change unit panel buttons to combat buttons so clicking on them selects units instead of deleting them
    uiInterface.UnitManager.SetUnitButtonsToCombat();

    // Start wave spawners of attack node
    attackNode.GetComponent<CrystalNode>().SetWaveSpawnersActive(true, spawnedCrystalSeeker);
    // Disables the tree wall so units can pass through
    attackNode.GetComponent<CrystalNode>().DisableTreeWall();

    // Turn off tempFOVMeshes so we can't see the enemies attacking from the path
    conqueredNodes[conqueredNodes.Count - 1].GetComponent<ConqueredNode>().DisablePreparationFOV();
  }

  public void DefenseWin()
  {
    CurrentPhase = PHASES.PREPARATION;

    // Remove all units on the playing field, friendly units are contained in Unit Manager, enemy units are contained in Hideable Manager
    uiInterface.EscortPhaseRemoveAllUnits();
    GetComponent<HideableManager>().RemoveAllUnits();

    GameObject attackingFromNode = conqueredNodes[conqueredNodes.Count - 1];

    // Enable the crystal nodes functionalities
    attackingFromNode.GetComponent<CrystalSeekerSpawner>().enabled = true;
    attackingFromNode.GetComponent<CrystalOrder>().enabled = true;

    // Disable wave spawners of the conquered node
    attackNode.GetComponent<CrystalNode>().SetWaveSpawnersActive(false, null);

    // Turn on all the tempFOVMeshes of every conquered nodes 
    for (int i = 0; i < conqueredNodes.Count; ++i)
    {
      conqueredNodes[i].GetComponent<ConqueredNode>().EnablePreparationFOV();
    }

    nodeSelected = false;

    BeginPreparationPhase();

    // Check if our attacking from node has already conquered a node. If yes, skip to army selection screen and force player
    // to attack that node. Otherwise, player is free to choose
    if (attackingFromNode.GetComponent<CrystalNode>().conqueredNode != null)
    {
      attackingFromNode.GetComponent<CrystalSeekerSpawner>().SetCrystalTarget(attackingFromNode.GetComponent<CrystalNode>().conqueredNode);
      uiInterface.PreparationPhaseSelectArmyUI(true);
    }
  }

  public void DefenseLose()
  {
    CurrentPhase = PHASES.PREPARATION_DEFENSE;
    --currentPhaseCycle;

    // Remove all units on the playing field, friendly units are contained in Unit Manager, enemy units are contained in Hideable Manager
    uiInterface.EscortPhaseRemoveAllUnits();
    GetComponent<HideableManager>().RemoveAllUnits();

    // Disable wave spawners and crystal nodes functionalities of the attacking node we lost to
    attackNode.GetComponent<CrystalNode>().SetWaveSpawnersActive(false, null);
    attackNode.GetComponent<CrystalSeekerSpawner>().enabled = false;
    attackNode.GetComponent<CrystalOrder>().enabled = false;

    // Update the attacking node to the node we lost and remove that node from our list of conquered nodes
    attackNode = conqueredNodes[conqueredNodes.Count - 1];
    conqueredNodes.RemoveAt(conqueredNodes.Count - 1);

    // Enable the crystal nodes functionalities of the new attacking node
    attackNode.GetComponent<Faction>().faction = Faction.FACTIONS.NEUTRAL;
    attackNode.GetComponent<CrystalSeekerSpawner>().enabled = true;
    attackNode.GetComponent<CrystalSeekerSpawner>().SetCrystalTarget(conqueredNodes[conqueredNodes.Count - 1]);
    attackNode.GetComponent<CrystalOrder>().enabled = true;

    // Disable wave spawners of the new attacking node
    attackNode.GetComponent<CrystalNode>().SetWaveSpawnersActive(false, null);

    // Turn on all the tempFOVMeshes of every conquered nodes 
    for (int i = 0; i < conqueredNodes.Count; ++i)
    {
      conqueredNodes[i].GetComponent<ConqueredNode>().EnablePreparationFOV();
    }

    BeginPreparationDefensePhase();
  }
}
