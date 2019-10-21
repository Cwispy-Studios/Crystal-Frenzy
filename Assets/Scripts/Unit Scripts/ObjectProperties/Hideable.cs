using UnityEngine;

public class Hideable: MonoBehaviour
{
  private void OnEnable()
  {
    HideableManager.hideableUnits.Add(gameObject);
  }

  private void OnDisable()
  {
    HideableManager.hideableUnits.Remove(gameObject);
  }
}
