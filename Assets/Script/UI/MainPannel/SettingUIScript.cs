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
        // ���� �ۼ�
    }
    public void ClickSaveAndExit()
    {
        // ���̺� �ε� ���� �� �ۼ��ϱ�.
    }

    public void BGMToggle()
    {
        // �̺�Ʈ �Ŵ��� ����� ���� ����
    }
    public void SoundMuteToggle()
    {
        // �̺�Ʈ �Ŵ��� ����� ���� ����
    }


}
