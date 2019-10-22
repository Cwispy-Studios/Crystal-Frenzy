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
  [SerializeField]
  private GameObject defendButton = null;

  private void Awake()
  {
    GameManager gameManager = FindObjectOfType<GameManager>();
    selectArmyButton.GetComponent<Button>().onClick.AddListener(gameManager.NodeSelected);
    backToSelectNodesButton.GetComponent<Button>().onClick.AddListener(gameManager.ReturnToNodeSelection);
    attackButton.GetComponent<Button>().onClick.AddListener(gameManager.BeginEscort);
    defendButton.GetComponent<Button>().onClick.AddListener(gameManager.BeginDefense);
  }

  public void SelectNodeUI()
  {
    selectArmyButton.SetActive(true);
    armyRecruitmentPanel.SetActive(false);
    backToSelectNodesButton.SetActive(false);
    attackButton.SetActive(false);
  }

  public void SelectArmyUI(bool conquered)
  {
    selectArmyButton.SetActive(false);
    armyRecruitmentPanel.SetActive(true);

    if (!conquered)
    {
      backToSelectNodesButton.SetActive(true);
    }
    
    attackButton.SetActive(true);
  }

  public void DefenseSelectArmyUI()
  {
    armyRecruitmentPanel.SetActive(true);
    defendButton.SetActive(true);
  }

  public void SetSelectArmyButtonInteractable(bool interactable)
  {
    selectArmyButton.GetComponent<Button>().interactable = interactable;
  }

  public void SetAttackButtonInteractable(bool interactable)
  {
    attackButton.GetComponent<Button>().interactable = interactable;
  }

  public void SetDefendButtonInteractable(bool interactable)
  {
    defendButton.GetComponent<Button>().interactable = interactable;
  }

  public void DisableUI()
  {
    armyRecruitmentPanel.SetActive(false);
    backToSelectNodesButton.SetActive(false);
    attackButton.SetActive(false);
  }

  public void DefenseDisableUI()
  {
    armyRecruitmentPanel.SetActive(false);
    defendButton.SetActive(false);
  }
}
