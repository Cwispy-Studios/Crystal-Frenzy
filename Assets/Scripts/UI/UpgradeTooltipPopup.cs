using UnityEngine;
using UnityEngine.UI;

public class UpgradeTooltipPopup : MonoBehaviour
{
  [SerializeField]
  private Text nameText = null, healthText = null, damageText = null, attackSpeedText = null, 
    descriptionText = null, constructText = null;

  public void SetText(string name, int health, int damage, float attackSpeed, string description, string constructMessage)
  {
    nameText.text = name;
    healthText.text = health.ToString();
    damageText.text = damage.ToString();
    attackSpeedText.text = attackSpeed.ToString();
    descriptionText.text = description;
    constructText.text = constructMessage;
  }
}