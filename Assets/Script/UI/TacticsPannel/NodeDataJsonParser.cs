using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Diagnostics;


public class NodeDataJsonParser : MonoBehaviour
{

    public Dictionary<string, NodeData> nodeDictionary;
   
    void Start()
    {
        nodeDictionary = new Dictionary<string, NodeData>();


        TextAsset jsonFile = Resources.Load<TextAsset>("UI/NodeJson/NodeDefinition"); // Resources/Nodes/nodeData.json
        nodeDictionary = JsonConvert.DeserializeObject<Dictionary<string, NodeData>>(jsonFile.text);

        foreach (var node in nodeDictionary)
        {
            //nodeDict[node.id] = node;
            UnityEngine.Debug.Log($"ID: {node.Key}, Name: {node.Value.name}, Cost: {node.Value.cost}");
        }


    }

}
