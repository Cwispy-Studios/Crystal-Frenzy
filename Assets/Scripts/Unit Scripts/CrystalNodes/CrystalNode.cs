using UnityEngine;

public class CrystalNode : MonoBehaviour
{
  [SerializeField]
  private GameObject[] connectedNodes = null;
  [SerializeField]
  private GameObject[] pathSplines = null;
  [SerializeField]
  private GameObject[] spawnPoints = null;

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

  public void SetWaveSpawnersActive(bool active, GameObject target)
  {
    foreach (GameObject spawnPoint in spawnPoints)
    {
      spawnPoint.GetComponent<WaveSpawner>().enabled = active;
      spawnPoint.GetComponent<WaveSpawner>().target = target;
    }
  }
}
