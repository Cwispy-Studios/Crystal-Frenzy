using UnityEngine;
using UnityEngine.UI;

public class BushManager : MonoBehaviour
{
  [SerializeField]
  private Image bushHealthBar = null, bushProgressBar = null;
  [SerializeField]
  private Text bushHealthText = null, bushProgressText = null;

  public float CurrentBushHealth { get; private set; }
  public float MaxBushHealth { get; private set; }

  private FMODUnity.StudioEventEmitter musicEmitter;

  private bool bushHealthFound = false;
  private GameObject bushReference = null;
  private float unboostedHealth = 0;

  private void Awake()
  {
    CurrentBushHealth = MaxBushHealth = 450f;
    bushHealthText.text = Mathf.CeilToInt(CurrentBushHealth).ToString() + " / " + Mathf.CeilToInt(MaxBushHealth).ToString();

    musicEmitter = Camera.main.GetComponent<FMODUnity.StudioEventEmitter>();
  }

  private void Update()
  {
    if (!bushHealthFound && bushReference && unboostedHealth != bushReference.GetComponent<Health>().MaxHealth)
    {
      FindBoostedMaxHealth();
    }
  }

  public void Initialise(GameObject bush)
  {
    bushReference = bush;
    unboostedHealth = bush.GetComponent<Health>().MaxHealth;

    bushHealthFound = false;
    
    bushHealthBar.fillAmount = 1;
  }

  public void HandleBushHealthChanged(float pct, float currentHealth)
  {
    CurrentBushHealth = currentHealth;
    bushHealthBar.fillAmount = pct;

    musicEmitter.SetParameter("Enemy Crystal Seeker Health", pct);

    bushHealthText.text = Mathf.CeilToInt(CurrentBushHealth).ToString() + " / " + Mathf.CeilToInt(MaxBushHealth).ToString();
  }
  
  private void FindBoostedMaxHealth()
  {
    CurrentBushHealth = MaxBushHealth = bushReference.GetComponent<Health>().MaxHealth;
    bushHealthFound = true;
  }

  public void UpdateProgressPct(float pct)
  {
    if (bushProgressBar == null || bushProgressText == null)
      return;

    bushProgressBar.fillAmount = pct;
    bushProgressText.text = (pct * 100f).ToString("F1") + "%";

    musicEmitter.SetParameter("Enemy Crystal Seeker Progress", pct);
  }

  public void ResetHealth()
  {
    bushHealthBar.fillAmount = 1;

    musicEmitter.SetParameter("Enemy Crystal Seeker Health", 1);

    bushReference = null;
  }
}
