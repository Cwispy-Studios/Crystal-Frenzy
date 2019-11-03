using UnityEngine;

public class GoldGiver : MonoBehaviour
{
  [SerializeField]
  private int goldProvided = 0;

  private bool rewardsGold = true;

  public void DisableGoldReward()
  {
    rewardsGold = false;
  }

  private void OnDestroy()
  {
    if (rewardsGold)
    {
      GameManager.resourceManager.SpendGold(-goldProvided);
    } 
  }
}
