using UnityEngine;

public class Cheats : MonoBehaviour
{
  [SerializeField]
  private Projector fogOfWarBlackProjector = null, fogOfWarProjector = null;

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.G))
    {
      GameManager.resourceManager.SpendGold(-1000);
    }

    if (Input.GetKeyDown(KeyCode.C))
    {
      GameManager.resourceManager.CheatCrystals();
    }

    if (Input.GetKeyDown(KeyCode.F))
    {
      fogOfWarBlackProjector.enabled = !fogOfWarBlackProjector.enabled;
      fogOfWarProjector.enabled = !fogOfWarProjector.enabled;
    }

    if (Input.GetKeyDown(KeyCode.B))
    {
      GetComponent<CameraControls>().SetFreeView();
    }

    if (Input.GetKeyDown(KeyCode.D))
    {
      var crystalSeeker = FindObjectOfType<CrystalSeeker>();

      if (crystalSeeker)
      {
        crystalSeeker.GetComponent<Health>().ModifyHealth(-100f, Vector3.zero);
      }
    }

    if (Input.GetKeyDown(KeyCode.H))
    {
      var crystalSeeker = FindObjectOfType<CrystalSeeker>();

      if (crystalSeeker)
      {
        crystalSeeker.GetComponent<Health>().ModifyHealth(100f, Vector3.zero);
      }
    }

    if (Input.GetKeyDown(KeyCode.S))
    {
      var crystalSeeker = FindObjectOfType<CrystalSeeker>();

      if (crystalSeeker)
      {
        crystalSeeker.GetComponent<BezierSolution.BezierWalkerWithSpeed>().speed += 2f;
      }
    }

    if (Input.GetKeyDown(KeyCode.L))
    {
      var crystalSeeker = FindObjectOfType<CrystalSeeker>();

      if (crystalSeeker)
      {
        crystalSeeker.GetComponent<BezierSolution.BezierWalkerWithSpeed>().speed -= 2f;
      }
    }

    if (Input.GetKeyDown(KeyCode.R))
    {
      FindObjectOfType<GameManager>().CheatRound(1);
    }

    if (Input.GetKeyDown(KeyCode.E))
    {
      FindObjectOfType<GameManager>().CheatRound(-1);
    }
  }
}
