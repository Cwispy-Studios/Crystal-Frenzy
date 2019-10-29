using UnityEngine;
using UnityEngine.UI;

public class NodeUIPanel : MonoBehaviour
{
  private const float WORLD_SCALE = 1000f;
  private float panelScale;

  [SerializeField]
  private UINode nodeButtonPrefab = null;

  private UINode[] uiNodes;

  private void Awake()
  {
    panelScale = GetComponent<RectTransform>().sizeDelta.x;

    CrystalNode[] crystalNodes = FindObjectsOfType<CrystalNode>();

    uiNodes = new UINode[crystalNodes.Length];

    for (int i = 0; i < crystalNodes.Length; ++i)
    {
      Vector3 panelPos = new Vector3
      {
        x = (crystalNodes[i].transform.position.x / WORLD_SCALE) * panelScale,
        y = (crystalNodes[i].transform.position.z / WORLD_SCALE) * panelScale,
        z = 0
      };

      uiNodes[i] = Instantiate(nodeButtonPrefab, transform, false);
      uiNodes[i].GetComponent<RectTransform>().anchoredPosition = panelPos;
      uiNodes[i].SetNode(crystalNodes[i].gameObject, WORLD_SCALE, panelScale);
    }
  }

  public void UpdateUINodesColours()
  {
    for (int i = 0; i < uiNodes.Length; ++i)
    {
      uiNodes[i].UpdateColour();
    }
  }
}
