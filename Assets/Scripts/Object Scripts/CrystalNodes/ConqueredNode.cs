using UnityEngine;

public class ConqueredNode : MonoBehaviour
{
  public bool conquered = false;

  [SerializeField]
  private GameObject assemblySpace;
  public GameObject AssemblySpace
  {
    get { return assemblySpace; }
    set { if (assemblySpace == null) assemblySpace = value; }
  }

  [SerializeField]
  private GameObject[] tempFOVMeshes;

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
