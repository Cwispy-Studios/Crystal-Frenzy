using UnityEngine;
using UnityEngine.UI;

public class UpgradeTooltipPopup : MonoBehaviour
{
  [SerializeField]
  private Text nameText = null, costText = null, healthText = null, damageText = null, attackSpeedText = null, 
    descriptionText = null, constructText = null;

  public void SetText(string name, int cost, float health, float damage, float attackSpeed, string description, string constructMessage)
  {
    int intHealth = Mathf.CeilToInt(health);
    int intDamage = Mathf.CeilToInt(damage);

    nameText.text = name;
    costText.text = cost.ToString();
    healthText.text = "+" + intHealth.ToString();
    damageText.text = "+" + intDamage.ToString();
    attackSpeedText.text = "+" + attackSpeed.ToString();
    descriptionText.text = description;
    constructText.text = constructMessage;
  }
}