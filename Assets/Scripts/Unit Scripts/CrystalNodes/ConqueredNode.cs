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
  private GameObject cameraBound;
  public GameObject CameraBound
  {
    get { return cameraBound; }
    set { if (cameraBound == null) cameraBound = value; }
  }

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
