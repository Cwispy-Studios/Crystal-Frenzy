using System;
using UnityEngine;

public class Health : MonoBehaviour
{
  public static event Action<Health> OnHealthAdded = delegate { };
  public static event Action<Health> OnHealthRemoved = delegate { };

  [SerializeField]
  private int maxHealth = 100;

  public int CurrentHealth { get; private set; }

  public event Action<float> OnHealthChanged = delegate { };

  private void OnEnable()
  {
    CurrentHealth = maxHealth;
    //OnHealthAdded(this);
  }

  public void ModifyHealth(int amount)
  {
    CurrentHealth += amount;

    CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

    float currentHealthPct = (float) CurrentHealth / maxHealth;

    OnHealthChanged(currentHealthPct);
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
    OnHealthRemoved(this);
  }
}
