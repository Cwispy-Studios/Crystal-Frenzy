using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
  [SerializeField]
  private UpgradeButton[] upgradeButtons = null;

  private UIInterface uiInterface = null;

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
  }

  private void Update()
  {
    for (int i = 0; i < upgradeButtons.Length; ++i)
    {
      // Upgrade button is null if the max level has been reached, then no button will be shown for that upgrade
      if (upgradeButtons[i] != null && upgradeButtons[i].upgraded == true)
      {
        GameObject newButton = upgradeButtons[i].nextLevelButton;

        Destroy(upgradeButtons[i].gameObject);

        if (newButton != null)
        {
          upgradeButtons[i] = newButton.GetComponent<UpgradeButton>();
        }
      }
    }
  }
}
