using UnityEngine;
using UnityEngine.SceneManagement;


public class Button_Manager : MonoBehaviour
{
    private bool isPause = false;
    private PauseMenu pauseMenu;
    private ResultMenu resultMenu;
    private Data_Manager dataManager;

    private LoadingEffect loadingEffect;
    private HistoryUI historyUI;

    private void Start()
    {
        dataManager = Data_Manager.Instance;

        loadingEffect = LoadingEffect.Instance;

        pauseMenu = PauseMenu.FindAnyObjectByType<PauseMenu>();
        resultMenu = ResultMenu.FindAnyObjectByType<ResultMenu>();
        historyUI = HistoryUI.FindAnyObjectByType<HistoryUI>();
    }

    public void TogglePause()
    {
        if (pauseMenu.IsAnimating()) return;

        if (isPause)
        {
            isPause = false;
            Time_System.Instance.StartTimer();
            pauseMenu.HidePauseMenu();
        }
        else
        {
            isPause = true;
            Time_System.Instance.StopTimer();
            pauseMenu.ShowPauseMenu();
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        dataManager.SaveCurrentScore();

        Point_System.Instance.ResetPoint();
        Time_System.Instance.ResetTime();
        Game_System.Instance.ResetGame();

        if (isPause)
            TogglePause();
        resultMenu.HideResultMenu();
    }

    public void Menu()
    {
        Time.timeScale = 1f;
        dataManager.SaveCurrentScore();
        loadingEffect.LoadSceneWithTransition("HomeScene");
    }

    public void Play()
    {
        Time.timeScale = 1f;
        loadingEffect.LoadSceneWithTransition("GameScene");
    }

    public void ToggleHistory()
    {
        historyUI.OpenHistory();
    }

    public void ClearHistory()
    {
        dataManager.ClearHistory();
        historyUI.OpenHistory();
    }

    public void Revive()
    {
        Ads_Manager.Instance.ShowReviveAd();
    }
}
