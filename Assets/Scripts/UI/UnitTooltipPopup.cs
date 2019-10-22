using UnityEngine;
using UnityEngine.UI;

public class UnitTooltipPopup : MonoBehaviour
{
  [SerializeField]
  private Text nameText = null, costText = null, healthText = null, damageText = null, attackSpeedText = null, 
    descriptionText = null, constructText = null;

  public void SetText(string name, int cost, int health, int damage, float attackSpeed, string description, string constructMessage)
  {
    nameText.text = name;
    costText.text = cost.ToString();
    healthText.text = health.ToString();
    damageText.text = damage.ToString();
    attackSpeedText.text = attackSpeed.ToString();
    descriptionText.text = description;
    constructText.text = constructMessage;
  }
}