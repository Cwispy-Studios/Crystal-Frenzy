using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructedBuildingsPanel : MonoBehaviour
{
  private readonly Vector3 ICON_SIZE = new Vector3(62f, 62f);
  private readonly Vector3 FIRST_ICON_POS = new Vector3(-32f, 97f);
  private readonly float ICON_GAP_X = 64f;
  private readonly float ICON_GAP_Y = 65f;

  private List<Button> constructedBuildingsList = null;

  [SerializeField]
  private ConstructedBuildingButton buttonPrefab = null;

  private void Awake()
  {
    constructedBuildingsList = new List<Button>();
  }

  public void AddNewConstructedBuilding(GameObject constructedBuilding, Sprite iconImage)
  {
    var newButton = Instantiate(buttonPrefab, transform);
    newButton.SetBuilding(constructedBuilding);
    newButton.GetComponent<Image>().sprite = iconImage;

    Vector2 anchoredPos = newButton.GetComponent<RectTransform>().anchoredPosition;

    int secondColumnCount = 3;
    anchoredPos = FIRST_ICON_POS;

    // First column
    if (constructedBuildingsList.Count <= secondColumnCount)
    {
      anchoredPos.y -= ICON_GAP_Y * (constructedBuildingsList.Count);
    }

    // Second column
    else
    {
      anchoredPos.x += ICON_GAP_X;

      anchoredPos.y -= ICON_GAP_Y * (constructedBuildingsList.Count - secondColumnCount - 1);
    }

    newButton.GetComponent<RectTransform>().anchoredPosition = anchoredPos;

    constructedBuildingsList.Add(newButton.GetComponent<Button>());
  }
}
