using UnityEngine;

public class FieldOfVision : MonoBehaviour
{
  public bool fovActive = true;

  [SerializeField]
  private GameObject fovMeshPrefab;

  [SerializeField]
  private float fovRange;

  private GameObject fovMesh;

  private void Awake()
  {
    fovMesh = Instantiate(fovMeshPrefab);

    // Set the global scale before parenting it
    Vector3 fovMeshScale = new Vector3(fovRange, 0.1f, fovRange);
    fovMesh.transform.localScale = fovMeshScale;

    Vector3 fovMeshPos = new Vector3(transform.position.x, 0, transform.position.z);
    fovMesh.transform.position = fovMeshPos;
    fovMesh.transform.parent = transform;
  }

  private void Update()
  {
    fovMesh.SetActive(fovActive);
  }
}
