using UnityEngine;
using UnityEngine.UI;

public class MinerManager : MonoBehaviour
{
  [SerializeField]
  private Image minerHealthBar;
  [SerializeField]
  private CrystalSeeker minerPrefab;

  public float CurrentMinerHealth { get; private set; }
  public float MaxMinerHealth { get; private set; }

  private void Awake()
  {
    CurrentMinerHealth = MaxMinerHealth = minerPrefab.GetComponent<Health>().MaxHealth;
  }

  public void HandleMinerHealthChanged(float pct, float currentHealth)
  {
    CurrentMinerHealth = currentHealth;
    minerHealthBar.fillAmount = pct;
  }

  public void UpgradeMinerHealth(float healthIncrease, float newMaxHealth)
  {
    CurrentMinerHealth += healthIncrease;
    MaxMinerHealth = newMaxHealth;
    minerHealthBar.fillAmount = CurrentMinerHealth / MaxMinerHealth;
  }

  public void RepairMiner(int healthRepaired)
  {
    CurrentMinerHealth += healthRepaired;
    Mathf.Clamp(CurrentMinerHealth, 0, MaxMinerHealth);
    minerHealthBar.fillAmount = CurrentMinerHealth / MaxMinerHealth;
  }
}
