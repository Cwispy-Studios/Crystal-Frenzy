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
  private float maxHealth = 100;
  public float MaxHealth
  {
    get { return maxHealth; }
  }

  [SerializeField]
  private float regeneration = 0.1f;
  public float Regeneration
  {
    get { return regeneration; }
  }

  [SerializeField]
  private bool explodesOnDeath = false;
  [SerializeField]
  private float explosionDmgPctOfHealth = 0.25f, explosionRadius = 10f;

  [SerializeField]
  private COMBATANT_TYPE combatantType = COMBATANT_TYPE.NORMAL;
  public COMBATANT_TYPE CombatantType
  {
    get
    {
      return combatantType;
    }
  }

  public float CurrentHealth { get; protected set; }

  public event Action<float> OnHealthChanged = delegate { };

  private void Start()
  {
    if (GetComponent<CrystalSeeker>() != null && GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS)
    {
      CurrentHealth = GameManager.minerManager.CurrentMinerHealth;
    }

    else
    {
      CurrentHealth = maxHealth;
    }
  }

  public void ModifyHealth(float amount)
  {
    CurrentHealth += amount;

    CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

    float currentHealthPct = CurrentHealth / maxHealth;

    OnHealthChanged(currentHealthPct);
    
    if (GetComponent<CrystalSeeker>() != null && GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS)
    {
      GameManager.minerManager.HandleMinerHealthChanged(currentHealthPct, CurrentHealth);
    }

    // Kill the unit
    if (CurrentHealth <= 0)
    {
      // Check if unit is recruitable, destroys the unit button as well
      RecruitableUnit recruitableUnit = GetComponent<RecruitableUnit>();

      if (recruitableUnit != null)
      {
        recruitableUnit.KillUnit();
      }

      if (explodesOnDeath)
      {
        int layerMask = 0;

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, ~layerMask);

        for (int i = 0; i < colliders.Length; ++i)
        {
          if (colliders[i].GetComponent<Faction>() != null && colliders[i].GetComponent<Health>() != null)
          {
            // Check if is unfriendly unit and has health
            if ((GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS && colliders[i].GetComponent<Faction>().faction == Faction.FACTIONS.FOREST) ||
                (GetComponent<Faction>().faction == Faction.FACTIONS.FOREST && colliders[i].GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS))
            {
              colliders[i].GetComponent<Health>().ModifyHealth(-explosionDmgPctOfHealth * MaxHealth);

              if (GetComponent<StatusEffects>())
              {
                GetComponent<StatusEffects>().AfflictStatusEffects(colliders[i].gameObject);
              }
            }
          }
        }
      }

      Destroy(gameObject);
    }
  }

  private void Update()
  {
    ModifyHealth(regeneration * Time.deltaTime);

    // Layer 10 is invisible, enemies are set to invisible if they are inside fog of war, we shouldn't see their health bars
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
      regeneration += upgradeProperties[i].regeneraton;

      if (GetComponent<CrystalSeeker>() != null && GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS)
      {
        GameManager.minerManager.UpgradeMinerHealth(upgradeProperties[i].health, maxHealth);
      }
    }
  }

  public void SetBoostedValues(BoostValues boostValues)
  {
      maxHealth += (GameManager.CurrentRound - 1) * boostValues.healthModifier * maxHealth;
      regeneration += (GameManager.CurrentRound - 1) * boostValues.regeneratonModifier * regeneration;
  }

  public bool AtMaxHealth()
  {
    return CurrentHealth == MaxHealth;
  }

  public float HealthPct()
  {
    return CurrentHealth / MaxHealth;
  }
}
