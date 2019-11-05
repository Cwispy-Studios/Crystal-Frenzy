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

  [Header("Loot Reward Panel")]
  [SerializeField]
  private LootRewardPanel lootRewardPanel = null;

  [Header("Pause Menu Panel")]
  [SerializeField]
  private PauseMenu pauseMenuPanel = null;

  [Header("Miner and Bush Information Canvas")]
  [SerializeField]
  private GameObject minerCanvas = null;
  [SerializeField]
  private GameObject bushCanvas = null;

  public static ResourceManager resourceManager;
  public static BuildingManager buildingManager;
  public static UpgradeManager upgradeManager;
  public static MinerManager minerManager;
  public static BushManager bushManager;
  public static TutorialManager tutorialManager;

  private List<GameObject> conqueredNodes;
  private GameObject attackNode;

  // Enemies get stronger each round
  public int CurrentRound { get; private set; } = 0;
  public PHASES CurrentPhase { get; private set; }

  /////////////////////////////////////////////////////////
  // Preparation phase
  public bool NodeSelected { get; private set; }

  private GameObject crystalSeekerToDestroy;

  private FMODUnity.StudioEventEmitter musicEmitter;

  private FMOD.Studio.Bus Master;

  private void Awake()
  {
    resourceManager = GetComponent<ResourceManager>();
    buildingManager = GetComponent<BuildingManager>();
    upgradeManager = GetComponent<UpgradeManager>();
    minerManager = GetComponent<MinerManager>();
    bushManager = GetComponent<BushManager>();
    tutorialManager = GetComponent<TutorialManager>();

    conqueredNodes = new List<GameObject>
    {
      fortress
    };

    CurrentPhase = PHASES.PREPARATION;

    Master = FMODUnity.RuntimeManager.GetBus("bus:/Master");

    Master.setVolume(1);

    musicEmitter = Camera.main.GetComponent<FMODUnity.StudioEventEmitter>();
  }

  private void Start()
  {
    BeginPreparationPhase(true);
    tutorialManager.StartTutorial();
  }

  private void FixedUpdate()
  {
    phaseText.text = CurrentPhase.ToString().Replace("_", " ") + " PHASE";
    nodeText.text = "Round " + CurrentRound.ToString();
  }

  private void Update()
  {
    if (pauseMenuPanel.gameObject.activeSelf == false && Input.GetKeyDown(KeyCode.Escape))
    {
      uiInterface.GetComponent<CanvasGroup>().interactable = false;
      pauseMenuPanel.gameObject.SetActive(true);
    }

    else if (pauseMenuPanel.gameObject.activeSelf)
    {
      return;
    }

    if (tutorialManager.DisablePhaseProgression)
    {
      return;
    }

    else
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
  }

  ///////////////////////////////////////////////////////////////
  // PREPARATION FUNCTIONS

  public void BeginPreparationPhase(bool allowSelection)
  {
    CurrentPhase = PHASES.PREPARATION;

    musicEmitter.SetParameter("Phases", (int)CurrentPhase);

    ++CurrentRound;

    minerCanvas.GetComponent<CanvasGroup>().alpha = 1;
    bushCanvas.GetComponent<CanvasGroup>().alpha = 0;

    GameObject lastConqueredNode = conqueredNodes[conqueredNodes.Count - 1];

    playerCamera.GetComponent<CameraIssueOrdering>().enabled = false;
    playerCamera.GetComponent<CameraControls>().enabled = true;

    // Get the latest conquered node and enable the assembly FOV mesh so we can see the conquered node area so we can see the paths and crystal nodes 
    lastConqueredNode.GetComponent<ConqueredNode>().SetAssemblyFOV(true);

    if (allowSelection)
    {
      // Force the camera into a bird's eye view
      playerCamera.GetComponent<CameraManager>().PointCameraAtPosition(lastConqueredNode.transform.position, true, false);

      // Turn on the path visibility meshes to all the connected nodes we can attack so we can see the path and our options
      lastConqueredNode.GetComponent<CrystalNode>().SetPathVisibilityMeshes(true);
    }

    else
    {
      // Turn on the path visibility meshes to only the connected conquered nodes
      lastConqueredNode.GetComponent<CrystalNode>().SetConqueredPathVisibilityMeshes(lastConqueredNode.GetComponent<CrystalNode>().conqueredNode, true);
    }

    // Update the camera bounds
    playerCamera.GetComponent<CameraControls>().AddCameraBounds(lastConqueredNode.GetComponent<ConqueredNode>().SelectionCameraBound);

    // Set the UI Interfaces to invisible and show the button to select army roster
    uiInterface.PreparationPhaseSelectNodeUI();

    uiInterface.UpdateUINodeColours();
  }

  private void PreparationPhase()
  {
    GameObject lastConqueredNode = conqueredNodes[conqueredNodes.Count - 1];

    if (NodeSelected == false)
    {
      uiInterface.UpdateUINodeColours();

      // While it is false, keep the button not interactable
      if (lastConqueredNode.GetComponent<CrystalSeekerSpawner>().CrystalSelected == false)
      {
        uiInterface.PreparationPhaseSetSelectArmyButtonInteractable(false);
      }

      // Once crystal is selected, button is selectable
      else
      {
        uiInterface.PreparationPhaseSetSelectArmyButtonInteractable(true);
        // Update the loot target panel
        CrystalRewards crystalRewards = lastConqueredNode.GetComponent<CrystalSeekerSpawner>().CrystalTarget.GetComponent<CrystalRewards>();
        uiInterface.UpdateLootTargetPanel(crystalRewards.GoldLoot, crystalRewards.CrystalIncomeReward, crystalRewards.BuildingSlotRewarded, crystalRewards.RewardsUpgrade, crystalRewards.RewardedUpgrade);
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

  public void BeginArmySelection(bool conquered)
  {
    NodeSelected = true;

    uiInterface.PreparationPhaseSelectArmyUI(conquered);

    GameObject attackingFromNode = conqueredNodes[conqueredNodes.Count - 1];

    // Set the camera to point at the assembly space
    playerCamera.GetComponent<CameraManager>().PointCameraAtPosition(attackingFromNode.GetComponent<ConqueredNode>().AssemblySpace.transform.position, false, false);
    // Set the unit manager assembly space reference so that selecting our troops spawns them in the assembly space
    uiInterface.UnitManager.SetAssemblySpace(attackingFromNode.GetComponent<ConqueredNode>().AssemblySpace);

    // Update the reference to the node we are going to attack
    attackNode = attackingFromNode.GetComponent<CrystalSeekerSpawner>().CrystalTarget;

    uiInterface.UpdateUINodeColours();
  }

  public void ReturnToNodeSelection()
  {
    NodeSelected = false;

    GameObject lastConqueredNode = conqueredNodes[conqueredNodes.Count - 1];

    // Force the camera into a bird's eye view
    playerCamera.GetComponent<CameraManager>().PointCameraAtPosition(lastConqueredNode.transform.position, true, false);

    // Set the UI Interfaces to invisible and show the button to select army roster
    uiInterface.PreparationPhaseSelectNodeUI();

    uiInterface.UpdateUINodeColours();
  }

  ///////////////////////////////////////////////////////////////
  // ESCORT FUNCTIONS
  public void BeginEscort()
  {
    // Update phase, UI and camera
    CurrentPhase = PHASES.ESCORT;

    musicEmitter.SetParameter("Phases", (int)CurrentPhase);

    uiInterface.PreparationPhaseDisableUI();

    GameObject attackingFromNode = conqueredNodes[conqueredNodes.Count - 1];

    playerCamera.GetComponent<CameraIssueOrdering>().enabled = true;

    // Update the camera bounds
    // Remove the selection camera bounds from the attacking node
    playerCamera.GetComponent<CameraControls>().RemoveCameraBounds(attackingFromNode.GetComponent<ConqueredNode>().SelectionCameraBound);
    // Add the specific path camera bound we are attacking 
    playerCamera.GetComponent<CameraControls>().AddCameraBounds(attackingFromNode.GetComponent<CrystalNode>().RetrieveCameraBound(attackNode));

    // Disabled the crystal nodes functionalities and spawns a crystal seeker
    GameObject spawnedCrystalSeeker = attackingFromNode.GetComponent<CrystalSeekerSpawner>().SpawnCrystalSeeker();
    attackingFromNode.GetComponent<CrystalSeekerSpawner>().ResetCrystalSelection();
    attackingFromNode.GetComponent<CrystalSeekerSpawner>().enabled = false;

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
    // Disables the tree wall so units can pass through, finds it in the attacking node
    attackingFromNode.GetComponent<CrystalNode>().DisableTreeWall(attackNode);

    // Turn off the path visibility meshes to all the connected nodes we don't see the enemies attacking
    attackingFromNode.GetComponent<CrystalNode>().SetPathVisibilityMeshes(false);
  }

  public void EscortWinCutscene(GameObject crystalSeeker)
  {
    crystalSeekerToDestroy = crystalSeeker;

    musicEmitter.SetParameter("At Loot Reward Screen", 1);

    // Fade out the UI Interfaces
    UIFade uiFade = uiInterface.GetComponent<UIFade>();
    uiFade.BeginFadeOut();

    // Disable camera controls
    playerCamera.GetComponent<CameraIssueOrdering>().enabled = false;
    playerCamera.GetComponent<CameraObjectSelection>().ClearSelectionList();
    playerCamera.GetComponent<CameraObjectSelection>().ClearHoverList(true);
    playerCamera.GetComponent<CameraObjectSelection>().enabled = false;
    playerCamera.GetComponent<CameraControls>().enabled = false;

    // Move the camera over to the node we just conquered
    playerCamera.GetComponent<CameraManager>().PointCameraAtPosition(attackNode.transform.position, false, false, 0, 0.5f, false);

    // Start rotation around the node
    playerCamera.GetComponent<CameraManager>().RotateAroundObject(attackNode);

    // Disable wave spawners of the conquered node
    attackNode.GetComponent<CrystalNode>().SetWaveSpawnersActive(false, null);
    // Make the node visible
    attackNode.GetComponent<ConqueredNode>().SetAssemblyFOV(true);

    // Kill all enemies
    GetComponent<HideableManager>().KillAllUnits();

    // Change crystal colour
    attackNode.GetComponent<CrystalNode>().SetCrystalColour(true);

    // Show the loot reward panel
    lootRewardPanel.SetText(uiInterface.LootTargetPanel, attackNode.GetComponent<ConqueredNode>().conquered, EscortWin, PHASE_OUTCOME.ESCORT_WIN);
    lootRewardPanel.ShowLootPanel(true);
  }

  public void EscortWin()
  {
    EndCutscene();

    GameObject attackingFromNode = conqueredNodes[conqueredNodes.Count - 1];
    attackingFromNode.GetComponent<CrystalNode>().conqueredNode = attackNode;

    attackingFromNode.GetComponent<CrystalNode>().SetConnectedNodesUnconquerable(attackNode);

    // Add the conquered node to the list
    conqueredNodes.Add(attackNode);

    GameObject conqueredNode = conqueredNodes[conqueredNodes.Count - 1];

    conqueredNode.GetComponent<CrystalRewards>().CollectLoot(conqueredNode.GetComponent<ConqueredNode>().conquered);

    conqueredNode.GetComponent<ConqueredNode>().conquered = true;

    // Enable the crystal nodes functionalities
    conqueredNode.GetComponent<CrystalSeekerSpawner>().enabled = true;

    // Initialise the rewards of the connecting nodes (if they have not already been initialised)
    conqueredNode.GetComponent<CrystalNode>().InitialiseRewards();

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

    // Find the node we conquered inside the node we were attacking from, and only turn on only those path visibility meshes
    attackingFromNode.GetComponent<CrystalNode>().SetConqueredPathVisibilityMeshes(conqueredNode, true);

    // Turn the conquered node into your faction
    conqueredNode.GetComponent<Faction>().faction = Faction.FACTIONS.GOBLINS;

    // Update the camera bounds
    playerCamera.GetComponent<CameraControls>().AddCameraBounds(conqueredNode.GetComponent<ConqueredNode>().SelectionCameraBound);

    uiInterface.UpdateLootTargetPanel(0, 0, false, false, UPGRADE_TYPE.LAST);

    // Check if our newly conquered node has already conquered a node. If yes, skip to army selection screen and force player
    // to attack that node. Otherwise, player is free to choose
    if (conqueredNode.GetComponent<CrystalNode>().conqueredNode != null)
    {
      conqueredNode.GetComponent<CrystalSeekerSpawner>().SetCrystalTarget(conqueredNode.GetComponent<CrystalNode>().conqueredNode);
      BeginPreparationPhase(false);
      BeginArmySelection(true);
      NodeSelected = true;
    }

    else
    {
      BeginPreparationPhase(true);
      NodeSelected = false;

      // If capturing a node that has a crystal selected, we reset it to null
      if (conqueredNode.GetComponent<CrystalSeekerSpawner>().CrystalSelected == true)
      {
        conqueredNode.GetComponent<CrystalSeekerSpawner>().ResetCrystalSelection();
      }
    }
  }

  public void EscortLoseCutscene(GameObject crystalSeeker)
  {
    crystalSeekerToDestroy = crystalSeeker;

    musicEmitter.SetParameter("At Loot Reward Screen", 1);

    // Fade out the UI Interfaces
    UIFade uiFade = uiInterface.GetComponent<UIFade>();
    uiFade.BeginFadeOut();

    // Disable camera controls
    playerCamera.GetComponent<CameraIssueOrdering>().enabled = false;
    playerCamera.GetComponent<CameraObjectSelection>().ClearSelectionList();
    playerCamera.GetComponent<CameraObjectSelection>().ClearHoverList(true);
    playerCamera.GetComponent<CameraObjectSelection>().enabled = false;
    playerCamera.GetComponent<CameraControls>().enabled = false;

    // Move the camera over to the crystal seeker we lost
    playerCamera.GetComponent<CameraManager>().PointCameraAtPosition(crystalSeeker.transform.position, false, false, 0, 0.5f, false);

    // Start rotation around the node
    playerCamera.GetComponent<CameraManager>().RotateAroundObject(crystalSeeker);

    // Disable wave spawners of the node we lost to
    attackNode.GetComponent<CrystalNode>().SetWaveSpawnersActive(false, null);

    // Kill all your units
    uiInterface.UnitManager.KillAllUnits();

    // Show the loot reward panel
    lootRewardPanel.SetText(null, false, EscortLose, PHASE_OUTCOME.ESCORT_LOSE);
    lootRewardPanel.ShowLootPanel(true);
  }

  public void EscortLose()
  {
    EndCutscene();

    // Set the miner manager health to 10%
    minerManager.MinerDestroyed();

    resourceManager.GainCrystalManpower();

    // Remove all units on the playing field, friendly units are contained in Unit Manager, enemy units are contained in Hideable Manager
    uiInterface.EscortPhaseRemoveAllUnits();
    GetComponent<HideableManager>().RemoveAllUnits();

    // Enable the crystal nodes functionalities
    attackNode.GetComponent<CrystalSeekerSpawner>().enabled = true;
    // Set the node we were attacking to target the node we were attacking from.
    attackNode.GetComponent<CrystalSeekerSpawner>().SetDefendingCrystalTarget(conqueredNodes[conqueredNodes.Count - 1]);

    // Disable wave spawners of the conquered node
    attackNode.GetComponent<CrystalNode>().SetWaveSpawnersActive(false, null);

    BeginPreparationDefensePhase();
  }

  ///////////////////////////////////////////////////////////////
  // PREPARATION DEFENSE FUNCTIONS
  public void BeginPreparationDefensePhase()
  {
    CurrentPhase = PHASES.PREPARATION_DEFENSE;

    musicEmitter.SetParameter("Phases", (int)CurrentPhase);

    ++CurrentRound;

    GameObject attackingFromNode = conqueredNodes[conqueredNodes.Count - 1];

    // Set the camera to point at the assembly space
    playerCamera.GetComponent<CameraManager>().PointCameraAtPosition(attackingFromNode.GetComponent<ConqueredNode>().AssemblySpace.transform.position, false, false);

    // Disables the player from issuing orders to units
    playerCamera.GetComponent<CameraIssueOrdering>().enabled = false;

    uiInterface.PreparationDefensePhaseSelectArmyUI();

    // Set the unit manager assembly space reference
    uiInterface.UnitManager.SetAssemblySpace(attackingFromNode.GetComponent<ConqueredNode>().AssemblySpace);

    // Update the loot target panel
    uiInterface.UpdateLootTargetPanel(0, 0, false, false, UPGRADE_TYPE.LAST);
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

    musicEmitter.SetParameter("Phases", (int)CurrentPhase);

    uiInterface.PreparationDefensePhaseDisableUI();

    playerCamera.GetComponent<CameraIssueOrdering>().enabled = true;

    // Disable the crystal nodes functionalities and spawns a crystal seeker
    GameObject spawnedCrystalSeeker = attackNode.GetComponent<CrystalSeekerSpawner>().SpawnCrystalSeeker();
    attackNode.GetComponent<CrystalSeekerSpawner>().ResetCrystalSelection();
    attackNode.GetComponent<CrystalSeekerSpawner>().enabled = false;

    minerCanvas.GetComponent<CanvasGroup>().alpha = 0;
    bushCanvas.GetComponent<CanvasGroup>().alpha = 1;

    bushManager.Initialise(spawnedCrystalSeeker);

    // Change unit panel buttons to combat buttons so clicking on them selects units instead of deleting them
    uiInterface.UnitManager.SetUnitButtonsToCombat();

    // Start wave spawners of attack node towards your crystal
    attackNode.GetComponent<CrystalNode>().SetWaveSpawnersActive(true, conqueredNodes[conqueredNodes.Count - 1]);
  }

  public void DefenseWinCutscene(GameObject crystalSeeker)
  {
    crystalSeekerToDestroy = crystalSeeker;

    musicEmitter.SetParameter("At Loot Reward Screen", 1);

    // Fade out the UI Interfaces
    UIFade uiFade = uiInterface.GetComponent<UIFade>();
    uiFade.BeginFadeOut();

    // Disable camera controls
    playerCamera.GetComponent<CameraIssueOrdering>().enabled = false;
    playerCamera.GetComponent<CameraObjectSelection>().ClearSelectionList();
    playerCamera.GetComponent<CameraObjectSelection>().ClearHoverList(true);
    playerCamera.GetComponent<CameraObjectSelection>().enabled = false;
    playerCamera.GetComponent<CameraControls>().enabled = false;

    // Move the camera over to the crystal seeker we just killed
    playerCamera.GetComponent<CameraManager>().PointCameraAtPosition(crystalSeeker.transform.position, false, false, 0, 0.5f, false);

    // Start rotation around the node
    playerCamera.GetComponent<CameraManager>().RotateAroundObject(crystalSeeker);

    // Disable wave spawners of the conquered node
    attackNode.GetComponent<CrystalNode>().SetWaveSpawnersActive(false, null);

    // Kill all enemies
    GetComponent<HideableManager>().KillAllUnits();

    // Show the loot reward panel
    lootRewardPanel.SetText(null, false, DefenseWin, PHASE_OUTCOME.DEFENSE_WIN);
    lootRewardPanel.ShowLootPanel(true);
  }

  public void DefenseWin()
  {
    EndCutscene();

    resourceManager.GainCrystalManpower();

    bushManager.ResetHealth();

    // Remove all units on the playing field, friendly units are contained in Unit Manager, enemy units are contained in Hideable Manager
    uiInterface.EscortPhaseRemoveAllUnits();
    GetComponent<HideableManager>().RemoveAllUnits();

    GameObject attackingFromNode = conqueredNodes[conqueredNodes.Count - 1];

    // Enable the crystal nodes functionalities
    attackingFromNode.GetComponent<CrystalSeekerSpawner>().enabled = true;

    // Disable wave spawners of the conquered node
    attackNode.GetComponent<CrystalNode>().SetWaveSpawnersActive(false, null);

    NodeSelected = false;

    // Check if our attacking from node has already conquered a node. If yes, skip to army selection screen and force player
    // to attack that node. Otherwise, player is free to choose
    if (attackingFromNode.GetComponent<CrystalNode>().conqueredNode != null)
    {
      attackingFromNode.GetComponent<CrystalSeekerSpawner>().SetCrystalTarget(attackingFromNode.GetComponent<CrystalNode>().conqueredNode);
      NodeSelected = true;

      BeginPreparationPhase(false);
      BeginArmySelection(true);
    }

    else
    {
      BeginPreparationPhase(true);
    }
  }

  public void DefenseLoseCutscene(GameObject crystalSeeker)
  {
    crystalSeekerToDestroy = crystalSeeker;

    musicEmitter.SetParameter("At Loot Reward Screen", 1);

    // Fade out the UI Interfaces
    UIFade uiFade = uiInterface.GetComponent<UIFade>();
    uiFade.BeginFadeOut();

    // Disable camera controls
    playerCamera.GetComponent<CameraIssueOrdering>().enabled = false;
    playerCamera.GetComponent<CameraObjectSelection>().ClearSelectionList();
    playerCamera.GetComponent<CameraObjectSelection>().ClearHoverList(true);
    playerCamera.GetComponent<CameraObjectSelection>().enabled = false;
    playerCamera.GetComponent<CameraControls>().enabled = false;

    // The node we just lost
    GameObject lostNode = GetActiveNode();

    // Move the camera over to the node we just lost
    playerCamera.GetComponent<CameraManager>().PointCameraAtPosition(lostNode.transform.position, false, false, 0, 0.5f, false);

    // Start rotation around the node
    playerCamera.GetComponent<CameraManager>().RotateAroundObject(lostNode);

    // Disable wave spawners of the attacking node
    attackNode.GetComponent<CrystalNode>().SetWaveSpawnersActive(false, null);
    // Make our node visible
    lostNode.GetComponent<ConqueredNode>().SetAssemblyFOV(true);

    // Kill all your units
    uiInterface.UnitManager.KillAllUnits();

    // Change crystal colour
    lostNode.GetComponent<CrystalNode>().SetCrystalColour(false);

    // Show the loot reward panel
    lootRewardPanel.SetText(null, false, DefenseLose, PHASE_OUTCOME.DEFENSE_LOSE);
    lootRewardPanel.ShowLootPanel(true);
  }

  public void DefenseLose()
  {
    EndCutscene();

    resourceManager.GainCrystalManpower();

    // Remove all units on the playing field, friendly units are contained in Unit Manager, enemy units are contained in Hideable Manager
    uiInterface.EscortPhaseRemoveAllUnits();
    GetComponent<HideableManager>().RemoveAllUnits();

    GameObject lostNode = conqueredNodes[conqueredNodes.Count - 1];
    lostNode.GetComponent<CrystalNode>().active = false;
    attackNode.GetComponent<CrystalNode>().targeted = true;

    // Remove the path camera bounds from the node we lost
    //playerCamera.GetComponent<CameraControls>().RemoveCameraBounds(lostNode.GetComponent<CrystalNode>().RetrieveCameraBound(attackNode));

    // Disable wave spawners and crystal nodes functionalities of the attacking node we lost to
    attackNode.GetComponent<CrystalNode>().SetWaveSpawnersActive(false, null);
    attackNode.GetComponent<CrystalSeekerSpawner>().enabled = false;

    // Update the attacking node to the node we lost and remove that node from our list of conquered nodes
    attackNode = conqueredNodes[conqueredNodes.Count - 1];
    conqueredNodes.RemoveAt(conqueredNodes.Count - 1);

    // Enable the crystal nodes functionalities of the new attacking node
    attackNode.GetComponent<Faction>().faction = Faction.FACTIONS.NEUTRAL;
    attackNode.GetComponent<CrystalSeekerSpawner>().enabled = true;
    attackNode.GetComponent<CrystalSeekerSpawner>().SetDefendingCrystalTarget(conqueredNodes[conqueredNodes.Count - 1]);

    // Lose the crystal income from that node
    resourceManager.LoseIncome(attackNode.GetComponent<CrystalRewards>().CrystalIncomeReward);

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

    // Turn off the assembly visibility mesh of the node we lost
    attackNode.GetComponent<ConqueredNode>().SetAssemblyFOV(false);
    conqueredNodes[conqueredNodes.Count - 1].GetComponent<CrystalNode>().SetPathVisibilityMeshes(false);

    BeginPreparationDefensePhase();
  }

  private void EndCutscene()
  {
    Destroy(crystalSeekerToDestroy);

    musicEmitter.SetParameter("At Loot Reward Screen", 0);

    // Reset changed components in the cutscene
    lootRewardPanel.ShowLootPanel(false);
    playerCamera.GetComponent<CameraObjectSelection>().enabled = true;
    // Fade in the UI Interfaces
    UIFade uiFade = uiInterface.GetComponent<UIFade>();
    uiFade.BeginFadeIn();

    resourceManager.GainCrystalManpower();

    // Remove all units on the playing field, friendly units are contained in Unit Manager, enemy units are contained in Hideable Manager
    uiInterface.EscortPhaseRemoveAllUnits();
    GetComponent<HideableManager>().RemoveAllUnits();
  }

  public GameObject GetActiveNode()
  {
    return conqueredNodes[conqueredNodes.Count - 1];
  }
}
