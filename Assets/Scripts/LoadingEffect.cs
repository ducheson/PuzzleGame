using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class LoadingEffect : MonoBehaviour
{
    public static LoadingEffect Instance;

    public RectTransform top;
    public RectTransform bottom;

    private Vector2 topOriginalPos;
    private Vector2 bottomOriginalPos;

    public float animationDuration = 0.5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make persistent across scenes
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (top != null) top.gameObject.SetActive(true);
        if (bottom != null) bottom.gameObject.SetActive(true);

        topOriginalPos = top.anchoredPosition;
        bottomOriginalPos = bottom.anchoredPosition;

        top.anchoredPosition = topOriginalPos + new Vector2(0, 0);
        bottom.anchoredPosition = bottomOriginalPos - new Vector2(0, 0);
    }

    public void LoadSceneWithTransition(string sceneName)
    {
        // Animate into center
        top.DOAnchorPos(Vector2.zero, animationDuration).SetEase(Ease.OutCubic);
        bottom.DOAnchorPos(Vector2.zero, animationDuration).SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                // Load scene after both panels finish moving in
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadScene(sceneName);
            });
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Animate out after scene loads
        top.DOAnchorPos(topOriginalPos, animationDuration).SetEase(Ease.InCubic);
        bottom.DOAnchorPos(bottomOriginalPos, animationDuration).SetEase(Ease.InCubic);
    }
}
