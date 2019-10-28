using UnityEngine;

public class Afflictable : MonoBehaviour
{
  public Poison posionAffliction;

  private void Awake()
  {
    posionAffliction = new Poison();
  }

  private void Update()
  {
    if (posionAffliction.active)
    {
      posionAffliction.Update();
    }      
  }
}
