﻿using UnityEngine;

public class FoodProvider : MonoBehaviour
{
  public int foodProvided = 0;

  private void OnEnable()
  {
    GameManager.resourceManager.FarmClaimed(this);
  }

  // When upgrading, old farm gets destroyed and this gets called
  private void OnDestroyed()
  {
    GameManager.resourceManager.FarmLost(this);
  }
}