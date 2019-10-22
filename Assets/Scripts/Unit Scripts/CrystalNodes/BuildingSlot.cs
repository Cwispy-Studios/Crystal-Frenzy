using UnityEngine;

public class BuildingSlot : MonoBehaviour
{
  [HideInInspector]
  public bool active = false;
  private bool constructed = false;

  [SerializeField]
  private GameObject buildingSlot = null;

  private void Update()
  {
    if (constructed == false)
    {
      buildingSlot.GetComponent<Selectable>().enabled = active;

      if (active == false)
      {
        CameraObjectSelection.SelectedUnitsList.Remove(buildingSlot);
        CameraObjectSelection.MouseHoverUnitsList.Remove(buildingSlot);
      }
    }
  }
}
