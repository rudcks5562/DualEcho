using System.Collections.Generic;
using UnityEngine;

public enum NodeState
{
    Locked,
    Available,
    Activated
}
[System.Serializable]
public class NodeData
{
    public string name;
    public int cost;
    public NodeState state;
    public string iconId;
    public List<string> connections;
    public List<string> prerequisites;
    public string description;
}

[System.Serializable]
public class WrapperNodeData
{
    public Dictionary<string, NodeData> nodes;
}
