using UnityEngine;
using UnityEngine.UI;

public class ConstructPanel : MonoBehaviour
{
  [SerializeField]
  private ConstructButton 
    farmButton = null,
    archeryRangeButton = null,
    blacksmithButton = null,
    brawlPitButton = null,
    mageTowerButton = null;

  private UIInterface uiInterface = null;

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
  }

  private void Update()
  {
    // These buildings can only be constructed once
    archeryRangeButton.GetComponent<Button>().interactable = !GameManager.buildingManager.archeryRangeConstructed;
    blacksmithButton.GetComponent<Button>().interactable = !GameManager.buildingManager.blacksmithConstructed;
    brawlPitButton.GetComponent<Button>().interactable = !GameManager.buildingManager.brawlPitConstructed;
    mageTowerButton.GetComponent<Button>().interactable = !GameManager.buildingManager.mageTowerConstructed;
  }
}
