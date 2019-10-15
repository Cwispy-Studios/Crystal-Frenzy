﻿using UnityEngine;

public class CrystalNode : MonoBehaviour
{
  [SerializeField]
  private GameObject[] connectedNodes = null;
  [SerializeField]
  private GameObject[] pathSplines = null;

  public void CheckCrystalIsValid(GameObject checkObject, ref GameObject setTarget, ref GameObject crystalPath)
  {
    for (int i = 0; i < connectedNodes.Length; ++i)
    {
      if (connectedNodes[i] == checkObject && checkObject.GetComponent<Faction>().faction != GetComponent<Faction>().faction)
      {
        setTarget = checkObject;
        crystalPath = pathSplines[i];

        break;
      }
    }
  }
}
