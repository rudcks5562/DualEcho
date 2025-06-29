using System.Collections.Generic;
using UnityEngine;

public class NodeLayoutGenerator : MonoBehaviour
{
    public GameObject nodePrefab; // À°°¢Çü ³ëµå ÇÁ¸®ÆÕ
    public float nodeSpacing = 150f;

    private Dictionary<string, GameObject> nodeUIs = new();
    /*
     
         public void CreateNodeUI(Vector2 center, NodeData root, Dictionary<string, NodeData> allNodes)
    {
        HashSet<string> visited = new();
        RecursivePlace(root, center, 0, visited, allNodes);
    }

    void RecursivePlace(NodeData node, Vector2 position, int depth, HashSet<string> visited, Dictionary<string, NodeData> allNodes)
    {
        if (visited.Contains(node.id))
            return;

        visited.Add(node.id);

        GameObject nodeUI = Instantiate(nodePrefab, transform);
        nodeUI.GetComponent<RectTransform>().anchoredPosition = position;
        nodeUIs[node.id] = nodeUI;

        int count = node.dependencies.Length;// conn
        float angleStep = 360f / Mathf.Max(count, 1);

        for (int i = 0; i < count; i++)
        {
            string childId = node.dependencies[i];
            if (!allNodes.ContainsKey(childId)) continue;

            float angle = i * angleStep;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector2 newPos = position + dir * nodeSpacing;

            RecursivePlace(allNodes[childId], newPos, depth + 1, visited, allNodes);
        }
    }

     
     */


}
