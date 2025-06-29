using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableObjectScript", menuName = "Scriptable Objects/NewScriptableObjectScript")]
public class MapZoneContent : ScriptableObject
{
    public string ZoneName;
    public string MainFacility;
    public int StablePoint;
    public string Information;
    public string Category;
    public List<KeyValuePair<string, string>> Reports;


    
}
