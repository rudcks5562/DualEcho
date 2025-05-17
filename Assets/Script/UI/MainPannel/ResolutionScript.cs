using UnityEngine;

public class ResolutionScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector2 referenceResolution = new Vector2(1920, 1080); // ���� �ػ�
        float screenRatio = Mathf.Min(Screen.width / referenceResolution.x, Screen.height / referenceResolution.y);

        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 100) * screenRatio; // ���� ũ�� * ����

    }// ������ UI�� ���Ͽ� ������ ��!

    // Update is called once per frame
    void Update()
    {
        
    }
}
