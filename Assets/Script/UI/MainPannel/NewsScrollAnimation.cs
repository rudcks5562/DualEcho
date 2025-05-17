using UnityEngine;
using System.Collections;
using TMPro;

public class NewsScrollAnimation : MonoBehaviour
{
    [SerializeField] 
    public TextMeshProUGUI newsText;// 실제 텍스트 
    public RectTransform textTransform; // 스크롤할 텍스트 영역
    public float scrollSpeed = 200f;     // 스피드
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
