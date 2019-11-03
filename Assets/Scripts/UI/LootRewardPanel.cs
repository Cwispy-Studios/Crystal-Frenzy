using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LootRewardPanel : MonoBehaviour
{
  public delegate void NextPhase();

  [SerializeField]
  private Text goldText = null, crystalText = null, unitPointsText = null;
  [SerializeField]
  private Text[] additionalRewardsText = null;
  [HideInInspector]
  public bool isBuildingSlotRewarded = false, isUpgradeRewarded = false;
  [HideInInspector]
  public UPGRADE_TYPE upgradeRewarded;
  [SerializeField]
  private Button continueButton = null;

  private void Awake()
  {
    transform.localScale = Vector3.zero;
  }

  public void SetText(LootTargetPanel lootTargetPanel, NextPhase nextPhase)
  {
    continueButton.onClick.RemoveAllListeners();
    continueButton.onClick.AddListener(delegate { nextPhase(); });

    isBuildingSlotRewarded = lootTargetPanel.isBuildingSlotRewarded;
    isUpgradeRewarded = lootTargetPanel.isUpgradeRewarded;
    upgradeRewarded = lootTargetPanel.upgradeRewarded;

    int gold = lootTargetPanel.cacheGold;
    int crystal = lootTargetPanel.cacheCrystalIncome;

    if (gold == 0)
    {
      goldText.text = "None";
    }

    else
    {
      goldText.text = gold.ToString();
    }

    if (crystal == 0)
    {
      crystalText.text = "None";
    }

    else
    {
      crystalText.text = "+" + crystal.ToString() + " (+" + GameManager.resourceManager.CrystalsIncome + ")";
    }

    unitPointsText.text = GameManager.resourceManager.ArmySize.ToString();

    int numAddRewards = 0;

    // Reset additional rewards texts
    for (int i = 0; i < additionalRewardsText.Length; ++i)
    {
      additionalRewardsText[i].text = "";
    }

    if (isBuildingSlotRewarded == true)
    {
      additionalRewardsText[numAddRewards].text += "<color=lightblue>Building Slot Captured</color>";
      ++numAddRewards;
    }

    if (isUpgradeRewarded == true)
    {
      additionalRewardsText[numAddRewards].text = "<color=magenta>" + upgradeRewarded.ToString().Replace("_", " ") + "</color>";
      ++numAddRewards;
    }
  }

  public void ShowLootPanel(bool show)
  {
    GetComponent<CanvasGroup>().interactable = false;

    StartScaling(show);
  }

  private IEnumerator Scale(Vector3 fromScale, Vector3 toScale, float delayDuration)
  {
    float startTime = Time.time;
   
    while (Time.time - startTime < delayDuration)
    {
      yield return 1;
    }

    float startScaleTime = Time.time;
    float scaleDuration = 1f;

    while (Time.time - startScaleTime < scaleDuration)
    {
      transform.localScale = Vector3.Lerp(fromScale, toScale, (Time.time - startScaleTime) / scaleDuration);

      yield return 1;
    }

    transform.localScale = toScale;

    if (toScale == Vector3.one)
    {
      GetComponent<CanvasGroup>().interactable = true;
    }
  }

  private Coroutine StartScaling(bool show)
  {
    Vector3 targetScale = show ? Vector3.one : Vector3.zero;
    float delayDuration = show ? 2f : 0f;

    return StartCoroutine(Scale(transform.localScale, targetScale, delayDuration));
  }
}
