/******************************************************************************/
/*!
\file   Health.cs
\author Unity3d College, Wong Zhihao
\par    email: wongzhihao.student.utwente.nl
\date   15 October 2019
\brief

  Brief:
    Implementation from: https://www.youtube.com/watch?v=kQqqo_9FfsU&t=396s
*/
/******************************************************************************/

using System;
using UnityEngine;

public class Health : MonoBehaviour
{
  public static event Action<Health> OnHealthAdded = delegate { };
  public static event Action<Health> OnHealthRemoved = delegate { };

  [SerializeField]
  private int maxHealth = 100;
  public int MaxHealth
  {
    get { return maxHealth; }
    set { maxHealth = value; }
  }

  [SerializeField]
  private COMBATANT_TYPE combatantType = COMBATANT_TYPE.NORMAL;
  public COMBATANT_TYPE CombatantType
  {
    get
    {
      return combatantType;
    }
  }

  public int CurrentHealth { get; private set; }

  public event Action<float> OnHealthChanged = delegate { };

  private void Start()
  {
    CurrentHealth = maxHealth;
  }

  public void ModifyHealth(int amount)
  {
    CurrentHealth += amount;

    CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

    float currentHealthPct = (float) CurrentHealth / maxHealth;

    OnHealthChanged(currentHealthPct);

    // Kill the unit
    if (CurrentHealth == 0)
    {
      // Check if unit is recruitable, destroys the unit button as well
      RecruitableUnit recruitableUnit = GetComponent<RecruitableUnit>();

      if (recruitableUnit != null)
      {
        recruitableUnit.KillUnit();
      }

      Destroy(gameObject);
    }
  }

  private void Update()
  {
    // Layer 10 is invisible, enemies are set to invisible if they are inside fog of war
    if (CurrentHealth == maxHealth || gameObject.layer == 10)
    {
      OnHealthRemoved(this);
    }

    else
    {
      OnHealthAdded(this);
    }
  }

  private void OnDisable()
  {
    if (CurrentHealth != maxHealth)
    {
      OnHealthRemoved(this);
    }
  }

  public void SetUpgradedProperties(UpgradeProperties[] upgradeProperties)
  {
    for (int i = 0; i < upgradeProperties.Length; ++i)
    {
      maxHealth += upgradeProperties[i].health;
    }
  }
}
