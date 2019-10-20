﻿using UnityEngine;

public class EnemyAI : MonoBehaviour
{
  private const float UPDATE_INTERVAL = 0.5f;
  private float updateCountdown = 0;

  public GameObject target;

  private void Update()
  {
    if (target != null && updateCountdown <= 0)
    {
      GetComponent<Order>().IssueOrderPoint(target.transform.position);
      GetComponent<Attack>().SetAttackMovePosition(target.transform.position);
      updateCountdown = UPDATE_INTERVAL;
    }
  }
}