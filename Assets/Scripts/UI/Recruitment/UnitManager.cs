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

  private void Start()
  {
    resourceManager = gameManager.GetComponent<ResourceManager>();
    unitButtonsList = new List<GameObject>();
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
    }
  }

  private void PositionUnitsOnAssemblySpace()
  {
    if (unitButtonsList.Count > 0)
    {
      Vector3 assemblyPos = assemblySpace.transform.position;
      Vector3 frontAssemblyPos = assemblyPos + (assemblySpace.transform.forward * (assemblySpace.GetComponent<Renderer>().bounds.size.z / 2));
      Debug.Log(assemblySpace.GetComponent<Renderer>().bounds);

      for (int i = 0; i < unitButtonsList.Count; ++i)
      {
        Debug.Log(unitButtonsList[i].name);
        UnitButton unitButton = unitButtonsList[i].GetComponent<UnitButton>();
        // Get the size of the unit
        float unitRadius = unitButton.Unit.GetComponent<NavMeshAgent>().radius * ((transform.lossyScale.x + transform.lossyScale.z) / 2f);

        GameObject unit = unitButton.GetComponent<UnitButton>().Unit;

        // If it is even numbered
        if (unitButtonsList.Count % 2 == 0)
        {
          // Even numbered units are placed on the left (- radius), odd numbered units are placed on the right (+ radius)
          if (i % 2 == 0)
          {
            unit.transform.position = frontAssemblyPos + (-assemblySpace.transform.right * unitRadius) + 
              (-assemblySpace.transform.right * unitRadius * 2 * (i / 2));
          }

          else
          {
            unit.transform.position = frontAssemblyPos + (assemblySpace.transform.right * unitRadius) +
              (assemblySpace.transform.right * unitRadius * 2 * (i / 2));
          }
        }

        // Odd numbered, unit can be placed in the center
        else
        {
          if (i == 0)
          {
            unit.transform.position = frontAssemblyPos;
          }

          else
          {
            // Even numbered units are placed on the left (- radius), odd numbered units are placed on the right (+ radius)
            if (i % 2 == 0)
            {
              unit.transform.position = frontAssemblyPos + (-assemblySpace.transform.right * unitRadius * 2 * (i / 2));
            }

            else
            {
              unit.transform.position = frontAssemblyPos + (assemblySpace.transform.right * unitRadius * 2 * ((i / 2) + 1));
            }
          }
        }
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
}
