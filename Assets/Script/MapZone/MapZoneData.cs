using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "MapZone/Zone Data")]
public class MapZoneData : ScriptableObject
{
    public List<Vector3> sharedPoints = new(); // ��� zone���� �����Ǵ� ����Ʈ��
    public List<MapZone> zones = new();        // ������ zone
}

[System.Serializable]
public class MapZone
{
    public string zoneName;
    public Color zoneColor;

    public List<int> sharedPointIndices = new(); // ���� ��ǥ �ε���
    public List<Vector3> localPoints = new();    // ���� ��ǥ��
    public List<MapZonePointRef> allPoints = new();     // ��ü ���� ��� 
}


