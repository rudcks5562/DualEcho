[System.Serializable]
public class MapZonePointRef
{
    public bool isShared; // true: sharedPoints, false: localPoints
    public int index;     // sharedPointIndices or localPoints 중 어떤 건지
}