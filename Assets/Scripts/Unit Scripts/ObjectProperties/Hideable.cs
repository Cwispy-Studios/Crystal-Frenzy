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
    gameObject.layer = 10;
    GetComponent<Selectable>().enabled = false;
    hideableManager.hideableUnits.Add(gameObject);
  }

  private void OnDisable()
  {
    hideableManager.hideableUnits.Remove(gameObject);
  }
}
