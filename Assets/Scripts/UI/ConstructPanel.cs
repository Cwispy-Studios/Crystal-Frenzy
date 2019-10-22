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

  public GameObject connectedNode;

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
  }

  private void Update()
  {
    // These buildings can only be constructed once
    archeryRangeButton.available = archeryRangeButton.GetComponent<Button>().interactable = !GameManager.buildingManager.archeryRangeConstructed;
    blacksmithButton.available = blacksmithButton.GetComponent<Button>().interactable = !GameManager.buildingManager.blacksmithConstructed;
    brawlPitButton.available = brawlPitButton.GetComponent<Button>().interactable = !GameManager.buildingManager.brawlPitConstructed;
    mageTowerButton.available = mageTowerButton.GetComponent<Button>().interactable = !GameManager.buildingManager.mageTowerConstructed;

    farmButton.connectedNode = archeryRangeButton.connectedNode = blacksmithButton.connectedNode = brawlPitButton.connectedNode = mageTowerButton.connectedNode = connectedNode;
  }
}
