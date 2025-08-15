using UnityEngine;
using DG.Tweening;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseUI;
    public GameObject pausePanel;
    public RectTransform pauseText;
    public RectTransform pauseMenu;
    public RectTransform pauseButton;

    [Header("Animation Settings")]
    public Vector2 hiddenPos = new Vector2(0f, 2000f);
    public float duration = 0.5f;

    private Vector2 originalTextPos;
    private Vector2 originalMenuPos;

    private CanvasGroup canvasGroup;
    private bool isAnimating = false;

    void Start()
    {
        pauseUI.SetActive(true);
    }

    void Awake()
    {
        // Ensure CanvasGroup exists
        canvasGroup = pausePanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = pausePanel.AddComponent<CanvasGroup>();

        // Cache original positions
        originalTextPos = pauseText.anchoredPosition;
        originalMenuPos = pauseMenu.anchoredPosition;

        // Initialize state
        pausePanel.SetActive(false);
        pauseText.anchoredPosition = hiddenPos;
        pauseMenu.anchoredPosition = hiddenPos;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowPauseMenu()
    {
        isAnimating = true;

        pauseButton.DORotate(new Vector3(0f, 0f, 360f), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic);

        pausePanel.SetActive(true);

        pauseText.DOAnchorPos(originalTextPos, duration).SetEase(Ease.OutBack);
        pauseMenu.DOAnchorPos(originalMenuPos, duration).SetEase(Ease.OutCubic);
        canvasGroup.DOFade(1f, duration).SetEase(Ease.OutSine).OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            isAnimating = false;

            // Pause game AFTER animation
            Time.timeScale = 0f;
        });
    }

    public void HidePauseMenu()
    {
        Time.timeScale = 1f;
        isAnimating = true;

        pauseButton.DORotate(new Vector3(0f, 0f, -360f), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic);

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        pauseText.DOAnchorPos(hiddenPos, duration).SetEase(Ease.InBack);
        pauseMenu.DOAnchorPos(hiddenPos, duration).SetEase(Ease.InCubic);
        canvasGroup.DOFade(0f, duration).SetEase(Ease.InSine).OnComplete(() =>
        {
            pausePanel.SetActive(false);
            isAnimating = false;
        });
    }

    public bool IsAnimating()
    {
        return isAnimating;
    }
}
