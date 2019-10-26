using UnityEngine;
using UnityEngine.UI;

public class UpgradeTooltipPopup : MonoBehaviour
{
  [SerializeField]
  private Text nameText = null, unitCostText = null, costText = null, healthText = null, damageText = null, attackSpeedText = null, 
    descriptionText = null, constructText = null;

  public void SetText(string name, int unitCost, int cost, float health, float damage, float attackSpeed, string description, string constructMessage)
  {
    int intHealth = Mathf.CeilToInt(health);
    int intDamage = Mathf.CeilToInt(damage);
    string plusOrMinus = "";

    nameText.text = name;
    costText.text = cost.ToString();

    if (unitCost < 0)
    {
      plusOrMinus = "";
    }

    else
    {
      plusOrMinus = "+";
    }

    unitCostText.text = plusOrMinus + unitCost.ToString();

    if (intHealth < 0)
    {
      plusOrMinus = "";
    }

    else
    {
      plusOrMinus = "+";
    }

    healthText.text = plusOrMinus + intHealth.ToString();

    if (intDamage < 0)
    {
      plusOrMinus = "";
    }

    else
    {
      plusOrMinus = "+";
    }

    damageText.text = plusOrMinus + intDamage.ToString();

    if (attackSpeed < 0)
    {
      plusOrMinus = "";
    }

    else
    {
      plusOrMinus = "+";
    }

    attackSpeedText.text = plusOrMinus + attackSpeed.ToString();
    descriptionText.text = description;
    constructText.text = constructMessage;
  }
}