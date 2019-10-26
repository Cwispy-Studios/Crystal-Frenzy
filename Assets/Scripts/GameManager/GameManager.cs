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
  public static BuildingManager buildingManager;
  public static UpgradeManager upgradeManager;

  private List<GameObject> conqueredNodes;
  private GameObject attackNode;

  // Enemies get stronger each round
  public static int CurrentRound { get; private set; } = 0;
  public static PHASES CurrentPhase { get; private set; }

  /////////////////////////////////////////////////////////
  // Preparation phase
  private bool nodeSelected;

  private void Awake()
  {
    resourceManager = GetComponent<ResourceManager>();
    buildingManager = GetComponent<BuildingManager>();
    upgradeManager = GetComponent<UpgradeManager>();

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
    phaseText.text = CurrentPhase.ToString().Replace("_", " ") + " PHASE";
    nodeText.text = "Node " + CurrentRound.ToString();
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
    CurrentPhase = PHASES.PREPARATION;

    ++CurrentRound;

    GameObject lastConqueredNode = conqueredNodes[conqueredNodes.Count - 1];

    // Force the camera into a bird's eye view
    playerCamera.GetComponent<CameraManager>().SetBirdsEyeView(lastConqueredNode.transform.position);

    // Get the latest conquered node and set all the temp FOV mesh to true so we can see the paths and crystal nodes 
    lastConqueredNode.GetComponent<ConqueredNode>().EnablePreparationFOV();

    // Update the camera bounds
    playerCamera.GetComponent<CameraControls>().AddCameraBounds(lastConqueredNode.GetComponent<ConqueredNode>().CameraBound);

    // Set the UI Interfaces to invisible and show the button to select army roster
    uiInterface.PreparationPhaseSelectNodeUI();

    playerCamera.GetComponent<CameraControls>().enabled = true;
    playerCamera.GetComponent<CameraIssueOrdering>().enabled = true;
  }

  public void BeginArmySelection()
  {
    nodeSelected = true;

    uiInterface.PreparationPhaseSelectArmyUI(false);

    GameObject attackingFromNode = conqueredNodes[conqueredNodes.Count - 1];

    // Set the camera to point at the assembly space
    playerCamera.GetComponent<CameraManager>().PointCameraAtAssembly(attackingFromNode.GetComponent<ConqueredNode>().AssemblySpace.transform.position);
    // Disable the camera ordering
    playerCamera.GetComponent<CameraIssueOrdering>().enabled = false;
    // Set the unit manager assembly space reference so that selecting our troops spawns them in the assembly space
    uiInterface.UnitManager.SetAssemblySpace(attackingFromNode.GetComponent<ConqueredNode>().AssemblySpace);

    // Update the reference to the node we are going to attack
    attackNode = attackingFromNode.GetComponent<CrystalSeekerSpawner>().CrystalTarget;

    // Update the loot target panel
    uiInterface.UpdateLootTargetPanel(attackNode.GetComponent<CrystalRewards>().goldLoot, attackNode.GetComponent<CrystalRewards>().crystalIncomeReward);
  }

  public void ReturnToNodeSelection()
  {
    nodeSelected = false;

    GameObject lastConqueredNode = conqueredNodes[conqueredNodes.Count - 1];

    // Force the camera into a bird's eye view
    playerCamera.GetComponent<CameraManager>().SetBirdsEyeView(lastConqueredNode.transform.position);

    // Set the UI Interfaces to invisible and show the button to select army roster
    uiInterface.PreparationPhaseSelectNodeUI();

    playerCamera.GetComponent<CameraIssueOrdering>().enabled = true;
  }

  ///////////////////////////////////////////////////////////////
  // ESCORT FUNCTIONS
  public void BeginEscort()
  {
    // Update phase, UI and camera
    CurrentPhase = PHASES.ESCORT;

    uiInterface.PreparationPhaseDisableUI();

    playerCamera.GetComponent<CameraIssueOrdering>().enabled = true;

    // Disabled the crystal nodes functionalities and spawns a crystal seeker
    GameObject attackingFromNode = conqueredNodes[conqueredNodes.Count - 1];
    GameObject spawnedCrystalSeeker = attackingFromNode.GetComponent<CrystalSeekerSpawner>().SpawnCrystalSeeker();
    attackingFromNode.GetComponent<CrystalSeekerSpawner>().enabled = false;
    attackingFromNode.GetComponent<CrystalOrder>().enabled = false;

    BuildingSlot buildingSlot = attackNode.GetComponent<BuildingSlot>();

    // If attack node has a building slot, deactivate it
    if (buildingSlot.enabled == true)
    {
      buildingSlot.inControl = false;
    }

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
    // Remove all units on the playing field, friendly units are contained in Unit Manager, enemy units are contained in Hideable Manager
    uiInterface.EscortPhaseRemoveAllUnits();
    GetComponent<HideableManager>().RemoveAllUnits();

    GameObject attackingFromNode = conqueredNodes[conqueredNodes.Count - 1];
    attackingFromNode.GetComponent<CrystalNode>().conqueredNode = attackNode;

    // Add the conquered node to the list
    conqueredNodes.Add(attackNode);

    GameObject conqueredNode = conqueredNodes[conqueredNodes.Count - 1];

    resourceManager.CollectLoot(conqueredNode.GetComponent<CrystalRewards>().goldLoot,
      conqueredNode.GetComponent<CrystalRewards>().crystalIncomeReward,
      conqueredNode.GetComponent<ConqueredNode>().conquered);

    conqueredNode.GetComponent<ConqueredNode>().conquered = true;

    // Enable the crystal nodes functionalities
    conqueredNode.GetComponent<CrystalSeekerSpawner>().enabled = true;
    conqueredNode.GetComponent<CrystalOrder>().enabled = true;

    // Disable wave spawners of the conquered node
    conqueredNode.GetComponent<CrystalNode>().SetWaveSpawnersActive(false, null);

    BuildingSlot buildingSlot = conqueredNode.GetComponent<BuildingSlot>();

    // If conquered node has a building slot, activate it
    if (buildingSlot.enabled == true)
    {
      buildingSlot.inControl = true;

      // Check if a building has already been constructed, if yes, recapture it
      if (buildingSlot.Constructed)
      {
        // Sets the faction correctly
        buildingSlot.RecaptureBuilding();
      }
    }

    // Turn on all the tempFOVMeshes of every conquered nodes 
    for (int i = 0; i < conqueredNodes.Count; ++i)
    {
      conqueredNodes[i].GetComponent<ConqueredNode>().EnablePreparationFOV();
    }

    // Turn the conquered node into your faction
    conqueredNode.GetComponent<Faction>().faction = Faction.FACTIONS.GOBLINS;

    // Update the camera bounds
    playerCamera.GetComponent<CameraControls>().AddCameraBounds(conqueredNode.GetComponent<ConqueredNode>().CameraBound);

    uiInterface.UpdateLootTargetPanel(0, 0);

    // Check if our newly conquered node has already conquered a node. If yes, skip to army selection screen and force player
    // to attack that node. Otherwise, player is free to choose
    if (conqueredNode.GetComponent<CrystalNode>().conqueredNode != null)
    {
      attackingFromNode.GetComponent<CrystalSeekerSpawner>().SetCrystalTarget(attackingFromNode.GetComponent<CrystalNode>().conqueredNode);
      uiInterface.PreparationPhaseSelectArmyUI(true);
      nodeSelected = true;
    }

    else
    {
      // If capturing a node that has a crystal selected, we reset it to null
      if (conqueredNode.GetComponent<CrystalSeekerSpawner>().crystalSelected == true)
      {
        conqueredNode.GetComponent<CrystalSeekerSpawner>().ResetCrystalSelection();
      }
    }

    nodeSelected = false;

    BeginPreparationPhase();
  }

  public void EscortLose()
  {
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

  ///////////////////////////////////////////////////////////////
  // PREPARATION DEFENSE FUNCTIONS
  public void BeginPreparationDefensePhase()
  {
    CurrentPhase = PHASES.PREPARATION_DEFENSE;

    ++CurrentRound;

    GameObject attackingFromNode = conqueredNodes[conqueredNodes.Count - 1];

    // Set the camera to point at the assembly space
    playerCamera.GetComponent<CameraManager>().PointCameraAtAssembly(attackingFromNode.GetComponent<ConqueredNode>().AssemblySpace.transform.position);

    // Get the latest conquered node and set all the temp FOV mesh to true so we can see the paths and crystal nodes 
    attackingFromNode.GetComponent<ConqueredNode>().EnablePreparationFOV();

    playerCamera.GetComponent<CameraIssueOrdering>().enabled = true;

    uiInterface.PreparationDefensePhaseSelectArmyUI();
    
    // Disable the camera ordering
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

  ///////////////////////////////////////////////////////////////
  // DEFENSE FUNCTIONS
  public void BeginDefense()
  {
    // Update phase, UI and camera
    CurrentPhase = PHASES.DEFENSE;

    uiInterface.PreparationDefensePhaseDisableUI();

    playerCamera.GetComponent<CameraIssueOrdering>().enabled = true;

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
      nodeSelected = true;
    }
  }

  public void DefenseLose()
  {
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

    // Lose the crystal income from that node
    resourceManager.LoseIncome(attackNode.GetComponent<CrystalRewards>().crystalIncomeReward);

    BuildingSlot buildingSlot = attackNode.GetComponent<BuildingSlot>();

    // If the node we lost has a building slot, we flag it as lost control
    if (buildingSlot.enabled == true)
    {
      buildingSlot.inControl = false;

      // Check if a building has already been constructed, if yes, lose it
      if (buildingSlot.Constructed)
      {
        // Sets the faction correctly
        buildingSlot.LoseBuilding();
      }
    }

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
