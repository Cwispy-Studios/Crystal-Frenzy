using UnityEngine;

[System.Serializable]
public class UpgradeProperties
{
  [Header("Health components")]
  // Health components
  public int health;

  [Header("Attack components")]
  // Attack components
  public int damage;
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

  public void Reset()
  {
    health = 0;

    damage = 0;
    attackSpeed = 0;
    attackRange = 0;
    detectRange = 0;

    fovRange = 0;

    speed = 0;

    cost = 0;
  }
}
