using UnityEngine;

public class Hideable: MonoBehaviour
{
  private HideableManager hideableManager;

  private void Awake()
  {
    hideableManager = FindObjectOfType<HideableManager>();
  }

  private void OnEnable()
  {
    hideableManager.hideableUnits.Add(gameObject);
  }

  private void OnDisable()
  {
    hideableManager.hideableUnits.Remove(gameObject);
  }
}
