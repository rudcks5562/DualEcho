using UnityEngine;

public class NodeUIManager : MonoBehaviour
{

        public NodeDataJsonParser loader;
        public NodeLayoutGenerator layoutManager;

        void Start()
        {
            loader = GetComponent<NodeDataJsonParser>();
            layoutManager = GetComponent<NodeLayoutGenerator>();

            // 예시: 시작 노드 4개로부터 각각 배치
            string[] startNodes = { "A", "B", "C", "D" };
            Vector2[] origins = {
            new Vector2(-300, 200),
            new Vector2(300, 200),
            new Vector2(-300, -200),
            new Vector2(300, -200)
        };

            for (int i = 0; i < startNodes.Length; i++)
            {
                if (loader.nodeDictionary.TryGetValue(startNodes[i], out var node))
                {
                    //layoutManager.CreateNodeUI(origins[i], node, loader.nodeDict);
                }
            }
        }
    
}
