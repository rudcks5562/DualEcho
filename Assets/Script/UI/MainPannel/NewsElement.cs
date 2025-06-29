using UnityEngine;
using TMPro;

public class NewsElement : MonoBehaviour
{
    public TextMeshProUGUI dataText;
    public TextMeshProUGUI timeText;

    public void SetData(string context,string logTime)
    {
        dataText.text = context;
        timeText.text = logTime;
    }
}
