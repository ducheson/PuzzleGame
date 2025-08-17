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

        top.anchoredPosition = topOriginalPos;
        bottom.anchoredPosition = bottomOriginalPos;
    }

    public void LoadInEffect(System.Action onComplete = null)
    {
        Sequence seq = DOTween.Sequence().SetUpdate(true); // ✅ ignore Time.timeScale

        seq.Append(top.DOAnchorPos(Vector2.zero, animationDuration)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true));

        seq.Join(bottom.DOAnchorPos(Vector2.zero, animationDuration)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true));

        seq.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void LoadOutEffect()
    {
        Sequence seq = DOTween.Sequence().SetUpdate(true); // ✅ ignore Time.timeScale

        seq.Append(top.DOAnchorPos(topOriginalPos, animationDuration)
            .SetEase(Ease.InCubic)
            .SetUpdate(true));

        seq.Join(bottom.DOAnchorPos(bottomOriginalPos, animationDuration)
            .SetEase(Ease.InCubic)
            .SetUpdate(true));
    }
}