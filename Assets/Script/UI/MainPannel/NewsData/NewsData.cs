using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewsData", menuName = "NewsUI/FragmentsData")]
public class NewsData : ScriptableObject
{
    public List<string> subjects;
    public List<string> actions;
    public List<string> details;
}
