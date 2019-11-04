using UnityEngine;
using UnityEngine.UI;

public class RepairSlider : MonoBehaviour
{
  private float GOLD_COST_PER_HEALTH = 0.35f;

  [SerializeField]
  private Text
    currentHealthText = null,
    maxHealthText = null,
    repairHealthText = null,
    goldCostText = null;

  [SerializeField]
  private Button repairButton = null;

  private int repairHealthAmount = 0;
  private int repairCost = 0;

  [FMODUnity.EventRef]
  public string repairSliderClickSound = null;
  private FMOD.Studio.EventInstance repairSliderClickInstance;

  private void Awake()
  {
    GetComponent<Slider>().onValueChanged.AddListener(CalculateRepairCost);
    repairButton.onClick.AddListener(Repair);


    repairSliderClickInstance = FMODUnity.RuntimeManager.CreateInstance(repairSliderClickSound);

    FMODUnity.RuntimeManager.AttachInstanceToGameObject(repairSliderClickInstance, Camera.main.transform, (Rigidbody)null);
  }

  private void OnEnable()
  {
    InitialiseSlider();
  }

  private void InitialiseSlider()
  {
    currentHealthText.text = GameManager.minerManager.CurrentMinerHealth.ToString();
    maxHealthText.text = GameManager.minerManager.MaxMinerHealth.ToString();

    if (GameManager.minerManager.CurrentMinerHealth == GameManager.minerManager.MaxMinerHealth)
    {
      GetComponent<Slider>().value = 0;
      GetComponent<Slider>().interactable = false;
      repairButton.interactable = false;
      repairHealthText.text = "";
      goldCostText.text = "NONE";
    }

    else
    {
      GetComponent<Slider>().interactable = true;
    }
  }

  private void CalculateRepairCost(float sliderValue)
  {
    float missingHealth = GameManager.minerManager.MaxMinerHealth - GameManager.minerManager.CurrentMinerHealth;
    float repairHealthAmountFloat = missingHealth * sliderValue;
    repairHealthAmount = Mathf.CeilToInt(missingHealth * sliderValue);

    repairHealthText.text = Mathf.CeilToInt(repairHealthAmountFloat + GameManager.minerManager.CurrentMinerHealth).ToString();
    repairCost = Mathf.CeilToInt((repairHealthAmount * GOLD_COST_PER_HEALTH));
    goldCostText.text = repairCost.ToString();

    if (repairCost > 0 && repairCost <= GameManager.resourceManager.Gold)
    {
      repairButton.interactable = true;
    }

    else
    {
      repairButton.interactable = false;
    }

    repairSliderClickInstance.setPitch(1 + (sliderValue * 0.5f));
    repairSliderClickInstance.start();
  }

  private void Repair()
  {
    GameManager.resourceManager.SpendGold(repairCost);
    GameManager.minerManager.RepairMiner(repairHealthAmount);

    GetComponent<Slider>().value = 0;
    InitialiseSlider();
  }
}
