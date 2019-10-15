using UnityEngine;

public class CameraBuildingConstruction : MonoBehaviour
{
  public const KeyCode CONSTRUCT_KEY = KeyCode.B;

  // Grid prefab for the construction mode
  [SerializeField]
  private GameObject gridPrefab = null;
  // Materials for the grid
  [SerializeField]
  private Material availableGridMaterial = null, unavailableGridMaterial = null;
  // Store all the building prefabs that can be constructed from both sides
  [SerializeField]
  private GameObject testSpawnerPrefab = null, testSpawner2Prefab = null;

  // The building selected to be constructed
  private GameObject buildingToConstruct = null;

  // Pools the grids
  private GameObject [] grids;
  // Up to 10x10 building
  private const int MAX_GRIDS = 100;
  // Number of active grids shown on screen, based on the size of the building selected
  private int activeGrids = 0;

  // Where to construct the building
  private Vector3 constructPos;

  // Various modes
  private bool buildingSelectMode = false;
  private bool constructMode = false;
  private bool constructValid = false;

  private void Awake()
  {
    constructPos = new Vector3();
    grids = new GameObject[MAX_GRIDS]; 
    
    for(int i = 0; i < MAX_GRIDS; ++i)
    {
      grids[i] = Instantiate(gridPrefab);
      grids[i].SetActive(false);
    }
  }

  private void Update()
  {
    // Enter or exit building select mode and construct mode
    if (Input.GetKeyDown(CONSTRUCT_KEY))
    {
      if (!buildingSelectMode && !constructMode)
      {
        buildingSelectMode = true;
      }

      else if (buildingSelectMode)
      {
        buildingSelectMode = false;
      }

      else if (constructMode)
      {
        constructMode = false;
      }
    }

    if (buildingSelectMode)
    {
      BuildingSelectMode();
    }

    else if (constructMode)
    {
      ConstructMode();
    }

    // Not in any mode, no grids should be shown, deactivate all the gridss
    else
    {
      SetActiveGrids(0, 0);
    }
  }

  private void BuildingSelectMode()
  {
    // Key 0 is pressed, this is debug key which spawns test building
    if (Input.GetKeyDown(KeyCode.Alpha0))
    {
      buildingSelectMode = false;
      constructMode = true;

      switch (GetComponent<Faction>().faction)
      {
        case Faction.FACTIONS.GOBLINS:
          buildingToConstruct = testSpawnerPrefab;
          break;

        case Faction.FACTIONS.FOREST:
          buildingToConstruct = testSpawner2Prefab;
          break;

        default:
          Debug.LogError("Camera in building select mode does not belong to any player! Faction is " + GetComponent<Faction>().faction);
          break;
      }
    }

    // Repeat as more buildings added
  }

  private void ConstructMode()
  {
    Vector3 mousePos = GetWorldMousePosOnGrid();

    //Debug.Log("Floored mouse pos: " + mousePos);

    // Retrieve Renderer of the building prefab we are going to construct so we can check the size
    Renderer buildingRenderer = buildingToConstruct.GetComponent<Renderer>();

    // Get the size of the object so we know how many grids to display to represent it on the map
    int objectWidth = Mathf.RoundToInt(buildingRenderer.bounds.size.x);
    int objectLength = Mathf.RoundToInt(buildingRenderer.bounds.size.z);

    SetActiveGrids(objectWidth, objectLength);

    PositionGrids(mousePos, objectWidth, objectLength);

    // Instantiate a building when the player clicks the LMB
    if (Input.GetMouseButtonDown(0) && constructValid)
    {
      // Set in the middle of the center grid
      constructPos.Set(mousePos.x + 0.5f, buildingToConstruct.transform.lossyScale.y / 2, mousePos.z + 0.5f);

      // Instantiate the building in the scene
      Instantiate(buildingToConstruct, constructPos, new Quaternion());

      if (!Input.GetKey(KeyCode.LeftShift))
        constructMode = false;
    }
  }

  private Vector3 GetWorldMousePosOnGrid()
  {
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    // We only want to collide with layer 8 (terrain)
    int layerMask = 1 << 8;
    // Invert bitmask to only have 8
    layerMask = ~layerMask;

    if (Physics.Raycast(ray, out RaycastHit hitInfo))
    {
      //Debug.Log(hitInfo.collider.name);
      //Debug.DrawRay(hitInfo.point, Vector3.up);
      //Debug.Log("Actual mouse pos: " + hitInfo.point);
      // Return the mouse position according to which grid it is on
      return new Vector3(
        Mathf.Floor(hitInfo.point.x),
        Mathf.Floor(hitInfo.point.y),
        Mathf.Floor(hitInfo.point.z));
    }

    // TODO: ADD A SAFETY NET HERE
    else return new Vector3();
  }

  private void SetActiveGrids(int objectWidth, int objectLength)
  {
    int numGrids = objectWidth * objectLength;

    if (numGrids > MAX_GRIDS)
    {
      Debug.LogError("ERROR! Attempting to project object of grid size " + numGrids);
    }

    // If number of grids to activate is more than the current number of active grids, only activate what is needed
    if (numGrids > activeGrids)
    {
      for (int i = activeGrids; i < numGrids; ++i)
      {
        grids[i].SetActive(true);
      }

      activeGrids = numGrids;
    }

    // Deactive the excess number of grids
    else if (numGrids < activeGrids)
    {
      for (int i = activeGrids - 1; i >= numGrids; --i)
      {
        grids[i].SetActive(false);
      }

      activeGrids = numGrids;
    }
  }

  private void PositionGrids(Vector3 clickGrid, int objectWidth, int objectLength)
  {
    // The mouse position is the center position of the square, so we have to divide the width and length by 2 rounded down
    // to get the outer coordinates of the entire grid. For this, we take the bottom right grid as the origin grid
    // Top right means -x and -y from the center grid to get to the bottom right
    float originX = clickGrid.x - Mathf.Floor(objectWidth / 2);
    float originZ = clickGrid.z - Mathf.Floor(objectWidth / 2);

    constructValid = true;

    // Now, we set the positions of the grid in intervals of 1 unit
    for (int x = 0; x < objectWidth; ++x)
    {
      for (int z = 0; z < objectLength; ++z)
      {
        int index = (x * objectWidth) + z;

        Vector3 newPos = new Vector3(originX + x, 0, originZ + z);

        grids[index].transform.position = newPos;

        // Set the colour of the grid based on whether or not the space is already occupied by another object
        if (TerrainGrid.CheckGridOccupied(grids[index].transform.GetChild(0).GetComponent<Collider>()))
        {
          grids[index].transform.GetChild(0).GetComponent<Renderer>().material = unavailableGridMaterial;

          constructValid = false;
        }

        else
        {
          grids[index].transform.GetChild(0).GetComponent<Renderer>().material = availableGridMaterial;
        }
      }
    }
  }
}
