using UnityEngine;

public class ResolutionScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector2 referenceResolution = new Vector2(1920, 1080); // 기준 해상도
        float screenRatio = Mathf.Min(Screen.width / referenceResolution.x, Screen.height / referenceResolution.y);

        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 100) * screenRatio; // 기준 크기 * 비율

    }// 깨지는 UI에 한하여 적용할 것!

    // Update is called once per frame
    void Update()
    {
        
    }
}
