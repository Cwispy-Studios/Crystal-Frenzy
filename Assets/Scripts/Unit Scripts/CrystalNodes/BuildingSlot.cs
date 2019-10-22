using UnityEngine;

public class BuildingSlot : MonoBehaviour
{
  private bool active = false;
  private bool constructed = false;

  [SerializeField]
  private GameObject buildingSlot = null;

  private void Update()
  {
    if (constructed == false)
    {
      buildingSlot.GetComponent<Selectable>().enabled = active;
    }
  }
}
