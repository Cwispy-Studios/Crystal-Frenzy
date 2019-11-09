using UnityEngine;
using UnityEngine.UI;

public class MinerManager : MonoBehaviour
{
  [SerializeField]
  private Image minerHealthBar = null, minerProgressBar = null;
  [SerializeField]
  private CrystalSeeker minerPrefab = null;
  [SerializeField]
  private Text minerHealthText = null, minerProgressText = null;

  public float CurrentMinerHealth { get; private set; }
  public float MaxMinerHealth { get; private set; }

  private float destroyedMinerHealthPct = 0.1f;

  private FMODUnity.StudioEventEmitter musicEmitter;

  private void Awake()
  {
    CurrentMinerHealth = MaxMinerHealth = minerPrefab.GetComponent<Health>().MaxHealth;
    minerHealthText.text = Mathf.FloorToInt(CurrentMinerHealth).ToString() + " / " + Mathf.CeilToInt(MaxMinerHealth).ToString();

    minerProgressBar.fillAmount = 0;
    minerProgressText.text = "0%";

    musicEmitter = Camera.main.GetComponent<FMODUnity.StudioEventEmitter>();
  }

  public void HandleMinerHealthChanged(float pct, float currentHealth)
  {
    CurrentMinerHealth = currentHealth;
    minerHealthBar.fillAmount = pct;

    musicEmitter.SetParameter("Player Crystal Seeker Health", pct);

    minerHealthText.text = Mathf.FloorToInt(CurrentMinerHealth).ToString() + " / " + Mathf.CeilToInt(MaxMinerHealth).ToString();
  }

  public void UpdateProgressPct(float pct)
  {
    if (minerProgressBar == null)
    {
      return;
    }

    minerProgressBar.fillAmount = pct;
    minerProgressText.text = (pct * 100f).ToString("F1") + "%";
    musicEmitter.SetParameter("Player Crystal Seeker Progress", pct);
  }

  public void UpgradeMinerHealth(float healthIncrease)
  {
    CurrentMinerHealth += healthIncrease;
    MaxMinerHealth += healthIncrease;
    minerHealthBar.fillAmount = CurrentMinerHealth / MaxMinerHealth;

    musicEmitter.SetParameter("Player Crystal Seeker Health", CurrentMinerHealth / MaxMinerHealth);

    minerHealthText.text = Mathf.FloorToInt(CurrentMinerHealth).ToString() + " / " + Mathf.CeilToInt(MaxMinerHealth).ToString();
  }

  public void RepairMiner(int healthRepaired)
  {
    CurrentMinerHealth += healthRepaired;
    Mathf.Clamp(CurrentMinerHealth, 0, MaxMinerHealth);
    minerHealthBar.fillAmount = CurrentMinerHealth / MaxMinerHealth;

    musicEmitter.SetParameter("Player Crystal Seeker Health", CurrentMinerHealth / MaxMinerHealth);

    minerHealthText.text = Mathf.FloorToInt(CurrentMinerHealth).ToString() + " / " + Mathf.CeilToInt(MaxMinerHealth).ToString();
  }

  public void MinerDestroyed()
  {
    CurrentMinerHealth = MaxMinerHealth * destroyedMinerHealthPct;
    minerHealthBar.fillAmount = destroyedMinerHealthPct;

    musicEmitter.SetParameter("Player Crystal Seeker Health", destroyedMinerHealthPct);

    minerHealthText.text = Mathf.FloorToInt(CurrentMinerHealth).ToString() + " / " + Mathf.CeilToInt(MaxMinerHealth).ToString();
  }
}
