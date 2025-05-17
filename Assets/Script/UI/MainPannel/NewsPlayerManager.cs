using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Collections;

public class NewsPlayerManager : MonoBehaviour
{
    public TextMeshProUGUI newsText;
    [Header("���� ���� ������ (ScriptableObject)")]
    public NewsData newsFragmentsData;

    [Header("UI �ִϸ����� ���� (���� ǥ�ÿ�)")]
    public NewsScrollAnimation newsScrollAnimation;

    public NewsGenerateScript generator;
    public List<KeyValuePair<string,string>> newsHistory = new List <KeyValuePair<string, string>>();
    public Queue<KeyValuePair<string, string>> newsQueue = new Queue<KeyValuePair<string, string>>();
    public bool isPlayingNews = false;
    public GameObject newsCardPrefab; // ������ ����
    public Transform contentParent;   // ScrollView ���� Content

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

            yield return StartCoroutine(newsScrollAnimation.ScrollCoroutine(current.Key + " " + current.Value)); // ��ũ�� ���� ������ ��� + �̰����� �ð�,���� ��¼��� ���氡��
        }

        isPlayingNews = false;
    }

    // ���� ���� ��� ��ȯ
    public List<KeyValuePair<string, string>> GetNewsHistory()
    {
        return new List<KeyValuePair<string, string>>(newsHistory);
    }

    // ���� �α� �ʱ�ȭ
    public void ClearNews()
    {
        newsHistory.Clear();
    }
}
