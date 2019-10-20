using UnityEngine;

public class FieldOfVision : MonoBehaviour
{
  public bool fovActive = true;

  [SerializeField]
  private GameObject fovMeshPrefab = null;

  [SerializeField]
  private float fovRange = 15f;

  private GameObject fovMesh = null;

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
    if (GetComponent<Faction>().faction != Faction.FACTIONS.GOBLINS)
    {
      fovActive = false;
    }

    fovMesh.SetActive(fovActive);
  }
}
