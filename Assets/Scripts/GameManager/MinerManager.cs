using UnityEngine;
using UnityEngine.UI;

public class MinerManager : MonoBehaviour
{
  [SerializeField]
  private Image minerHealthBar;
  [SerializeField]
  private CrystalSeeker minerPrefab;

  public float CurrentMinerHealth { get; private set; }
  private float maxMinerHealth;

  private void Awake()
  {
    CurrentMinerHealth = maxMinerHealth = minerPrefab.GetComponent<Health>().MaxHealth;
  }

  public void HandleMinerHealthChanged(float pct, float currentHealth)
  {
    CurrentMinerHealth = currentHealth;
    minerHealthBar.fillAmount = pct;
  }

  public void UpgradeMinerHealth(float healthIncrease, float newMaxHealth)
  {
    CurrentMinerHealth += healthIncrease;
    maxMinerHealth = newMaxHealth;
    minerHealthBar.fillAmount = CurrentMinerHealth / maxMinerHealth;
  }
}
