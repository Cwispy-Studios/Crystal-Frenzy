using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class UnitManager : MonoBehaviour
{
  private readonly Vector3 ICON_SIZE = new Vector3(70f, 70f);
  private readonly Vector3 SELECTED_ICON_SIZE = new Vector3(80f, 80f);
  private readonly Vector3 FIRST_ICON_POS = new Vector3(15f, 0);
  private readonly float ICON_GAP_X = 80f;
  private readonly float ICON_GAP_Y = 95f;

 
  [SerializeField]
  private GameManager gameManager = null;
  [SerializeField]
  private GameObject unitButtonPrefab = null;

  private ResourceManager resourceManager = null;
  private List<GameObject> unitButtonsList = null;
  private GameObject assemblySpace = null;

  private GameObject playerCamera = null;

  private List<GameObject> stabbyList = new List<GameObject>();
  private List<GameObject> shootyList = new List<GameObject>();
  private List<GameObject> bruteList = new List<GameObject>();
  private List<GameObject> warlockList = new List<GameObject>();

  public List<GameObject> SelectedUnits { get; } = new List<GameObject>();
  public List<GameObject> UnselectedUnits { get; } = new List<GameObject>();

  private void Awake()
  {
    resourceManager = gameManager.GetComponent<ResourceManager>();
    unitButtonsList = new List<GameObject>();
    playerCamera = Camera.main.gameObject;
  }

  public void AddUnitToPanel(GameObject unitPrefab, Sprite buttonImage)
  {
    int unitPoints = unitPrefab.GetComponent<RecruitableUnit>().unitPoints;

    UPGRADE_TYPE[] affectedByUpgrades = unitPrefab.GetComponent<Upgradable>().affectedByUpgrades;

    for (int i = 0; i < affectedByUpgrades.Length; ++i)
    {
      // Retrieve the ugrade properties
      UpgradeProperties[] upgradeProperties = GameManager.upgradeManager.RetrieveUpgradeProperties(affectedByUpgrades[i]);

      if (upgradeProperties != null)
      {
        for (int up = 0; up < upgradeProperties.Length; ++up)
        {
          unitPoints += upgradeProperties[up].cost;
        }
      }
    }
    
    // Check if the unit can be recruited within the unit cap
    if (resourceManager.ArmySize + unitPoints <= resourceManager.UnitCap)
    {
      // Create the button and the associated unit
      GameObject unitButton = Instantiate(unitButtonPrefab);
      GameObject unit = Instantiate(unitPrefab);

      // Assign the unit button and the unit together
      unitButton.GetComponent<UnitButton>().Unit = unit;
      unit.GetComponent<RecruitableUnit>().UnitButton = unitButton;

      // Set the image of the unit button
      unitButton.GetComponent<Image>().sprite = buttonImage;
      // Since we are still in Preparation Phase, clicking on the button removes the instance of it from the Army Roster
      unitButton.GetComponent<Button>().onClick.AddListener(delegate { RemoveUnitFromRoster(unit, unitButton); });
      // Set UnitPanel as the parent
      unitButton.transform.SetParent(transform, false);

      // Add the button to the unit list
      unitButtonsList.Add(unitButton);
      AddUnitToList(unit);

      // Update army size
      resourceManager.UpdateArmySize(unitPoints);

      SortUnits();

      UpdateButtonPositions();

      PositionUnitsOnAssemblySpace();
    }
  }

  private void RemoveUnitFromRoster(GameObject removeUnit, GameObject removeButton)
  {
    unitButtonsList.Remove(removeButton);
    RemoveUnitFromList(removeUnit);

    resourceManager.UpdateArmySize(-removeUnit.GetComponent<RecruitableUnit>().unitPoints);

    Destroy(removeUnit);
    Destroy(removeButton);

    SortUnits();
    UpdateButtonPositions();
    PositionUnitsOnAssemblySpace();
  }

  public void KillUnit(GameObject removeButton, int unitPoints)
  {
    SelectedUnits.Remove(removeButton.GetComponent<UnitButton>().Unit);
    UnselectedUnits.Remove(removeButton.GetComponent<UnitButton>().Unit);
    RemoveUnitFromList(removeButton.GetComponent<UnitButton>().Unit);
    unitButtonsList.Remove(removeButton);

    resourceManager.UpdateArmySize(-unitPoints);
    
    UpdateButtonPositions();
  }

  private void UpdateButtonPositions()
  {
    // Holds the button positions
    Vector3 buttonPos = FIRST_ICON_POS;
    float fromPanelOffset = FIRST_ICON_POS.x;
    float panelWidth = GetComponentInParent<RectTransform>().sizeDelta.x;

    for (int i = 0; i < SelectedUnits.Count; ++i)
    {
      GameObject unitButton = SelectedUnits[i].GetComponent<RecruitableUnit>().UnitButton;

      // Set button dimensions and scale
      unitButton.GetComponent<RectTransform>().sizeDelta = SELECTED_ICON_SIZE;
      unitButton.transform.localScale = new Vector3(1f, 1f, 1f);

      // Check if icon will overflow the end of the unit panel on x-axis
      if (buttonPos.x + ICON_GAP_X > panelWidth - fromPanelOffset)
      {
        buttonPos.x = FIRST_ICON_POS.x;
        buttonPos.y -= ICON_GAP_Y;
      }

      unitButton.GetComponent<RectTransform>().anchoredPosition = buttonPos;

      buttonPos.x += ICON_GAP_X;

      unitButton.GetComponent<UnitButton>().SetHealthBarTransform(SELECTED_ICON_SIZE, true);
    }

    for (int i = 0; i < UnselectedUnits.Count; ++i)
    {
      GameObject unitButton = UnselectedUnits[i].GetComponent<RecruitableUnit>().UnitButton;

      // Set button dimensions and scale
      unitButton.GetComponent<RectTransform>().sizeDelta = ICON_SIZE;
      unitButton.transform.localScale = new Vector3(1f, 1f, 1f);

      // Check if icon will overflow the end of the unit panel on x-axis
      if (buttonPos.x + ICON_GAP_X > panelWidth - fromPanelOffset)
      {
        buttonPos.x = FIRST_ICON_POS.x;
        buttonPos.y -= ICON_GAP_Y;
      }

      unitButton.GetComponent<RectTransform>().anchoredPosition = buttonPos;

      buttonPos.x += ICON_GAP_X;

      unitButton.GetComponent<UnitButton>().SetHealthBarTransform(ICON_SIZE, false);
    }
  }

  private void SortUnits()
  {
    SelectedUnits.Clear();
    UnselectedUnits.Clear();

    // Goes through the unit lists in the order we want the units to be placed in the unit panel
    SortSelectedUnselectedList(bruteList);
    SortSelectedUnselectedList(stabbyList);
    SortSelectedUnselectedList(warlockList);
    SortSelectedUnselectedList(shootyList);
  }

  public void SortSelectedUnselectedList(List<GameObject> unitList)
  {
    for (int i = 0; i < unitList.Count; ++i)
    {
      if (unitList[i].GetComponent<Selectable>().selectStatus == Selectable.SELECT_STATUS.SELECTED)
      {
        SelectedUnits.Add(unitList[i]);
      }

      else
      {
        UnselectedUnits.Add(unitList[i]);
      }
    }
  }

  public void UpdateSelectionLists()
  {
    SortUnits();
    UpdateButtonPositions();
  }

  public void AddUnitToList(GameObject unit)
  {
    switch (unit.GetComponent<UnitType>().unitType)
    {
      case UNIT_TYPE.STABBY:
        stabbyList.Add(unit);
        break;

      case UNIT_TYPE.SHOOTY:
        shootyList.Add(unit);
        break;

      case UNIT_TYPE.BRUTE:
        bruteList.Add(unit);
        break;

      case UNIT_TYPE.WARLOCK:
        warlockList.Add(unit);
        break;
    }
  }

  public void RemoveUnitFromList(GameObject unit)
  {
    switch (unit.GetComponent<UnitType>().unitType)
    {
      case UNIT_TYPE.STABBY:
        stabbyList.Remove(unit);
        break;

      case UNIT_TYPE.SHOOTY:
        shootyList.Remove(unit);
        break;

      case UNIT_TYPE.BRUTE:
        bruteList.Remove(unit);
        break;

      case UNIT_TYPE.WARLOCK:
        warlockList.Remove(unit);
        break;
    }
  }

  private void PositionUnitsOnAssemblySpace()
  {
    if (unitButtonsList.Count > 0)
    {
      Vector3 assemblyPos = assemblySpace.transform.position;
      Vector3 frontAssemblyPos = assemblyPos + (assemblySpace.transform.forward * (assemblySpace.GetComponent<Renderer>().bounds.size.z / 2));

      for (int i = 0; i < unitButtonsList.Count; ++i)
      {
        UnitButton unitButton = unitButtonsList[i].GetComponent<UnitButton>();
        GameObject unit = unitButton.GetComponent<UnitButton>().Unit;

        // Get the size of the unit
        float unitRadius = unit.GetComponent<NavMeshAgent>().radius * ((transform.lossyScale.x + transform.lossyScale.z) / 2f);

        Vector3 assemblyPosition;

        // If it is even numbered
        if (unitButtonsList.Count % 2 == 0)
        {
          // Even numbered units are placed on the left (- radius), odd numbered units are placed on the right (+ radius)
          if (i % 2 == 0)
          {
            assemblyPosition = frontAssemblyPos + (-assemblySpace.transform.right * unitRadius) +
              (-assemblySpace.transform.right * unitRadius * 2 * (i / 2));

          }

          else
          {
            assemblyPosition = frontAssemblyPos + (assemblySpace.transform.right * unitRadius) +
              (assemblySpace.transform.right * unitRadius * 2 * (i / 2));
          }
        }

        // Odd numbered, unit can be placed in the center
        else
        {
          if (i == 0)
          {
            assemblyPosition = frontAssemblyPos;
          }

          else
          {
            // Even numbered units are placed on the left (- radius), odd numbered units are placed on the right (+ radius)
            if (i % 2 == 0)
            {
              assemblyPosition = frontAssemblyPos + (-assemblySpace.transform.right * unitRadius * 2 * (i / 2));
            }

            else
            {
              assemblyPosition = frontAssemblyPos + (assemblySpace.transform.right * unitRadius * 2 * ((i / 2) + 1));
            }
          }
        }

        unit.transform.position = assemblyPosition;
        unit.GetComponent<NavMeshAgent>().Warp(assemblyPosition);
      }
    }
  }

  public void SetAssemblySpace(GameObject setAssemblySpace)
  {
    assemblySpace = setAssemblySpace;
  }

  private void SelectUnitButton(GameObject selectedUnit, GameObject selectedButton)
  {
    CameraProperties.selectionDisabled = true;
    CameraObjectSelection selector = playerCamera.GetComponent<CameraObjectSelection>();

    // If shift is not held, we are selecting that unit alone
    if (!Input.GetKey(KeyCode.LeftShift))
    {
      selector.ClearSelectionList();
      selector.AddObjectToSelectionList(selectedUnit);
    }

    // If shift is held, we add this unit to the selection list
    else
    {
      selector.AddObjectToSelectionList(selectedUnit);
    }

    // If double click, center on the unit

    UpdateSelectionLists();
  }

  public void SetUnitButtonsToCombat()
  {
    for (int i = 0; i < unitButtonsList.Count; ++i)
    {
      Button unitButton = unitButtonsList[i].GetComponent<Button>();

      unitButton.onClick.RemoveAllListeners();
      unitButton.onClick.AddListener((delegate { SelectUnitButton(unitButton.GetComponent<UnitButton>().Unit, unitButton.gameObject); }));
    }
  }

  public void RemoveAllUnits()
  {
    stabbyList.Clear();
    shootyList.Clear();
    bruteList.Clear();
    warlockList.Clear();

    for (int i = unitButtonsList.Count - 1; i >= 0; --i)
    {
      Destroy(unitButtonsList[i].GetComponent<UnitButton>().Unit);
      Destroy(unitButtonsList[i]);

      unitButtonsList.Remove(unitButtonsList[i]);
    }

    resourceManager.UpdateArmySize(-resourceManager.ArmySize);
  }
}
