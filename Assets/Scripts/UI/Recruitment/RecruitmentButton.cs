using UnityEngine;
using UnityEngine.UI;

public class RecruitmentButton : MonoBehaviour
{
  [SerializeField]
  private GameObject recruitableUnit = null;
  [SerializeField]
  private UnitManager unitManager = null;

  private void Start()
  {
    GetComponent<Button>().onClick.AddListener(AddToUnitPanel);
  }

  private void LateUpdate()
  {

  }

  private void AddToUnitPanel()
  {
    unitManager.AddUnitToPanel(recruitableUnit, GetComponent<Image>().sprite);
  }
}
