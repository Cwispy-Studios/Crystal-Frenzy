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
  [SerializeField]
  private COMBATANT_TYPE combatantType;
  public COMBATANT_TYPE CombatantType
  {
    get
    {
      return combatantType;
    }
  }

  public int CurrentHealth { get; private set; }

  public event Action<float> OnHealthChanged = delegate { };

  private void OnEnable()
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

    }
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      ModifyHealth(-10);
    }

    if (Input.GetKeyDown(KeyCode.M))
    {
      ModifyHealth(10);
    }

    if (CurrentHealth == maxHealth)
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
}
