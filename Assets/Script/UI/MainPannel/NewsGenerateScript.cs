using System;
using System.Collections.Generic;
using UnityEngine;

public class NewsGenerateScript : MonoBehaviour
{
    [SerializeField] private NewsData data;

    public List<KeyValuePair<string, string>> newsList;//record
    public string currentNews;


    void Start()
    {


        newsList= new List<KeyValuePair <string,string>>();// 뉴스, 시간


    }

    public string GenerateNews()
    {
        string news = $"{GetRandom(data.subjects)}이(가) {GetRandom(data.details)} {GetRandom(data.actions)}.";

        newsList.Add( new KeyValuePair<string,string> (news,"10:02 TEST-TIME"));
        return news;
    }

    public string GetRandom(List<string> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public List<KeyValuePair<string, string>> GetAllNewsList()
    {

        return newsList;
    }


}
