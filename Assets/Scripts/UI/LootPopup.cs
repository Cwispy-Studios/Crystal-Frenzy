using UnityEngine;
using UnityEngine.UI;

public class LootPopup : MonoBehaviour
{
  [SerializeField]
  private Text goldText = null, crystalText = null;

  public void SetText(int gold, int crystal)
  {
    goldText.text = gold.ToString();
    crystalText.text = "+" + crystal.ToString();
  }
}
