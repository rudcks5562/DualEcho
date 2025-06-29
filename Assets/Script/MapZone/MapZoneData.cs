using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "MapZone/Zone Data")]
public class MapZoneData : ScriptableObject
{
    public List<Vector3> sharedPoints = new(); // 모든 zone에서 공유되는 포인트들
    public List<MapZone> zones = new();        // 각각의 zone
}

[System.Serializable]
public class MapZone
{
    public string zoneName;
    public Color zoneColor;

    public List<int> sharedPointIndices = new(); // 공용 좌표 인덱스
    public List<Vector3> localPoints = new();    // 고유 좌표들
    public List<MapZonePointRef> allPoints = new();     // 전체 순서 기록 
}


