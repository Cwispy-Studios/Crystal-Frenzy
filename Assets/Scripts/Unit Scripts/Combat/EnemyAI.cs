using UnityEngine;

public class EnemyAI : MonoBehaviour
{
  private const float UPDATE_INTERVAL = 5f;
  private float updateCountdown = 0;

  public GameObject target;

  private void Awake()
  {
    updateCountdown = UPDATE_INTERVAL;
  }

  private void Start()
  {
    GetComponent<Attack>().SetAttackMovePosition(target.transform.position);
    updateCountdown = UPDATE_INTERVAL;

    // Crystal node, so does not need to constantly update
    if (target.GetComponent<CrystalNode>())
    {
      target = null;
    }
  }

  private void Update()
  {
    if (target != null)
    {
      if (updateCountdown <= 0)
      {
        //GetComponent<UnitOrder>().IssueOrderPoint(target.transform.position);
        GetComponent<Attack>().SetAttackMovePosition(target.transform.position);
        updateCountdown = UPDATE_INTERVAL;

        // Crystal node, so does not need to constantly update
        if (target.GetComponent<CrystalNode>())
        {
          target = null;
        }
      }
      
      else
      {
        updateCountdown -= Time.deltaTime;
      }
    }
  }
}
