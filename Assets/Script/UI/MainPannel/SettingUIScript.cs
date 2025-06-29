using UnityEngine;
using UnityEngine.EventSystems;

public class SettingUIScript : MonoBehaviour
{
    public GameObject settingPannel;
    public GameObject soundPannel;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        settingPannel.SetActive(false);
        soundPannel.SetActive(false);
        

    }


    public void ClickCloseButton()
    {

        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
        if (clickedObject != null)
        {
            Transform parentPanel = clickedObject.transform.parent;
            parentPanel.gameObject.SetActive(false);
        }


    }
    public void ClickSettingButton()
    {
        settingPannel.SetActive(true);
    }
    public void ClickSoundSettingButton()
    {

        soundPannel.SetActive(true);
    }
    public void ClickRestartGame()
    {
        // 후일 작성
    }
    public void ClickSaveAndExit()
    {
        // 세이브 로드 구현 후 작성하기.
    }

    public void BGMToggle()
    {
        // 이벤트 매니저 만들고 나서 구현
    }
    public void SoundMuteToggle()
    {
        // 이벤트 매니저 만들고 나서 구현
    }


}
