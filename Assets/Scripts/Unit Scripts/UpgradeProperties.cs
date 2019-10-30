using UnityEngine;

[System.Serializable]
public class UpgradeProperties
{
  public int upgradeLevel;

  [Header("Health components")]
  // Health components
  public float health;
  public float regeneraton;

  [Header("Attack components")]
  // Attack components
  public float damage;
  public float attackSpeed;
  public float attackRange;
  public float detectRange;
  public bool isAoe;
  public float aoeRadius;
  public float aoeDmgPct;

  [Header("Heal components")]
  // Heal components
  public bool isHealer;
  public float healPct;

  [Header("Status components")]
  // Status components
  public bool addsSlow;
  public bool addsCurse;

  [Header("Slow components")]
  public float attackSlowPct;
  public float moveSlowPct;
  public float slowDuration;

  [Header("Curse components")]
  public float attackReducPct;
  public float curseDuration;

  [Header("FOV components")]
  // FOV components
  public float fovRange;

  [Header("Navigation components")]
  // Navigation components
  public float speed;

  [Header("Recruitment components")]
  // Recruitment components
  public int cost;
}
