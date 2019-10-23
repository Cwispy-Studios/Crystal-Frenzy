using UnityEngine;

public class CrystalRewards : MonoBehaviour
{
  public int goldLoot;
  public int crystalIncomeReward;

  // Popup variables
  private const float POPUP_HOVER_TIME = 0.5f;
  private float popupHoverLength = 0;

  private UIInterface uiInterface;

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
  }

  private void Update()
  {
    // Check if this object is in the mouseHoverList
    if (CameraObjectSelection.MouseHoverUnitsList.Contains(gameObject))
    {
      popupHoverLength += Time.deltaTime;
    }

    else
    {
      popupHoverLength = 0;
    }

    if (popupHoverLength >= POPUP_HOVER_TIME)
    {
      uiInterface.ShowLootPopup(goldLoot, crystalIncomeReward, GetComponent<ConqueredNode>().conquered, gameObject);
    }

    else
    {
      uiInterface.HideLootPopup(gameObject);
    }
  }
}
