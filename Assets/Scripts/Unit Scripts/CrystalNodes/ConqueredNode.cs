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
  private GameObject assemblyFOVMesh = null;
  [SerializeField]
  private GameObject selectionCameraBound;
  public GameObject SelectionCameraBound
  {
    get { return selectionCameraBound; }
    set { if (selectionCameraBound == null) selectionCameraBound = value; }
  }

  public bool conquered = false;

  public void SetAssemblyFOV(bool active)
  {
    assemblyFOVMesh.SetActive(active);
  }

  //public void EnablePreparationFOV()
  //{
  //  for (int i = 0; i < assemblyFOVMeshes.Length; ++i)
  //  {
  //    assemblyFOVMeshes[i].SetActive(true);
  //  }
  //}

  //public void DisablePreparationFOV()
  //{
  //  for (int i = 0; i < assemblyFOVMeshes.Length; ++i)
  //  {
  //    assemblyFOVMeshes[i].SetActive(false);
  //  }
  //}
}
