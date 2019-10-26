using UnityEngine;

[System.Serializable]
public class UpgradeProperties
{
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
