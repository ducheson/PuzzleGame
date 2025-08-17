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
        Time.timeScale = 0f;

        pausePanel.SetActive(true);

        // Rotate button ignoring timescale
        pauseButton.DORotate(new Vector3(0f, 0f, 360f), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);

        // Animate text, menu, and fade canvas ignoring timescale
        pauseText.DOAnchorPos(originalTextPos, duration).SetEase(Ease.OutBack).SetUpdate(true);
        pauseMenu.DOAnchorPos(originalMenuPos, duration).SetEase(Ease.OutCubic).SetUpdate(true);
        canvasGroup.DOFade(1f, duration).SetEase(Ease.OutSine).SetUpdate(true).OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            isAnimating = false;
        });
    }

    public void HidePauseMenu()
    {
        isAnimating = true;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Rotate button ignoring timescale
        pauseButton.DORotate(new Vector3(0f, 0f, -360f), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);

        // Animate text, menu, and fade canvas ignoring timescale
        pauseText.DOAnchorPos(hiddenPos, duration).SetEase(Ease.InBack).SetUpdate(true);
        pauseMenu.DOAnchorPos(hiddenPos, duration).SetEase(Ease.InCubic).SetUpdate(true);
        canvasGroup.DOFade(0f, duration).SetEase(Ease.InSine).SetUpdate(true).OnComplete(() =>
        {
            pausePanel.SetActive(false);
            isAnimating = false;

            // Resume game after animation completes
            Time.timeScale = 1f;
        });
    }

    // ⏩ Instantly show menu (no animation)
    public void ShowImmediateMenu()
    {
        // Kill all running tweens just in case
        DOTween.Kill(pauseText);
        DOTween.Kill(pauseMenu);
        DOTween.Kill(canvasGroup);
        DOTween.Kill(pauseButton);

        Time.timeScale = 0f;

        pausePanel.SetActive(true);
        pauseText.anchoredPosition = originalTextPos;
        pauseMenu.anchoredPosition = originalMenuPos;

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        isAnimating = false;
    }

    // ⏩ Instantly hide menu (no animation)
    public void HideImmediateMenu()
    {
        DOTween.Kill(pauseText);
        DOTween.Kill(pauseMenu);
        DOTween.Kill(canvasGroup);
        DOTween.Kill(pauseButton);

        pausePanel.SetActive(false);
        pauseText.anchoredPosition = hiddenPos;
        pauseMenu.anchoredPosition = hiddenPos;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        Time.timeScale = 1f;
        isAnimating = false;
    }

    public bool IsAnimating()
    {
        return isAnimating;
    }
}
