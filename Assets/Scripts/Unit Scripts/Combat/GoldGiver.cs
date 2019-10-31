using UnityEngine;

public class GoldGiver : MonoBehaviour
{
  [SerializeField]
  private int goldProvided = 0;

  private void OnDestroy()
  {
    GameManager.resourceManager.SpendGold(-goldProvided);
  }
}
