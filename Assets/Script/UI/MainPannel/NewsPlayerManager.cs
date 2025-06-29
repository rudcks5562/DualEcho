using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Collections;

public class NewsPlayerManager : MonoBehaviour
{
    public TextMeshProUGUI newsText;
    [Header("뉴스 파편 데이터 (ScriptableObject)")]
    public NewsData newsFragmentsData;

    [Header("UI 애니메이터 연결 (뉴스 표시용)")]
    public NewsScrollAnimation newsScrollAnimation;

    public NewsGenerateScript generator;
    public List<KeyValuePair<string,string>> newsHistory = new List <KeyValuePair<string, string>>();
    public Queue<KeyValuePair<string, string>> newsQueue = new Queue<KeyValuePair<string, string>>();
    public bool isPlayingNews = false;
    public GameObject newsCardPrefab; // 프리팹 연결
    public Transform contentParent;   // ScrollView 안의 Content

    public void AddNews( string body, string time)
    {
        GameObject card = Instantiate(newsCardPrefab, contentParent);
        NewsElement ui = card.GetComponent<NewsElement>();
        ui.SetData( body, time);
    }
    public void Start()
    {
        GenerateAndShowNews();
        GenerateAndShowNews();
    }

    public void GenerateAndShowNews()
    {
        KeyValuePair<string,string> entry = new KeyValuePair<string, string>(generator.GenerateNews(),"2025.05.12");
        newsHistory.Add(entry);
        //entry.Key+entry.Value
        newsQueue.Enqueue(entry);
        AddNews(entry.Key, entry.Value);

        if (!isPlayingNews)
            StartCoroutine(PlayNewsFromQueue());
    }
    IEnumerator PlayNewsFromQueue()
    {
        isPlayingNews = true;

        while (newsQueue.Count > 0)
        {
            var current = newsQueue.Dequeue();
           // newsText.text = current.Key + " " + current.Value;

            yield return StartCoroutine(newsScrollAnimation.ScrollCoroutine(current.Key + " " + current.Value)); // 스크롤 끝날 때까지 대기 + 이곳에서 시간,내용 출력순서 변경가능
        }

        isPlayingNews = false;
    }

    // 이전 뉴스 기록 반환
    public List<KeyValuePair<string, string>> GetNewsHistory()
    {
        return new List<KeyValuePair<string, string>>(newsHistory);
    }

    // 뉴스 로그 초기화
    public void ClearNews()
    {
        newsHistory.Clear();
    }
}
