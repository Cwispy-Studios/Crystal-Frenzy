using UnityEngine;
using UnityEngine.UI;

public class MinerManager : MonoBehaviour
{
  [SerializeField]
  private Image minerHealthBar = null;
  [SerializeField]
  private CrystalSeeker minerPrefab = null;
  [SerializeField]
  private Text minerHealthText = null;

  public float CurrentMinerHealth { get; private set; }
  public float MaxMinerHealth { get; private set; }

  private float destroyedMinerHealthPct = 0.1f;

  private void Awake()
  {
    CurrentMinerHealth = MaxMinerHealth = minerPrefab.GetComponent<Health>().MaxHealth;
    minerHealthText.text = Mathf.CeilToInt(CurrentMinerHealth).ToString() + " / " + Mathf.CeilToInt(MaxMinerHealth).ToString();
  }

  public void HandleMinerHealthChanged(float pct, float currentHealth)
  {
    CurrentMinerHealth = currentHealth;
    minerHealthBar.fillAmount = pct;

    minerHealthText.text = Mathf.CeilToInt(CurrentMinerHealth).ToString() + " / " + Mathf.CeilToInt(MaxMinerHealth).ToString();
  }

  public void UpgradeMinerHealth(float healthIncrease, float newMaxHealth)
  {
    CurrentMinerHealth += healthIncrease;
    MaxMinerHealth = newMaxHealth;
    minerHealthBar.fillAmount = CurrentMinerHealth / MaxMinerHealth;

    minerHealthText.text = Mathf.CeilToInt(CurrentMinerHealth).ToString() + " / " + Mathf.CeilToInt(MaxMinerHealth).ToString();
  }

  public void RepairMiner(int healthRepaired)
  {
    CurrentMinerHealth += healthRepaired;
    Mathf.Clamp(CurrentMinerHealth, 0, MaxMinerHealth);
    minerHealthBar.fillAmount = CurrentMinerHealth / MaxMinerHealth;

    minerHealthText.text = Mathf.CeilToInt(CurrentMinerHealth).ToString() + " / " + Mathf.CeilToInt(MaxMinerHealth).ToString();
  }

  public void MinerDestroyed()
  {
    CurrentMinerHealth = MaxMinerHealth * destroyedMinerHealthPct;
    minerHealthBar.fillAmount = destroyedMinerHealthPct;

    minerHealthText.text = Mathf.CeilToInt(CurrentMinerHealth).ToString() + " / " + Mathf.CeilToInt(MaxMinerHealth).ToString();
  }
}
