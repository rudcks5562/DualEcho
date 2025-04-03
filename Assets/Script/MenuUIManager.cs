using UnityEngine;


public class MenuUIManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject saveLoadPanel;
    public GameObject optionsPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShowMainMenu(); // 시작 시 메인 메뉴 활성화
    }


    void Update()
    {

    }
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        saveLoadPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    public void ShowSaveLoad()
    {
        mainMenuPanel.SetActive(false);
        saveLoadPanel.SetActive(true);
    }

    public void ShowOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit(); // 게임 종료
    }
}

