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
  public bool kill = false;

  public static event Action<Health> OnHealthAdded = delegate { };
  public static event Action<Health> OnHealthRemoved = delegate { };

  [SerializeField]
  private GameObject ragdollPrefab = null;
  private bool ragdollSpawned = false;

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

  private float ogHealth;
  private float ogRegeneration;

  [SerializeField]
  private bool explodesOnDeath = false;
  [SerializeField]
  private float explosionDmgPctOfHealth = 0.25f, explosionRadius = 10f;
  [SerializeField]
  private GameObject explosionEffect = null;

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

  [FMODUnity.EventRef]
  public string deathSound = "", explodeSound = "";

  private GameManager gameManager;

  private void Awake()
  {
    ogHealth = maxHealth;
    ogRegeneration = regeneration;

    gameManager = FindObjectOfType<GameManager>();
  }

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

  public void ModifyHealth(float amount, Vector3 damageDirection)
  {
    // If unit is already dead, do nothing
    if (CurrentHealth <= 0)
    {
      return;
    }

    CurrentHealth += amount;

    // Saves the negative health (if it died) so we can use it as a force for the ragdoll
    float overkillDamage = CurrentHealth;

    CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

    float currentHealthPct = CurrentHealth / maxHealth;

    OnHealthChanged(currentHealthPct);
    
    if (GetComponent<CrystalSeeker>() != null)
    {
      if (GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS)
      {
        GameManager.minerManager.HandleMinerHealthChanged(currentHealthPct, CurrentHealth);
      }

      else if (GetComponent<Faction>().faction == Faction.FACTIONS.FOREST)
      {
        GameManager.bushManager.HandleBushHealthChanged(currentHealthPct, CurrentHealth);
      }
    }

    // Kill the unit
    if (CurrentHealth <= 0)
    {
      FMODUnity.RuntimeManager.PlayOneShot(deathSound, transform.position);

      // Check if unit is recruitable, destroys the unit button as well
      RecruitableUnit recruitableUnit = GetComponent<RecruitableUnit>();

      if (recruitableUnit != null)
      {
        recruitableUnit.KillUnit();
      }

      // If enemy explodes on death, damages everything around it
      if (explodesOnDeath)
      {
        FMODUnity.RuntimeManager.PlayOneShot(explodeSound, transform.position);

        int layerMask = 0;

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, ~layerMask);

        for (int i = 0; i < colliders.Length; ++i)
        {
          if (colliders[i].gameObject != gameObject && colliders[i].GetComponent<Faction>() != null && colliders[i].GetComponent<Health>() != null)
          {
            // Check if is unfriendly unit and has health
            //if ((GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS && colliders[i].GetComponent<Faction>().faction == Faction.FACTIONS.FOREST) ||
            //    (GetComponent<Faction>().faction == Faction.FACTIONS.FOREST && colliders[i].GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS))
            //{
              colliders[i].GetComponent<Health>().ModifyHealth(-explosionDmgPctOfHealth * MaxHealth, transform.position);

              if (GetComponent<StatusEffects>())
              {
                GetComponent<StatusEffects>().AfflictStatusEffects(colliders[i].gameObject);
              }
          }
        }

        Instantiate(explosionEffect, transform.position, new Quaternion());
      }

      if (ragdollPrefab && !ragdollSpawned)
      {
        var ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);

        overkillDamage = Mathf.Abs(overkillDamage);

        // Find the percentage of the overkill damage over the max health, the higher the percentage the greater the force
        float force;
        
        if (explodesOnDeath)
        {
          force = 0.5f;
        }
        
        else
        {
          force = overkillDamage / maxHealth;

          force = Mathf.Clamp(force, 0.1f, 0.5f);
        }
       
        ragdoll.GetComponentInChildren<Rigidbody>().AddForce((transform.position - damageDirection).normalized * force * 40000f);
        ragdollSpawned = true;

        if (GetComponent<CrystalSeeker>())
        {
          GetComponent<CrystalSeeker>().CrystalSeekerDead(ragdoll);
        }
      }
  
      Destroy(gameObject);
    }
  }

  private void Update()
  {
    ModifyHealth(regeneration * Time.deltaTime, Vector3.zero);

    if (kill)
    {
      ModifyHealth(-CurrentHealth, Vector3.zero);
    }

    // Layer 10 is invisible, enemies are set to invisible if they are inside fog of war, we shouldn't see their health bars
    if (CurrentHealth == maxHealth || gameObject.layer == 10 || CurrentHealth <= 0)
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
    }
  }

  public void SetBoostedValues(BoostValues boostValues)
  {
    maxHealth += (gameManager.CurrentRound - 1) * boostValues.healthModifier * ogHealth;
    regeneration += (gameManager.CurrentRound - 1) * boostValues.regeneratonModifier * ogRegeneration;

    CurrentHealth = maxHealth;
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
