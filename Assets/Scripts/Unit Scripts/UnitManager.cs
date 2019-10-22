using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class UnitManager : MonoBehaviour
{
  private readonly Vector3 ICON_SIZE = new Vector3(70f, 70f);
  private readonly Vector3 FIRST_ICON_POS = new Vector3(-640f, 40f);
  private readonly float ICON_GAP = 80f;

  private readonly int MAX_ICONS_X = 17;

  [SerializeField]
  private GameManager gameManager = null;
  [SerializeField]
  private GameObject unitButtonPrefab = null;

  private ResourceManager resourceManager = null;
  private List<GameObject> unitButtonsList = null;
  private GameObject assemblySpace = null;

  private GameObject playerCamera = null;

  private void Awake()
  {
    resourceManager = gameManager.GetComponent<ResourceManager>();
    unitButtonsList = new List<GameObject>();
    playerCamera = Camera.main.gameObject;
  }

  public void AddUnitToPanel(GameObject unitPrefab, Sprite buttonImage)
  {
    int unitPoints = unitPrefab.GetComponent<RecruitableUnit>().unitPoints;
    
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
      // Set button dimensions and scale
      unitButton.GetComponent<RectTransform>().sizeDelta = ICON_SIZE;
      unitButton.transform.localScale = new Vector3(1f, 1f, 1f);

      // Add the button to the unit list
      unitButtonsList.Add(unitButton);

      // Update army size
      resourceManager.UpdateArmySize(unitPoints);

      UpdateButtonPositions();

      PositionUnitsOnAssemblySpace();

      //if (GameManager.currentPhaseCycle == 2)
      //  Time.timeScale = 0.01f;
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

  private void RemoveUnitFromRoster(GameObject removeUnit, GameObject removeButton)
  {
    unitButtonsList.Remove(removeButton);

    resourceManager.UpdateArmySize(-removeUnit.GetComponent<RecruitableUnit>().unitPoints);

    Destroy(removeUnit);
    Destroy(removeButton);

    UpdateButtonPositions();
    PositionUnitsOnAssemblySpace();
  }

  public void KillUnit(GameObject removeButton, int unitPoints)
  {
    unitButtonsList.Remove(removeButton);

    resourceManager.UpdateArmySize(-unitPoints);
    
    UpdateButtonPositions();
  }

  private void UpdateButtonPositions()
  {
    if (unitButtonsList.Count > 0)
    {
      // TODO: Create seperate lists for every unit type, then sort them based on unit types

      for (int i = 0; i < unitButtonsList.Count; ++i)
      {
        Vector3 buttonPos = FIRST_ICON_POS;

        if (i < MAX_ICONS_X)
        {
          buttonPos.x += ICON_GAP * i;
        }

        else
        {
          buttonPos.x += ICON_GAP * (i - MAX_ICONS_X);
          buttonPos.y -= ICON_GAP;
        }

        // TODO: Make icons smaller if unit roster size exceeds 17 * 2

        unitButtonsList[i].GetComponent<RectTransform>().anchoredPosition = buttonPos;
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
    for (int i = unitButtonsList.Count - 1; i >= 0; --i)
    {
      resourceManager.UpdateArmySize(-unitButtonsList[i].GetComponent<UnitButton>().Unit.GetComponent<RecruitableUnit>().unitPoints);

      Destroy(unitButtonsList[i].GetComponent<UnitButton>().Unit);
      Destroy(unitButtonsList[i]);

      unitButtonsList.Remove(unitButtonsList[i]);
    }
  }
}
