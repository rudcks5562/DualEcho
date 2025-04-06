using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "MapZone/Zone Data")]
public class MapZoneData : ScriptableObject
{
    public List<MapZone> zones = new();
}

[System.Serializable]
public class MapZone
{
    public string zoneName = "New Zone";
    public Color zoneColor = Color.white;
    public List<Vector3> controlPoints = new();
}

