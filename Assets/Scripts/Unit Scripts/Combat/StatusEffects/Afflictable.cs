using UnityEngine;

public class Afflictable : MonoBehaviour
{
  [System.NonSerialized]
  public Poison posionAffliction;
  [System.NonSerialized]
  public Slow slowAffliction;
  [System.NonSerialized]
  public Curse curseAffliction;

  private void Awake()
  {
    posionAffliction = new Poison(gameObject);
    slowAffliction = new Slow(gameObject);
    curseAffliction = new Curse(gameObject);
  }

  private void Update()
  {
    if (posionAffliction.Active)
    {
      posionAffliction.Update();
    }

    if (slowAffliction.Active)
    {
      slowAffliction.Update();
    }

    if (curseAffliction.Active)
    {
      curseAffliction.Update();
    }
  }
}
