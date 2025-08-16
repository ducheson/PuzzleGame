using UnityEngine;
using DG.Tweening;

public class ResultMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject resultUI;
    public GameObject resultPanel;
    public RectTransform resultText;
    public RectTransform resultMenu;

    [Header("Animation Settings")]
    public Vector2 hiddenPos = new Vector2(0f, 2000f);
    public float duration = 0.5f;

    private Vector2 originalTextPos;
    private Vector2 originalMenuPos;

    private CanvasGroup canvasGroup;
    private bool isAnimating = false;

    void Start()
    {
        resultUI.SetActive(true);
    }

    void Awake()
    {
        // Ensure CanvasGroup exists
        canvasGroup = resultPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = resultPanel.AddComponent<CanvasGroup>();

        // Cache original positions
        originalTextPos = resultText.anchoredPosition;
        originalMenuPos = resultMenu.anchoredPosition;

        // Initialize state
        resultPanel.SetActive(false);
        resultText.anchoredPosition = hiddenPos;
        resultMenu.anchoredPosition = hiddenPos;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowResultMenu()
    {
        isAnimating = true;
        Time.timeScale = 0f;

        resultPanel.SetActive(true);

        // Animate text, menu, and fade canvas ignoring timescale
        resultText.DOAnchorPos(originalTextPos, duration).SetEase(Ease.OutBack).SetUpdate(true);
        resultMenu.DOAnchorPos(originalMenuPos, duration).SetEase(Ease.OutCubic).SetUpdate(true);
        canvasGroup.DOFade(1f, duration).SetEase(Ease.OutSine).SetUpdate(true).OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            isAnimating = false;
        });
    }

    public void HideResultMenu()
    {
        isAnimating = true;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Animate text, menu, and fade canvas ignoring timescale
        resultText.DOAnchorPos(hiddenPos, duration).SetEase(Ease.InBack).SetUpdate(true);
        resultMenu.DOAnchorPos(hiddenPos, duration).SetEase(Ease.InCubic).SetUpdate(true);
        canvasGroup.DOFade(0f, duration).SetEase(Ease.InSine).SetUpdate(true).OnComplete(() =>
        {
            resultPanel.SetActive(false);
            isAnimating = false;

            // Resume game after animation completes
            Time.timeScale = 1f;
        });
    }

    public bool IsAnimating()
    {
        return isAnimating;
    }
}
