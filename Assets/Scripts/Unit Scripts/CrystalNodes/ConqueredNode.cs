using UnityEngine;

public class ConqueredNode : MonoBehaviour
{
  [SerializeField]
  private GameObject assemblySpace;
  public GameObject AssemblySpace
  {
    get { return assemblySpace; }
    set { if (assemblySpace == null) assemblySpace = value; }
  }

  [SerializeField]
  private GameObject[] tempFOVMeshes = null;
  [SerializeField]
  private GameObject selectionCameraBound;
  public GameObject SelectionCameraBound
  {
    get { return selectionCameraBound; }
    set { if (selectionCameraBound == null) selectionCameraBound = value; }
  }

  public bool conquered = false;

  public void EnablePreparationFOV()
  {
    for (int i = 0; i < tempFOVMeshes.Length; ++i)
    {
      tempFOVMeshes[i].SetActive(true);
    }
  }

  public void DisablePreparationFOV()
  {
    for (int i = 0; i < tempFOVMeshes.Length; ++i)
    {
      tempFOVMeshes[i].SetActive(false);
    }
  }
}
