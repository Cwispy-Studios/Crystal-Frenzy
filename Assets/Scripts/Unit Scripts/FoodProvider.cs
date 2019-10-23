using UnityEngine;

public class FoodProvider : MonoBehaviour
{
  public int foodProvided = 0;

  private void OnEnable()
  {
    GameManager.resourceManager.FarmClaimed(this);
  }

  private void OnDestroyed()
  {
    GameManager.resourceManager.FarmLost(this);
  }
}