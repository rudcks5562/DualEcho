using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Diagnostics;
using System;

public class TextHoverScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TextMeshProUGUI text;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;
    public MenuUIManager uiManager;
    public MenuAction action;
    public float hoverScale = 1.2f;
    private Vector3 originalScale;
    public float duration = 0.2f;


    void Start()
    {
        text.color = normalColor;
        originalScale = text.transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = hoverColor;
        StartCoroutine(SmoothTransition(hoverColor, originalScale * hoverScale));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = normalColor;
        StartCoroutine(SmoothTransition(normalColor, originalScale));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StopAllCoroutines();
        if (uiManager != null)
        {
            switch (action)
            {
                case MenuAction.StartGame:
                    uiManager.ShowMainMenu();
                    break;
                case MenuAction.LoadGame:
                    uiManager.ShowSaveLoad();
                    break;
                case MenuAction.OpenOptions:
                    uiManager.ShowOptions();
                    break;
                case MenuAction.ExitGame:
                    uiManager.ExitGame();
                    break;
            }
        }
    }
    private System.Collections.IEnumerator SmoothTransition(Color targetColor, Vector3 targetScale)
    {
        float elapsed = 0f;
        Color startColor = text.color;
        Vector3 startScale = text.transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            text.color = Color.Lerp(startColor, targetColor, elapsed / duration);
            text.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
            yield return null;
        }

        text.color = targetColor;
        text.transform.localScale = targetScale;
    }


}

