using UnityEngine;

public enum BUILDING_TYPE
{
  FARM = 0,
  ARCHERY_RANGE,
  BLACKSMITH,
  BRAWL_PIT,
  MAGE_TOWER
}

public class BuildingType : MonoBehaviour
{
  public BUILDING_TYPE buildingType;
}
