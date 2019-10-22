using UnityEngine;
using UnityEngine.UI;

public class BuildingTooltipPopup : MonoBehaviour
{
  [SerializeField]
  private Text nameText = null, costText = null, descriptionText = null, constructText = null;

  public void SetText(string name, int cost, string description, string constructMessage)
  {
    nameText.text = name;
    costText.text = cost.ToString();
    descriptionText.text = description;
    constructText.text = constructMessage;
  }
}