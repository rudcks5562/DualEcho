using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Diagnostics;


public class ToggleExpandedPannel : MonoBehaviour
{
    [Header("UI 참조")]
    public RectTransform buttonGroupPanel; // 버튼 패널
    public RectTransform arrowImage;       // 화살표 이미지
    public RectTransform rootPanel;
    public RectTransform toggleButton;

    [Header("세팅")]
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
       // 초기화 용도의 패널조정 필요.

    }

    public void OnToggleClick()
    {
        isExpanded = !isExpanded;

        float targetHeight = isExpanded ? expandedHeight : collapsedHeight;
        float heightDelta = targetHeight - buttonGroupPanel.sizeDelta.y;

        // 화살표 회전 애니메이션
        float targetRotation = isExpanded ? 0f : 180f;// 180도 회전.
        arrowImage.DOLocalRotate(new Vector3(0, 0, targetRotation), animationDuration)
                  .SetEase(Ease.OutCubic); // 이징 적용

        // 버튼 패널 보이기 / 숨기기
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
