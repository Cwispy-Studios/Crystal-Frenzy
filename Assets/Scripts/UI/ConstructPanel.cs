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

  public GameObject connectedNode;

  private void Update()
  {
    // These buildings can only be constructed once
    archeryRangeButton.available = !GameManager.buildingManager.archeryRangeConstructed;
    blacksmithButton.available = !GameManager.buildingManager.blacksmithConstructed;
    brawlPitButton.available = !GameManager.buildingManager.brawlPitConstructed;
    mageTowerButton.available = !GameManager.buildingManager.mageTowerConstructed;

    farmButton.connectedNode = archeryRangeButton.connectedNode = blacksmithButton.connectedNode = brawlPitButton.connectedNode = mageTowerButton.connectedNode = connectedNode;
  }
}
