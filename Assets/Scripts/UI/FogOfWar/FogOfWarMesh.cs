using System.Collections.Generic;
using UnityEngine;

public enum FOV_SHAPE
{
  CIRCLE = 0,
  RECT,
}

public class FogOfWarMesh : MonoBehaviour
{
  [SerializeField]
  private FOV_SHAPE fovShape = FOV_SHAPE.CIRCLE;
  public FOV_SHAPE FovShape
  {
    get
    {
      return fovShape;
    }
  }

  private HideableManager hideableManager = null;

  private void Awake()
  {
    hideableManager = FindObjectOfType<HideableManager>();
  }

  private void OnEnable()
  {
    hideableManager.fogOfWarMeshList.Add(this);
  }

  private void OnDisable()
  {
    hideableManager.fogOfWarMeshList.Remove(this);
  }
}
