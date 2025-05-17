using UnityEngine;
using System.Collections;
using TMPro;

public class NewsScrollAnimation : MonoBehaviour
{
    [SerializeField] 
    public TextMeshProUGUI newsText;// ���� �ؽ�Ʈ 
    public RectTransform textTransform; // ��ũ���� �ؽ�Ʈ ����
    public float scrollSpeed = 200f;     // ���ǵ�
    private float startX;
    private float endX;

    private RectTransform panelRect;
    private Coroutine scrollCoroutine;
    void Start()
    {

        panelRect = GetComponent<RectTransform>();


    }


    public IEnumerator ScrollCoroutine(string content)
    {
        newsText.text = content;
        float panelWidth = panelRect.rect.width;
        float textWidth = textTransform.rect.width;

        float startX = panelWidth;
        float endX = -textWidth;

        Vector2 pos = textTransform.anchoredPosition;
        pos.x = startX;
        textTransform.anchoredPosition = pos;

        while (pos.x > endX)
        {
            pos.x -= scrollSpeed * Time.deltaTime;
            textTransform.anchoredPosition = pos;
            yield return null;// wait for sec?
        }
    }
}
