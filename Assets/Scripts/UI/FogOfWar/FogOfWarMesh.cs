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

  private void OnEnable()
  {
    HideableManager.fogOfWarMeshList.Add(this);
  }

  private void OnDisable()
  {
    HideableManager.fogOfWarMeshList.Remove(this);
  }
}
