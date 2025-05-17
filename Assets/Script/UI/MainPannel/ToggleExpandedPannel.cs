using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Diagnostics;


public class ToggleExpandedPannel : MonoBehaviour
{
    [Header("UI ����")]
    public RectTransform buttonGroupPanel; // ��ư �г�
    public RectTransform arrowImage;       // ȭ��ǥ �̹���
    public RectTransform rootPanel;
    public RectTransform toggleButton;

    [Header("����")]
    public float expandedHeight = 100f;
    public float collapsedHeight = 0f;
    public float animationDuration = 0.3f;

    private bool isExpanded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector2 size = buttonGroupPanel.sizeDelta;
        buttonGroupPanel.sizeDelta = new Vector2(size.x, collapsedHeight);
        buttonGroupPanel.gameObject.SetActive(false);
        Vector2 rootSize = rootPanel.sizeDelta;
        UnityEngine.Debug.Log(rootSize.y);
        rootPanel.sizeDelta = new Vector2(rootSize.x, rootSize.y - (expandedHeight - collapsedHeight));
       // �ʱ�ȭ �뵵�� �г����� �ʿ�.

    }

    public void OnToggleClick()
    {
        isExpanded = !isExpanded;

        float targetHeight = isExpanded ? expandedHeight : collapsedHeight;
        float heightDelta = targetHeight - buttonGroupPanel.sizeDelta.y;

        // ȭ��ǥ ȸ�� �ִϸ��̼�
        float targetRotation = isExpanded ? 0f : 180f;// 180�� ȸ��.
        arrowImage.DOLocalRotate(new Vector3(0, 0, targetRotation), animationDuration)
                  .SetEase(Ease.OutCubic); // ��¡ ����

        // ��ư �г� ���̱� / �����
        if (isExpanded)
        {
            buttonGroupPanel.gameObject.SetActive(true);

            buttonGroupPanel.DOSizeDelta(new Vector2(buttonGroupPanel.sizeDelta.x, expandedHeight), animationDuration)
                            .SetEase(Ease.OutCubic);
            rootPanel.DOSizeDelta(new Vector2(rootPanel.sizeDelta.x, rootPanel.sizeDelta.y + heightDelta), animationDuration).SetEase(Ease.OutCubic);


        }

        else
        {
            buttonGroupPanel.DOSizeDelta(new Vector2(buttonGroupPanel.sizeDelta.x, collapsedHeight), animationDuration)
                            .SetEase(Ease.InCubic)
                            .OnComplete(() => buttonGroupPanel.gameObject.SetActive(false));

            rootPanel.DOSizeDelta(new Vector2(rootPanel.sizeDelta.x, rootPanel.sizeDelta.y + heightDelta), animationDuration).SetEase(Ease.InCubic);

        }

    }



}
