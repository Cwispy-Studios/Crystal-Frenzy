using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
  [Header("Tutorial Panel")]
  [SerializeField]
  private GameObject tutorialPanel = null;
  [SerializeField]
  private Button button1 = null, button2 = null;
  [SerializeField]
  private Text tutorialText = null, button1Text = null, button2Text = null;

  private string[] tutorialTexts;

  public bool DisablePhaseProgression { get; private set; }
  private int currentTutorialPanel = 1;

  private Dictionary<int, UnityEngine.Events.UnityAction> panels;

  private Camera playerCamera;

  [Header("Scene Objects")]
  [SerializeField]
  private GameObject fortress = null, firstCrystalNode = null;

  public void StartTutorial()
  {
    tutorialTexts = new string[10];

    tutorialTexts[0] = 
      "Greetings Overlord, welcome to the battlefield. I trust you will fair better than the previous Overload who has since become Furst food ever since his latest... failures. " +
      "Tell me, is this your first forray into battle?";

    tutorialTexts[1] =
      "Ah, Furst food. No matter, allow me to explain the rules of the battle. Your task is to conquer the Life Crystal somewhere deep in the forest. " +
      "There is no clear path to it, and you will have to conquer the various Crystal Nodes along the way, and cut a path to the Life Crystal.";

    tutorialTexts[2] =
      "This is your Fortress. This is where you can upgrade your Miner to be sturdier or faster. This is the heart of your army, a marvel of - what? Why does it look like it is in shambles? " +
      "We had to allocate our resources to more.... prospective opportunities. Do not fear, I trust you will handle yourself.";

    tutorialTexts[3] =
      "This wil be your first objective. To capture the Crytal Node, you have to target your Crystal Miner to the Crystal Node. Select the Crytal Node to target it.";

    tutorialTexts[4] =
      "Excellent. Capturing a Crystal Node gives you various loot. Once you have captured one in a path, the other Crystal Nodes are locked out, so choose which to capture carefully. " +
      "The better the loot, the more voraciously the Fursts will defend the Crystal Node. Observe the path illuminated from your Fortress to the Crystal Node. This is the path that your Crystal Miner will follow. " +
      "Once deployed, it will advance towards the Crystal Node without fail. You cannot control it, and the Fursts will awaken to the sound of the Crystal Miner and come out in droves to destroy it. " +
      "It is imperative that you defend it at all costs with your troops.";

    tutorialTexts[5] =
      "Should your Crystal Miner be damaged in battle, you can repair it with Gold by bringing up the Upgrade and Repair panel by selecting any of your controlled Crystals Nodes or your Fortress. " +
      "Fail to defend your Crystal Miner and it will get destroyed, and the enemy will send out their own to reclaim their lost terrority, and you will have to destroy it to continue. " +
      "Lose enough and they will reach your Fortress, and the Fursts will certainly feast on your cadavers.";

    tutorialTexts[6] =
      "Gold is used to construct and upgrade buildings, and to repair your Miner. Gold can be acquired from killing Fursts and capturing Crystals for the first time. " +
      "Crystals is used to upgrade your units, and Crystals Income is acquired as a reward for capturing Crystal Nodes. " +
      "You gain Crystals at the end of every round from your Crystal Income, and every Unit Point that survives the Round is extra manpower to mine the Crystal Nodes, which earns you more Crystals. " +
      "Unit Cap is increased by constructing and upgrading Farms and limits the size of your army every Round.";

    tutorialTexts[7] =
      "The Fursts get stronger every Round, so do not dally, or you will find yourself against impossible odds. Learn your troops and what you can upgrade, and remember to make use of your Gold to construct and upgrade buildings. " +
      "Buildings cannot be demolished, so choose wisely what you construct first.";

    tutorialTexts[8] =
      "Excellent, I hope for your sake you are not saying that for the Fursts. Then you need no further counsel. May you bathe in the blood of the Fursts as the Sun dawns.";

    playerCamera = Camera.main;

    tutorialPanel.SetActive(true);

    panels = new Dictionary<int, UnityEngine.Events.UnityAction>
    {
      { 1, SetPanelOne },
      { 2, SetPanelTwo },
      { 3, SetPanelThree },
      { 4, SetPanelFour },
      { 5, SetPanelFive },
      { 6, SetPanelSix },
      { 7, SetPanelSeven },
      { 8, SetPanelEight },
      { 9, SetPanelNine },
    };

    DisablePhaseProgression = true;

    SetPanelOne();
  }

  /**************************************************************************
   * Introduction, check if player has already played before
   * ************************************************************************/
  private void SetPanelOne()
  {
    currentTutorialPanel = 1;

    // Set the texts
    tutorialText.text = tutorialTexts[currentTutorialPanel - 1];
    button1Text.text = "Yes Master.";
    button2Text.text = "I am seasoned, Master";

    // Set buttons active
    button1.gameObject.SetActive(true);
    button2.gameObject.SetActive(true);

    // Set the next panels for the buttons
    button1.onClick.AddListener(panels[currentTutorialPanel + 1]);
    button2.onClick.AddListener(panels[9]);

    // Do not allow player to select Crystal yet
    playerCamera.GetComponent<CameraObjectSelection>().enabled = false;
  }

  /**************************************************************************
   * New player, give overview of player goal
   * ************************************************************************/
  private void SetPanelTwo()
  {
    currentTutorialPanel = 2;

    // Set the texts
    tutorialText.text = tutorialTexts[currentTutorialPanel - 1];
    button1Text.text = "Continue";
    button2Text.text = "";

    // Set buttons active
    button1.gameObject.SetActive(true);
    button2.gameObject.SetActive(false);

    // Set the next panels for the buttons
    button1.onClick.RemoveListener(panels[currentTutorialPanel]);
    button1.onClick.AddListener(panels[currentTutorialPanel + 1]);
  }

  /**************************************************************************
   * Give rundown on Fortress
   * ************************************************************************/
  private void SetPanelThree()
  {
    currentTutorialPanel = 3;

    // Set the texts
    tutorialText.text = tutorialTexts[currentTutorialPanel -1];
    button1Text.text = "Continue";
    button2Text.text = "";

    // Set buttons active
    button1.gameObject.SetActive(true);
    button2.gameObject.SetActive(false);

    // Set the next panels for the buttons
    button1.onClick.RemoveListener(panels[currentTutorialPanel]);
    button1.onClick.AddListener(panels[currentTutorialPanel + 1]);

    playerCamera.GetComponent<CameraManager>().PointCameraAtPosition(fortress.transform.position, false, false, 20f, 1.2f, false);
  }

  /**************************************************************************
   * Tutorial on targeting Crystals
   * ************************************************************************/
  private void SetPanelFour()
  {
    currentTutorialPanel = 4;

    // Set the texts
    tutorialText.text = tutorialTexts[currentTutorialPanel - 1];
    button1Text.text = "";
    button2Text.text = "";

    // Set buttons active
    button1.gameObject.SetActive(false);
    button2.gameObject.SetActive(false);

    // Set the next panels for the buttons
    playerCamera.GetComponent<CameraManager>().PointCameraAtPosition(firstCrystalNode.transform.position, false, false, 20f);
    playerCamera.GetComponent<CameraObjectSelection>().enabled = true;
  }

  private void Update()
  {
    if (currentTutorialPanel == 4)
    {
      if (GameManager.GetActiveNode().GetComponent<CrystalSeekerSpawner>().crystalSelected == true)
      {
        SetPanelFive();
      }
    }
  }

  /**************************************************************************
   * Explanation of Crystal Miner during combat and Crystal Rewards
   * ************************************************************************/
  private void SetPanelFive()
  {
    currentTutorialPanel = 5;

    // Set the texts
    tutorialText.text = tutorialTexts[currentTutorialPanel - 1];
    button1Text.text = "Continue";
    button2Text.text = "";

    // Set buttons active
    button1.gameObject.SetActive(true);
    button2.gameObject.SetActive(false);

    // Set the next panels for the buttons
    button1.onClick.RemoveListener(panels[currentTutorialPanel]);
    button1.onClick.AddListener(panels[currentTutorialPanel + 1]);
  }

  /**************************************************************************
   * Further explanation of Crystal Miner 
   * ************************************************************************/
  private void SetPanelSix()
  {
    currentTutorialPanel = 6;

    // Set the texts
    tutorialText.text = tutorialTexts[currentTutorialPanel - 1];
    button1Text.text = "Continue";
    button2Text.text = "";

    // Set buttons active
    button1.gameObject.SetActive(true);
    button2.gameObject.SetActive(false);

    // Set the next panels for the buttons
    button1.onClick.RemoveListener(panels[currentTutorialPanel]);
    button1.onClick.AddListener(panels[currentTutorialPanel + 1]);
  }

  /**************************************************************************
   * Explanation of resources
   * ************************************************************************/
  private void SetPanelSeven()
  {
    currentTutorialPanel = 7;

    // Set the texts
    tutorialText.text = tutorialTexts[currentTutorialPanel - 1];
    button1Text.text = "Continue";
    button2Text.text = "";

    // Set buttons active
    button1.gameObject.SetActive(true);
    button2.gameObject.SetActive(false);

    // Set the next panels for the buttons
    button1.onClick.RemoveListener(panels[currentTutorialPanel]);
    button1.onClick.AddListener(panels[currentTutorialPanel + 1]);
  }

  /**************************************************************************
   * Closing notes
   * ************************************************************************/
  private void SetPanelEight()
  {
    currentTutorialPanel = 8;

    // Set the texts
    tutorialText.text = tutorialTexts[currentTutorialPanel - 1];
    button1Text.text = "To Battle!";
    button2Text.text = "";

    // Set buttons active
    button1.gameObject.SetActive(true);
    button2.gameObject.SetActive(false);

    // Set the next panels for the buttons
    button1.onClick.RemoveListener(panels[currentTutorialPanel]);
    button1.onClick.AddListener(ContinueGame);
  }

  /**************************************************************************
   * Returning player
   * ************************************************************************/
  private void SetPanelNine()
  {
    currentTutorialPanel = 9;

    // Set the texts
    tutorialText.text = tutorialTexts[currentTutorialPanel - 1];
    button1Text.text = "To Battle!";
    button2Text.text = "";

    // Set buttons active
    button1.gameObject.SetActive(true);
    button2.gameObject.SetActive(false);

    // Set the next panels for the buttons
    button1.onClick.RemoveListener(panels[currentTutorialPanel]);
    button1.onClick.AddListener(ContinueGame);
  }

  private void ContinueGame()
  {
    playerCamera.GetComponent<CameraObjectSelection>().enabled = true;
    DisablePhaseProgression = false;
    tutorialPanel.SetActive(false);
  }
}
