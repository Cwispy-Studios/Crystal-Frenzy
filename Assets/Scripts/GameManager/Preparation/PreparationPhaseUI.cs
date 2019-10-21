using UnityEngine;
using UnityEngine.UI;

public class PreparationPhaseUI : MonoBehaviour
{
  [SerializeField]
  private GameObject selectArmyButton = null;
  [SerializeField]
  private GameObject armyRecruitmentPanel = null;
  [SerializeField]
  private GameObject backToSelectNodesButton = null;
  [SerializeField]
  private GameObject attackButton = null;

  private void Awake()
  {
    selectArmyButton.GetComponent<Button>().onClick.AddListener(FindObjectOfType<GameManager>().NodeSelected);
    backToSelectNodesButton.GetComponent<Button>().onClick.AddListener(FindObjectOfType<GameManager>().ReturnToNodeSelection);
    attackButton.GetComponent<Button>().onClick.AddListener(FindObjectOfType<GameManager>().BeginEscort);
  }

  public void SelectNodeUI()
  {
    selectArmyButton.SetActive(true);
    armyRecruitmentPanel.SetActive(false);
    backToSelectNodesButton.SetActive(false);
    attackButton.SetActive(false);
  }

  public void SelectArmyUI()
  {
    selectArmyButton.SetActive(false);
    armyRecruitmentPanel.SetActive(true);
    backToSelectNodesButton.SetActive(true);
    attackButton.SetActive(true);
  }

  public void SetSelectArmyButtonInteractable(bool interactable)
  {
    selectArmyButton.GetComponent<Button>().interactable = interactable;
  }

  public void SetAttackButtonInteractable(bool interactable)
  {
    attackButton.GetComponent<Button>().interactable = interactable;
  }

  public void DisableUI()
  {
    armyRecruitmentPanel.SetActive(false);
    backToSelectNodesButton.SetActive(false);
    attackButton.SetActive(false);
  }
}
