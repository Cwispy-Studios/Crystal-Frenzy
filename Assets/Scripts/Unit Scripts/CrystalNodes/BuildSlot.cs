using UnityEngine;

public class BuildSlot : MonoBehaviour
{
  private UIInterface uiInterface;

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
  }

  private void Update()
  {
    // Check if this object is selected
    if (CameraObjectSelection.SelectedUnitsList.Contains(gameObject))
    {

    }

    else
    {

    }

    //if (popupHoverLength >= POPUP_HOVER_TIME)
    //{
    //  uiInterface.ShowLootPopup(goldLoot, crystalIncomeReward, gameObject);
    //}

    //else
    //{
    //  uiInterface.HideRewardPopup(gameObject);
    //}
  }
}
