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

public class HealthBarController : MonoBehaviour
{
  [SerializeField]
  private HealthBar healthBarPrefab = null;

  private Dictionary<Health, HealthBar> healthBars = new Dictionary<Health, HealthBar>();

  private void Awake()
  {
    Health.OnHealthAdded += AddHealthBar;
    Health.OnHealthRemoved += RemoveHealthBar;
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
    if (healthBars.ContainsKey(health))
    {
      Destroy(healthBars[health].gameObject);
      healthBars.Remove(health);
    }
  }
}
