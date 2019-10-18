using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

      // TODO: Set the unit's transform on the roster area

      // Add the button to the unit list
      unitButtonsList.Add(unitButton);

      // Update army size
      resourceManager.UpdateArmySize(unitPoints);

      UpdateButtonPositions();
    }
  }

  private void RemoveUnitFromRoster(GameObject removeUnit, GameObject removeButton)
  {
    unitButtonsList.Remove(removeButton);

    resourceManager.UpdateArmySize(-removeUnit.GetComponent<RecruitableUnit>().unitPoints);

    Destroy(removeUnit);
    Destroy(removeButton);

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
}
