/******************************************************************************/
/*!
\file   HealtBarController.cs
\author Unity3d College, Wong Zhihao
\par    email: wongzhihao.student.utwente.nl
\date   15 October 2019
\brief

  Brief:
    Implementation from: https://www.youtube.com/watch?v=kQqqo_9FfsU&t=396s
*/
/******************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
  public static Color normalColour = new Color(255f / 255f, 0 / 255f, 0 / 255f);
  public static Color poisonColour = new Color(140f / 255f, 255f / 255f, 140f / 255f);
  public static Color slowColour = new Color(255f / 255f, 205f / 255f, 130f / 255f);
  public static Color curseColour = new Color(255f / 255f, 150f / 255f, 255f / 255f);
  public static Color blankColour = new Color(0, 0, 0, 1f);

  [SerializeField]
  private HealthBar healthBarPrefab = null;

  private Dictionary<Health, HealthBar> healthBars = new Dictionary<Health, HealthBar>();

  private void Awake()
  {
    Health.OnHealthAdded += AddHealthBar;
    Health.OnHealthRemoved += RemoveHealthBar;
  }

  private void Update()
  {
    foreach (var item in healthBars)
    {
      Afflictable afflictable = item.Key.GetComponent<Afflictable>();

      item.Value.GetComponentInChildren<Image>().color = blankColour;

      if (afflictable)
      {
        int numAfflictions = 0;

        if (afflictable.posionAffliction.Active)
        {
          item.Value.GetComponentInChildren<Image>().color += poisonColour;
          ++numAfflictions;
        }

        if (afflictable.slowAffliction.Active)
        {
          item.Value.GetComponentInChildren<Image>().color += slowColour;
          ++numAfflictions;
        }

        if (afflictable.curseAffliction.Active)
        {
          item.Value.GetComponentInChildren<Image>().color += curseColour;
          ++numAfflictions;
        }

        if (numAfflictions == 0)
        {
          item.Value.GetComponentInChildren<Image>().color = normalColour;
        }

        else
        {
          item.Value.GetComponentInChildren<Image>().color /= numAfflictions;
        }
      }
    }
  }

  private void AddHealthBar(Health health)
  {
    if (healthBars.ContainsKey(health) == false)
    {
      var healthBar = Instantiate(healthBarPrefab, transform);
      healthBar.gameObject.SetActive(true);
      healthBars.Add(health, healthBar);
      healthBar.SetHealth(health);
    }
  }

  private void RemoveHealthBar(Health health)
  {
    if (this != null && healthBars.ContainsKey(health))
    {
      Destroy(healthBars[health].gameObject);
      healthBars.Remove(health);
    }
  }

  private void OnDestroy()
  {
    Health.OnHealthAdded -= AddHealthBar;
    Health.OnHealthRemoved -= RemoveHealthBar;

    foreach (var health in healthBars)
    {
      Destroy(health.Value);
    }

    healthBars.Clear();
  }
}
